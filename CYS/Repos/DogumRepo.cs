using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class DogumRepo
    {
        private readonly string _connectionString;

        public DogumRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<Dogum> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Dogum, Animal, Animal, Dogum>(@"
                    SELECT d.*, a1.*, a2.* 
                    FROM Dogum d
                    LEFT JOIN Animal a1 ON d.AnneId = a1.Id
                    LEFT JOIN Animal a2 ON d.BabaId = a2.Id
                    ORDER BY d.DogumTarihi DESC",
                    (dogum, anne, baba) => {
                        dogum.Anne = anne;
                        dogum.Baba = baba;
                        return dogum;
                    },
                    splitOn: "Id,Id").ToList();
            }
        }

        public Dogum GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Dogum, Animal, Animal, Dogum>(@"
                    SELECT d.*, a1.*, a2.* 
                    FROM Dogum d
                    LEFT JOIN Animal a1 ON d.AnneId = a1.Id
                    LEFT JOIN Animal a2 ON d.BabaId = a2.Id
                    WHERE d.Id = @Id",
                    (dogum, anne, baba) => {
                        dogum.Anne = anne;
                        dogum.Baba = baba;
                        return dogum;
                    },
                    new { Id = id },
                    splitOn: "Id,Id").FirstOrDefault();
            }
        }

        public List<Dogum> GetByAnneId(int anneId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Dogum, Animal, Animal, Dogum>(@"
                    SELECT d.*, a1.*, a2.* 
                    FROM Dogum d
                    LEFT JOIN Animal a1 ON d.AnneId = a1.Id
                    LEFT JOIN Animal a2 ON d.BabaId = a2.Id
                    WHERE d.AnneId = @AnneId
                    ORDER BY d.DogumTarihi DESC",
                    (dogum, anne, baba) => {
                        dogum.Anne = anne;
                        dogum.Baba = baba;
                        return dogum;
                    },
                    new { AnneId = anneId },
                    splitOn: "Id,Id").ToList();
            }
        }

        public int Add(Dogum dogum)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO Dogum (AnneId, BabaId, DogumTarihi, YavruSayisi, Notlar, OluDogum, DogumYeri, KayitTarihi, KaydedenKullaniciId)
                    VALUES (@AnneId, @BabaId, @DogumTarihi, @YavruSayisi, @Notlar, @OluDogum, @DogumYeri, @KayitTarihi, @KaydedenKullaniciId);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, dogum);
            }
        }

        public bool Update(Dogum dogum)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Dogum
                    SET AnneId = @AnneId,
                        BabaId = @BabaId,
                        DogumTarihi = @DogumTarihi,
                        YavruSayisi = @YavruSayisi,
                        Notlar = @Notlar,
                        OluDogum = @OluDogum,
                        DogumYeri = @DogumYeri
                    WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, dogum);
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Dogum WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public List<Dogum> GetByDateRange(DateTime baslangic, DateTime bitis)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Dogum, Animal, Animal, Dogum>(@"
                    SELECT d.*, a1.*, a2.* 
                    FROM Dogum d
                    LEFT JOIN Animal a1 ON d.AnneId = a1.Id
                    LEFT JOIN Animal a2 ON d.BabaId = a2.Id
                    WHERE d.DogumTarihi BETWEEN @Baslangic AND @Bitis
                    ORDER BY d.DogumTarihi DESC",
                    (dogum, anne, baba) => {
                        dogum.Anne = anne;
                        dogum.Baba = baba;
                        return dogum;
                    },
                    new { Baslangic = baslangic, Bitis = bitis },
                    splitOn: "Id,Id").ToList();
            }
        }

        public List<Dogum> GetByKullaniciId(int kullaniciId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Dogum, Animal, Animal, Dogum>(@"
                    SELECT d.*, a1.*, a2.* 
                    FROM Dogum d
                    LEFT JOIN Animal a1 ON d.AnneId = a1.Id
                    LEFT JOIN Animal a2 ON d.BabaId = a2.Id
                    WHERE d.KaydedenKullaniciId = @KullaniciId
                    ORDER BY d.DogumTarihi DESC",
                    (dogum, anne, baba) => {
                        dogum.Anne = anne;
                        dogum.Baba = baba;
                        return dogum;
                    },
                    new { KullaniciId = kullaniciId },
                    splitOn: "Id,Id").ToList();
            }
        }
    }
} 