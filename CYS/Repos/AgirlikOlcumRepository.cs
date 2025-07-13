using System;
using System.Collections.Generic;
using CYS.Data;
using CYS.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CYS.Repos
{
    public class AgirlikOlcumRepository : BaseRepository<AgirlikOlcum>
    {
        public AgirlikOlcumRepository(IConfiguration configuration) 
            : base(configuration, "agirlikolcum")
        {
        }

        public override void Add(AgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"INSERT INTO agirlikolcum (hayvanId, agirlik, olcumTarihi, bluetoothOlcum, userId, olcumNotu, requestId) 
                            VALUES (@hayvanId, @agirlik, @olcumTarihi, @bluetoothOlcum, @userId, @olcumNotu, @requestId)";
                
                connection.Execute(query, olcum);
            }
        }

        public override void Update(AgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"UPDATE agirlikolcum 
                            SET hayvanId = @hayvanId, 
                                agirlik = @agirlik, 
                                olcumTarihi = @olcumTarihi, 
                                bluetoothOlcum = @bluetoothOlcum, 
                                userId = @userId, 
                                olcumNotu = @olcumNotu 
                            WHERE id = @id";
                
                connection.Execute(query, olcum);
            }
        }

        public IEnumerable<AgirlikOlcum> GetByHayvanId(int hayvanId)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM agirlikolcum WHERE hayvanId = @hayvanId ORDER BY olcumTarihi DESC";
                return connection.Query<AgirlikOlcum>(query, new { hayvanId });
            }
        }
        
        // Tüm ağırlık ölçümlerini getiren yeni metod
        public IEnumerable<AgirlikOlcum> GetAll()
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM agirlikolcum ORDER BY olcumTarihi DESC";
                return connection.Query<AgirlikOlcum>(query);
            }
        }
        
        // ID'ye göre tek bir ağırlık ölçümü getiren metod
        public AgirlikOlcum GetById(int id)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM agirlikolcum WHERE id = @id";
                return connection.QueryFirstOrDefault<AgirlikOlcum>(query, new { id });
            }
        }
    }
} 