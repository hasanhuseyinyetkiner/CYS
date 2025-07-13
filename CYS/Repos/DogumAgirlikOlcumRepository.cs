using System;
using System.Collections.Generic;
using CYS.Data;
using CYS.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CYS.Repos
{
    public class DogumAgirlikOlcumRepository : BaseRepository<DogumAgirlikOlcum>
    {
        public DogumAgirlikOlcumRepository(IConfiguration configuration) 
            : base(configuration, "dogumagirlikolcum")
        {
        }

        public override void Add(DogumAgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"INSERT INTO dogumagirlikolcum 
                              (hayvanId, agirlik, dogumTarihi, olcumTarihi, bluetoothOlcum, userId, olcumNotu, anneRfid, dogumYeri, requestId, agirlikOlcumu, aktif, tarih) 
                              VALUES 
                              (@hayvanId, @agirlik, @dogumTarihi, @olcumTarihi, @bluetoothOlcum, @userId, @olcumNotu, @anneRfid, @dogumYeri, @requestId, @agirlikOlcumu, @aktif, @tarih)";
                
                connection.Execute(query, olcum);
            }
        }

        public override void Update(DogumAgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"UPDATE dogumagirlikolcum 
                            SET hayvanId = @hayvanId, 
                                agirlik = @agirlik, 
                                dogumTarihi = @dogumTarihi,
                                olcumTarihi = @olcumTarihi, 
                                bluetoothOlcum = @bluetoothOlcum, 
                                userId = @userId, 
                                olcumNotu = @olcumNotu,
                                anneRfid = @anneRfid,
                                dogumYeri = @dogumYeri,
                                aktif = @aktif
                            WHERE id = @id";
                
                connection.Execute(query, olcum);
            }
        }

        public IEnumerable<DogumAgirlikOlcum> GetByHayvanId(int hayvanId)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM dogumagirlikolcum WHERE hayvanId = @hayvanId AND aktif = 1 ORDER BY olcumTarihi DESC";
                return connection.Query<DogumAgirlikOlcum>(query, new { hayvanId });
            }
        }
        
        public DogumAgirlikOlcum GetById(int id)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM dogumagirlikolcum WHERE id = @id AND aktif = 1";
                return connection.QueryFirstOrDefault<DogumAgirlikOlcum>(query, new { id });
            }
        }
        
        public IEnumerable<DogumAgirlikOlcum> GetAll()
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM dogumagirlikolcum WHERE aktif = 1 ORDER BY olcumTarihi DESC";
                return connection.Query<DogumAgirlikOlcum>(query);
            }
        }
    }
} 