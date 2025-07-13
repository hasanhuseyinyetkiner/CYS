using System;
using System.Collections.Generic;
using CYS.Data;
using CYS.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CYS.Repos
{
    public class SuttenKesimAgirlikOlcumRepository : BaseRepository<SuttenKesimAgirlikOlcum>
    {
        public SuttenKesimAgirlikOlcumRepository(IConfiguration configuration) 
            : base(configuration, "suttenkesimagirlikolcum")
        {
        }

        public override void Add(SuttenKesimAgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"INSERT INTO suttenkesimagirlikolcum 
                              (hayvanId, agirlik, kesimTarihi, olcumTarihi, bluetoothOlcum, userId, olcumNotu, anneRfid, kesimYasi, requestId, agirlikOlcumu, aktif, tarih) 
                              VALUES 
                              (@hayvanId, @agirlik, @kesimTarihi, @olcumTarihi, @bluetoothOlcum, @userId, @olcumNotu, @anneRfid, @kesimYasi, @requestId, @agirlikOlcumu, @aktif, @tarih)";
                
                connection.Execute(query, olcum);
            }
        }

        public override void Update(SuttenKesimAgirlikOlcum olcum)
        {
            using (var connection = GetConnection())
            {
                var query = @"UPDATE suttenkesimagirlikolcum 
                            SET hayvanId = @hayvanId, 
                                agirlik = @agirlik, 
                                kesimTarihi = @kesimTarihi,
                                olcumTarihi = @olcumTarihi, 
                                bluetoothOlcum = @bluetoothOlcum, 
                                userId = @userId, 
                                olcumNotu = @olcumNotu,
                                anneRfid = @anneRfid,
                                kesimYasi = @kesimYasi,
                                aktif = @aktif
                            WHERE id = @id";
                
                connection.Execute(query, olcum);
            }
        }

        public IEnumerable<SuttenKesimAgirlikOlcum> GetByHayvanId(int hayvanId)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM suttenkesimagirlikolcum WHERE hayvanId = @hayvanId AND aktif = 1 ORDER BY olcumTarihi DESC";
                return connection.Query<SuttenKesimAgirlikOlcum>(query, new { hayvanId });
            }
        }
        
        public SuttenKesimAgirlikOlcum GetById(int id)
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM suttenkesimagirlikolcum WHERE id = @id AND aktif = 1";
                return connection.QueryFirstOrDefault<SuttenKesimAgirlikOlcum>(query, new { id });
            }
        }
        
        public IEnumerable<SuttenKesimAgirlikOlcum> GetAll()
        {
            using (var connection = GetConnection())
            {
                var query = "SELECT * FROM suttenkesimagirlikolcum WHERE aktif = 1 ORDER BY olcumTarihi DESC";
                return connection.Query<SuttenKesimAgirlikOlcum>(query);
            }
        }
    }
} 