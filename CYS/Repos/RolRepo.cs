using CYS.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace CYS.Repos
{
    public class RolRepo
    {
        private readonly string _connectionString;

        public RolRepo()
        {
            _connectionString = RepoHelper.GetConnectionString();
        }

        public List<Rol> GetAll()
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Rol>("SELECT * FROM Rol WHERE Aktif = 1 ORDER BY RolAdi").ToList();
            }
        }

        public Rol GetById(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Rol>("SELECT * FROM Rol WHERE Id = @Id", new { Id = id });
            }
        }

        public bool IsRolNameExists(string rolAdi)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.ExecuteScalar<bool>(
                    "SELECT COUNT(1) FROM Rol WHERE RolAdi = @RolAdi", 
                    new { RolAdi = rolAdi });
            }
        }

        public int Add(Rol rol)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO Rol (RolAdi, Aciklama, OlusturmaTarihi, Aktif)
                    VALUES (@RolAdi, @Aciklama, @OlusturmaTarihi, @Aktif);
                    SELECT LAST_INSERT_ID();";

                return db.ExecuteScalar<int>(sql, rol);
            }
        }

        public bool Update(Rol rol)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = @"
                    UPDATE Rol
                    SET RolAdi = @RolAdi,
                        Aciklama = @Aciklama,
                        Aktif = @Aktif
                    WHERE Id = @Id";

                int rowsAffected = db.Execute(sql, rol);
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Soft delete - Aktif değerini false yap
                string sql = "UPDATE Rol SET Aktif = 0 WHERE Id = @Id";
                int rowsAffected = db.Execute(sql, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public List<Izin> GetRolIzinleri(int rolId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                return db.Query<Izin>(@"
                    SELECT i.* 
                    FROM Izin i
                    INNER JOIN RolIzin ri ON i.Id = ri.IzinId
                    WHERE ri.RolId = @RolId AND i.Aktif = 1", 
                    new { RolId = rolId }).ToList();
            }
        }

        public bool AssignPermission(int rolId, int izinId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                // Önce aynı rol-izin ikilisi var mı kontrol et
                bool exists = db.ExecuteScalar<bool>(
                    "SELECT COUNT(1) FROM RolIzin WHERE RolId = @RolId AND IzinId = @IzinId",
                    new { RolId = rolId, IzinId = izinId });
                
                if (exists)
                {
                    return true; // Zaten var, başarılı olarak kabul et
                }
                
                // Yoksa ekle
                string sql = @"
                    INSERT INTO RolIzin (RolId, IzinId)
                    VALUES (@RolId, @IzinId)";
                
                int rowsAffected = db.Execute(sql, new { RolId = rolId, IzinId = izinId });
                return rowsAffected > 0;
            }
        }

        public bool RemovePermission(int rolId, int izinId)
        {
            using (IDbConnection db = new MySqlConnection(_connectionString))
            {
                string sql = "DELETE FROM RolIzin WHERE RolId = @RolId AND IzinId = @IzinId";
                int rowsAffected = db.Execute(sql, new { RolId = rolId, IzinId = izinId });
                return rowsAffected > 0;
            }
        }
    }
} 