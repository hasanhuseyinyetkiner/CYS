using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class IzinRepo
    {
        private readonly string _connectionString;

        public IzinRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<Izin> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Izin>("SELECT * FROM Izin WHERE Aktif = 1 ORDER BY Modul, IzinAdi").ToList();
            }
        }

        public Izin GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Izin>("SELECT * FROM Izin WHERE Id = @Id", new { Id = id });
            }
        }

        public List<Izin> GetByModul(string modul)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Izin>(
                    "SELECT * FROM Izin WHERE Modul = @Modul AND Aktif = 1 ORDER BY IzinAdi", 
                    new { Modul = modul }).ToList();
            }
        }

        public bool IsIzinExists(string izinKodu)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.ExecuteScalar<bool>(
                    "SELECT COUNT(1) FROM Izin WHERE IzinKodu = @IzinKodu", 
                    new { IzinKodu = izinKodu });
            }
        }

        public int Add(Izin izin)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO Izin (IzinAdi, IzinKodu, Aciklama, Modul, Aktif)
                    VALUES (@IzinAdi, @IzinKodu, @Aciklama, @Modul, @Aktif);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, izin);
            }
        }

        public bool Update(Izin izin)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Izin
                    SET IzinAdi = @IzinAdi,
                        IzinKodu = @IzinKodu,
                        Aciklama = @Aciklama,
                        Modul = @Modul,
                        Aktif = @Aktif
                    WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, izin);
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Soft delete - Aktif değerini false yap
                string sql = "UPDATE Izin SET Aktif = 0 WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public List<string> GetAllModuller()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<string>("SELECT DISTINCT Modul FROM Izin WHERE Aktif = 1 ORDER BY Modul").ToList();
            }
        }

        public bool HardDelete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Önce ilişkili rol-izin kayıtlarını sil
                db.Execute("DELETE FROM RolIzin WHERE IzinId = @Id", new { Id = id });
                
                // Sonra izni sil
                string sql = "DELETE FROM Izin WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }
    }
} 