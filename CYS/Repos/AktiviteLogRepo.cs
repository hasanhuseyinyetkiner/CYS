using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class AktiviteLogRepo
    {
        private readonly string _connectionString;

        public AktiviteLogRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<AktiviteLog> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<AktiviteLog, Kullanici, AktiviteLog>(@"
                    SELECT al.*, k.* 
                    FROM AktiviteLog al
                    LEFT JOIN Kullanici k ON al.KullaniciId = k.Id
                    ORDER BY al.IslemTarihi DESC",
                    (aktiviteLog, kullanici) => {
                        aktiviteLog.Kullanici = kullanici;
                        return aktiviteLog;
                    },
                    splitOn: "Id").ToList();
            }
        }

        public List<AktiviteLog> GetFiltered(int? kullaniciId = null, string modul = null, 
            DateTime? baslangicTarihi = null, DateTime? bitisTarihi = null)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT al.*, k.* 
                    FROM AktiviteLog al
                    LEFT JOIN Kullanici k ON al.KullaniciId = k.Id
                    WHERE 1=1";
                
                var parameters = new DynamicParameters();
                
                if (kullaniciId.HasValue)
                {
                    sql += " AND al.KullaniciId = @KullaniciId";
                    parameters.Add("KullaniciId", kullaniciId.Value);
                }
                
                if (!string.IsNullOrEmpty(modul))
                {
                    sql += " AND al.ModulAdi = @Modul";
                    parameters.Add("Modul", modul);
                }
                
                if (baslangicTarihi.HasValue)
                {
                    sql += " AND al.IslemTarihi >= @BaslangicTarihi";
                    parameters.Add("BaslangicTarihi", baslangicTarihi.Value);
                }
                
                if (bitisTarihi.HasValue)
                {
                    sql += " AND al.IslemTarihi <= @BitisTarihi";
                    parameters.Add("BitisTarihi", bitisTarihi.Value);
                }
                
                sql += " ORDER BY al.IslemTarihi DESC";
                
                return db.Query<AktiviteLog, Kullanici, AktiviteLog>(
                    sql,
                    (aktiviteLog, kullanici) => {
                        aktiviteLog.Kullanici = kullanici;
                        return aktiviteLog;
                    },
                    parameters,
                    splitOn: "Id").ToList();
            }
        }

        public AktiviteLog GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<AktiviteLog, Kullanici, AktiviteLog>(@"
                    SELECT al.*, k.* 
                    FROM AktiviteLog al
                    LEFT JOIN Kullanici k ON al.KullaniciId = k.Id
                    WHERE al.Id = @Id",
                    (aktiviteLog, kullanici) => {
                        aktiviteLog.Kullanici = kullanici;
                        return aktiviteLog;
                    },
                    new { Id = id },
                    splitOn: "Id").FirstOrDefault();
            }
        }

        public int Add(AktiviteLog log)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO AktiviteLog (KullaniciId, Islem, ModulAdi, IslemDetayi, IPAdresi, Tarayici, IslemTarihi)
                    VALUES (@KullaniciId, @Islem, @ModulAdi, @IslemDetayi, @IPAdresi, @Tarayici, @IslemTarihi);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, log);
            }
        }

        public List<AktiviteLog> GetByKullaniciId(int kullaniciId, int limit = 100)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<AktiviteLog, Kullanici, AktiviteLog>(@"
                    SELECT al.*, k.* 
                    FROM AktiviteLog al
                    LEFT JOIN Kullanici k ON al.KullaniciId = k.Id
                    WHERE al.KullaniciId = @KullaniciId
                    ORDER BY al.IslemTarihi DESC
                    LIMIT @Limit",
                    (aktiviteLog, kullanici) => {
                        aktiviteLog.Kullanici = kullanici;
                        return aktiviteLog;
                    },
                    new { KullaniciId = kullaniciId, Limit = limit },
                    splitOn: "Id").ToList();
            }
        }

        public List<AktiviteLog> GetByDateRange(DateTime baslangic, DateTime bitis, int limit = 1000)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<AktiviteLog, Kullanici, AktiviteLog>(@"
                    SELECT al.*, k.* 
                    FROM AktiviteLog al
                    LEFT JOIN Kullanici k ON al.KullaniciId = k.Id
                    WHERE al.IslemTarihi BETWEEN @Baslangic AND @Bitis
                    ORDER BY al.IslemTarihi DESC
                    LIMIT @Limit",
                    (aktiviteLog, kullanici) => {
                        aktiviteLog.Kullanici = kullanici;
                        return aktiviteLog;
                    },
                    new { Baslangic = baslangic, Bitis = bitis, Limit = limit },
                    splitOn: "Id").ToList();
            }
        }

        public bool DeleteOlderThan(DateTime date)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM AktiviteLog WHERE IslemTarihi < @Date";
                int rowsAffected = db.Execute(sql, new { Date = date });
                return rowsAffected > 0;
            }
        }
    }
} 