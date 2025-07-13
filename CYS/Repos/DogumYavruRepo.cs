using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class DogumYavruRepo
    {
        private readonly string _connectionString;

        public DogumYavruRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<DogumYavru> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<DogumYavru, Dogum, Animal, DogumYavru>(@"
                    SELECT dy.*, d.*, a.* 
                    FROM DogumYavru dy
                    LEFT JOIN Dogum d ON dy.DogumId = d.Id
                    LEFT JOIN Animal a ON dy.HayvanId = a.Id
                    ORDER BY d.DogumTarihi DESC",
                    (dogumYavru, dogum, hayvan) => {
                        dogumYavru.Dogum = dogum;
                        dogumYavru.Hayvan = hayvan;
                        return dogumYavru;
                    },
                    splitOn: "Id,Id").ToList();
            }
        }

        public DogumYavru GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<DogumYavru, Dogum, Animal, DogumYavru>(@"
                    SELECT dy.*, d.*, a.* 
                    FROM DogumYavru dy
                    LEFT JOIN Dogum d ON dy.DogumId = d.Id
                    LEFT JOIN Animal a ON dy.HayvanId = a.Id
                    WHERE dy.Id = @Id",
                    (dogumYavru, dogum, hayvan) => {
                        dogumYavru.Dogum = dogum;
                        dogumYavru.Hayvan = hayvan;
                        return dogumYavru;
                    },
                    new { Id = id },
                    splitOn: "Id,Id").FirstOrDefault();
            }
        }

        public List<DogumYavru> GetByDogumId(int dogumId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<DogumYavru, Animal, DogumYavru>(@"
                    SELECT dy.*, a.* 
                    FROM DogumYavru dy
                    LEFT JOIN Animal a ON dy.HayvanId = a.Id
                    WHERE dy.DogumId = @DogumId",
                    (dogumYavru, hayvan) => {
                        dogumYavru.Hayvan = hayvan;
                        return dogumYavru;
                    },
                    new { DogumId = dogumId },
                    splitOn: "Id").ToList();
            }
        }

        public List<DogumYavru> GetByHayvanId(int hayvanId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<DogumYavru, Dogum, DogumYavru>(@"
                    SELECT dy.*, d.* 
                    FROM DogumYavru dy
                    LEFT JOIN Dogum d ON dy.DogumId = d.Id
                    WHERE dy.HayvanId = @HayvanId",
                    (dogumYavru, dogum) => {
                        dogumYavru.Dogum = dogum;
                        return dogumYavru;
                    },
                    new { HayvanId = hayvanId },
                    splitOn: "Id").ToList();
            }
        }

        public int Add(DogumYavru dogumYavru)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO DogumYavru (DogumId, HayvanId, Cinsiyet, DogumAgirligi, KupeNo, Notlar, Yasayan, KayitTarihi)
                    VALUES (@DogumId, @HayvanId, @Cinsiyet, @DogumAgirligi, @KupeNo, @Notlar, @Yasayan, @KayitTarihi);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, dogumYavru);
            }
        }

        public bool Update(DogumYavru dogumYavru)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE DogumYavru
                    SET DogumId = @DogumId,
                        HayvanId = @HayvanId,
                        Cinsiyet = @Cinsiyet,
                        DogumAgirligi = @DogumAgirligi,
                        KupeNo = @KupeNo,
                        Notlar = @Notlar,
                        Yasayan = @Yasayan
                    WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, dogumYavru);
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM DogumYavru WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public bool DeleteByDogumId(int dogumId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM DogumYavru WHERE DogumId = @DogumId";
                int rowsAffected = db.Execute(sql, new { DogumId = dogumId });
                return rowsAffected > 0;
            }
        }

        public bool DeleteByHayvanId(int hayvanId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM DogumYavru WHERE HayvanId = @HayvanId";
                int rowsAffected = db.Execute(sql, new { HayvanId = hayvanId });
                return rowsAffected > 0;
            }
        }
    }
} 