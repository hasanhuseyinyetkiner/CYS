using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class KullaniciRepo
    {
        private readonly string _connectionString;

        public KullaniciRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<Kullanici> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Kullanici>("SELECT * FROM Kullanici WHERE Aktif = 1").ToList();
            }
        }

        public Kullanici GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Kullanici>("SELECT * FROM Kullanici WHERE Id = @Id", new { Id = id });
            }
        }

        public Kullanici GetByUsername(string username)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Kullanici>(
                    "SELECT * FROM Kullanici WHERE KullaniciAdi = @Username AND Aktif = 1", 
                    new { Username = username });
            }
        }

        public Kullanici GetByEmail(string email)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Kullanici>(
                    "SELECT * FROM Kullanici WHERE Email = @Email AND Aktif = 1", 
                    new { Email = email });
            }
        }

        public bool IsUsernameExists(string username)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.ExecuteScalar<bool>(
                    "SELECT COUNT(1) FROM Kullanici WHERE KullaniciAdi = @Username", 
                    new { Username = username });
            }
        }

        public bool IsEmailExists(string email)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.ExecuteScalar<bool>(
                    "SELECT COUNT(1) FROM Kullanici WHERE Email = @Email", 
                    new { Email = email });
            }
        }

        public int Add(Kullanici kullanici)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO Kullanici (KullaniciAdi, Email, Sifre, Ad, Soyad, Telefon, RolId, ProfilResmi, KayitTarihi, SonGirisTarihi, Aktif, Tema, Dil)
                    VALUES (@KullaniciAdi, @Email, @Sifre, @Ad, @Soyad, @Telefon, @RolId, @ProfilResmi, @KayitTarihi, @SonGirisTarihi, @Aktif, @Tema, @Dil);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, kullanici);
            }
        }

        public bool Update(Kullanici kullanici)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Kullanici
                    SET KullaniciAdi = @KullaniciAdi,
                        Email = @Email,
                        Sifre = @Sifre,
                        Ad = @Ad,
                        Soyad = @Soyad,
                        Telefon = @Telefon,
                        RolId = @RolId,
                        ProfilResmi = @ProfilResmi,
                        SonGirisTarihi = @SonGirisTarihi,
                        Aktif = @Aktif,
                        Tema = @Tema,
                        Dil = @Dil
                    WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, kullanici);
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Soft delete - Aktif değerini false yap
                string sql = "UPDATE Kullanici SET Aktif = 0 WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public bool HardDelete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Hard delete - Kaydı sil
                string sql = "DELETE FROM Kullanici WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public bool UpdateLastLogin(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "UPDATE Kullanici SET SonGirisTarihi = @Now WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id, Now = DateTime.Now });
                return rowsAffected > 0;
            }
        }
    }
} 