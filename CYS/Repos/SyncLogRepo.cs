using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class SyncLogRepo
    {
        private readonly string _connectionString;

        public SyncLogRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<SyncLog> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<SyncLog, Kullanici, SyncLog>(@"
                    SELECT sl.*, k.* 
                    FROM SyncLog sl
                    LEFT JOIN Kullanici k ON sl.KullaniciId = k.Id
                    ORDER BY sl.SyncTarihi DESC",
                    (syncLog, kullanici) => {
                        syncLog.Kullanici = kullanici;
                        return syncLog;
                    },
                    splitOn: "Id").ToList();
            }
        }

        public SyncLog GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<SyncLog, Kullanici, SyncLog>(@"
                    SELECT sl.*, k.* 
                    FROM SyncLog sl
                    LEFT JOIN Kullanici k ON sl.KullaniciId = k.Id
                    WHERE sl.Id = @Id",
                    (syncLog, kullanici) => {
                        syncLog.Kullanici = kullanici;
                        return syncLog;
                    },
                    new { Id = id },
                    splitOn: "Id").FirstOrDefault();
            }
        }

        public int Add(SyncLog syncLog)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO SyncLog (KullaniciId, SyncTarihi, CihazId, CihazAdi, CihazIP, YuklenenKayitSayisi, IndirilenKayitSayisi, SyncDurumu, HataMesaji)
                    VALUES (@KullaniciId, @SyncTarihi, @CihazId, @CihazAdi, @CihazIP, @YuklenenKayitSayisi, @IndirilenKayitSayisi, @SyncDurumu, @HataMesaji);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, syncLog);
            }
        }

        public SyncLog GetLastSync(int kullaniciId, string cihazId = null)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT sl.*, k.* 
                    FROM SyncLog sl
                    LEFT JOIN Kullanici k ON sl.KullaniciId = k.Id
                    WHERE sl.KullaniciId = @KullaniciId";
                
                object parameters;
                
                if (!string.IsNullOrEmpty(cihazId))
                {
                    sql += " AND sl.CihazId = @CihazId";
                    parameters = new { KullaniciId = kullaniciId, CihazId = cihazId };
                }
                else
                {
                    parameters = new { KullaniciId = kullaniciId };
                }
                
                sql += " ORDER BY sl.SyncTarihi DESC LIMIT 1";
                
                return db.Query<SyncLog, Kullanici, SyncLog>(
                    sql,
                    (syncLog, kullanici) => {
                        syncLog.Kullanici = kullanici;
                        return syncLog;
                    },
                    parameters,
                    splitOn: "Id").FirstOrDefault();
            }
        }

        public List<SyncLog> GetRecentSyncs(int kullaniciId, DateTime baslangicTarihi)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<SyncLog, Kullanici, SyncLog>(@"
                    SELECT sl.*, k.* 
                    FROM SyncLog sl
                    LEFT JOIN Kullanici k ON sl.KullaniciId = k.Id
                    WHERE sl.KullaniciId = @KullaniciId AND sl.SyncTarihi >= @BaslangicTarihi
                    ORDER BY sl.SyncTarihi DESC",
                    (syncLog, kullanici) => {
                        syncLog.Kullanici = kullanici;
                        return syncLog;
                    },
                    new { KullaniciId = kullaniciId, BaslangicTarihi = baslangicTarihi },
                    splitOn: "Id").ToList();
            }
        }

        public List<SyncLog> GetByDateRange(DateTime baslangic, DateTime bitis)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<SyncLog, Kullanici, SyncLog>(@"
                    SELECT sl.*, k.* 
                    FROM SyncLog sl
                    LEFT JOIN Kullanici k ON sl.KullaniciId = k.Id
                    WHERE sl.SyncTarihi BETWEEN @Baslangic AND @Bitis
                    ORDER BY sl.SyncTarihi DESC",
                    (syncLog, kullanici) => {
                        syncLog.Kullanici = kullanici;
                        return syncLog;
                    },
                    new { Baslangic = baslangic, Bitis = bitis },
                    splitOn: "Id").ToList();
            }
        }

        public bool DeleteOlderThan(DateTime date)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM SyncLog WHERE SyncTarihi < @Date";
                int rowsAffected = db.Execute(sql, new { Date = date });
                return rowsAffected > 0;
            }
        }
    }
} 