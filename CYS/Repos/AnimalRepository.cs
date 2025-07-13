using CYS.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYS.Repos
{
    public class AnimalRepository
    {
        private readonly HayvanCTX _hayvanCTX;

        public AnimalRepository()
        {
            _hayvanCTX = new HayvanCTX();
        }

        public List<Animal> GetAll()
        {
            var hayvanList = _hayvanCTX.hayvanList("SELECT * FROM hayvan", null);
            return hayvanList.Select(h => (Animal)h).ToList();
        }

        public Animal GetById(int id)
        {
            var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM hayvan WHERE id = @id", new { id });
            return hayvan;
        }

        public async Task<Animal> GetAnimalByRFIDAsync(string rfid)
        {
            var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM hayvan WHERE rfidKodu = @rfid", new { rfid });
            return hayvan;
        }

        public List<Animal> GetChangedSince(DateTime sonTarih)
        {
            var hayvanList = _hayvanCTX.hayvanList(
                "SELECT * FROM hayvan WHERE sonGuncelleme > @sonTarih OR tarih > @sonTarih",
                new { sonTarih });
            
            return hayvanList.Select(h => (Animal)h).ToList();
        }

        public Animal GetByRfidOrTag(string rfid, string tagNumber)
        {
            if (string.IsNullOrEmpty(rfid) && string.IsNullOrEmpty(tagNumber))
                return null;
                
            string query = "SELECT * FROM hayvan WHERE ";
            object param = null;
            
            if (!string.IsNullOrEmpty(rfid) && !string.IsNullOrEmpty(tagNumber))
            {
                query += "rfidKodu = @rfid OR kupeIsmi = @tagNumber";
                param = new { rfid, tagNumber };
            }
            else if (!string.IsNullOrEmpty(rfid))
            {
                query += "rfidKodu = @rfid";
                param = new { rfid };
            }
            else
            {
                query += "kupeIsmi = @tagNumber";
                param = new { tagNumber };
            }
            
            var hayvan = _hayvanCTX.hayvanTek(query, param);
            return hayvan ?? null;
        }

        public int Add(Animal animal)
        {
            Hayvan hayvan = new Hayvan
            {
                rfidKodu = animal.rfidKodu,
                kupeIsmi = animal.kupeIsmi,
                cinsiyet = animal.cinsiyet,
                agirlik = animal.agirlik,
                userId = animal.userId,
                kategoriId = animal.kategoriId,
                requestId = animal.requestId,
                aktif = animal.aktif,
                tarih = animal.tarih,
                ozellikler = animal.ozellikler,
                sonGuncelleme = animal.sonGuncelleme
            };

            return _hayvanCTX.hayvanEkle(hayvan);
        }

        public int Update(Animal animal)
        {
            Hayvan hayvan = new Hayvan
            {
                id = animal.id,
                rfidKodu = animal.rfidKodu,
                kupeIsmi = animal.kupeIsmi,
                cinsiyet = animal.cinsiyet,
                agirlik = animal.agirlik,
                userId = animal.userId,
                kategoriId = animal.kategoriId,
                requestId = animal.requestId,
                aktif = animal.aktif,
                tarih = animal.tarih,
                ozellikler = animal.ozellikler,
                sonGuncelleme = animal.sonGuncelleme
            };

            return _hayvanCTX.hayvanGuncelle(hayvan);
        }

        public async Task<List<Animal>> GetAnimalsAsync(bool? isActive = null, int? categoryId = null, string? searchTerm = null)
        {
            var query = "SELECT * FROM hayvan WHERE 1=1";
            var parameters = new Dictionary<string, object>();

            if (isActive.HasValue)
            {
                query += " AND aktif = @aktif";
                parameters.Add("aktif", isActive.Value ? 1 : 0);
            }

            if (categoryId.HasValue)
            {
                query += " AND kategoriId = @kategoriId";
                parameters.Add("kategoriId", categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (kupeIsmi LIKE @searchTerm OR rfidKodu LIKE @searchTerm)";
                parameters.Add("searchTerm", $"%{searchTerm}%");
            }

            var hayvanList = _hayvanCTX.hayvanList(query, parameters);
            return hayvanList.Select(h => (Animal)h).ToList();
        }

        public async Task<Animal?> GetAnimalByRfidAsync(string rfidTag)
        {
            var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM hayvan WHERE rfidKodu = @rfid", new { rfid = rfidTag });
            return hayvan;
        }

        public async Task<Animal?> GetAnimalDetailAsync(int id)
        {
            var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM hayvan WHERE id = @id", new { id });
            return hayvan;
        }

        public async Task<List<AgirlikOlcum>> GetWeightHistoryAsync(int animalId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = "SELECT * FROM agirlikolcum WHERE hayvanId = @hayvanId";
            var parameters = new Dictionary<string, object> { { "hayvanId", animalId } };

            if (startDate.HasValue)
            {
                query += " AND olcumTarihi >= @startDate";
                parameters.Add("startDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                query += " AND olcumTarihi <= @endDate";
                parameters.Add("endDate", endDate.Value);
            }

            query += " ORDER BY olcumTarihi DESC";

            var agirlikOlcumCTX = new AgirlikOlcumCTX();
            var measurements = agirlikOlcumCTX.agirlikOlcumList(query, parameters);
            return measurements.Select(m => new AgirlikOlcum
            {
                id = m.id,
                hayvanId = m.hayvanId,
                agirlik = m.agirlik,
                olcumTarihi = m.olcumTarihi,
                bluetoothOlcum = m.bluetoothOlcum,
                userId = m.userId,
                olcumNotu = m.olcumNotu,
                requestId = m.requestId,
                agirlikOlcumu = m.agirlikOlcumu,
                tarih = m.tarih
            }).ToList();
        }

        public async Task<bool> AddWeightMeasurementAsync(WeightMeasurementDto weightDto)
        {
            try
            {
                var measurement = new CYS.Models.AgirlikOlcum
                {
                    hayvanId = weightDto.AnimalId,
                    agirlik = weightDto.Weight,
                    olcumTarihi = weightDto.MeasurementDate,
                    bluetoothOlcum = false, // Default değer
                    userId = weightDto.UserId ?? 0,
                    olcumNotu = weightDto.Notes ?? "",
                    requestId = weightDto.RequestId ?? "",
                    aktif = true,
                    tarih = DateTime.Now
                };

                var agirlikOlcumCTX = new AgirlikOlcumCTX();
                agirlikOlcumCTX.agirlikOlcumEkle(measurement);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<object> GetHerdInfoAsync(int userId)
        {
            var totalAnimals = _hayvanCTX.hayvanList("SELECT COUNT(*) as count FROM hayvan WHERE userId = @userId AND aktif = 1", new { userId });
            var activeAnimals = _hayvanCTX.hayvanList("SELECT * FROM hayvan WHERE userId = @userId AND aktif = 1", new { userId });
            
            return new
            {
                TotalAnimals = totalAnimals.Count,
                ActiveAnimals = activeAnimals.Count,
                Categories = activeAnimals.GroupBy(h => h.kategoriId).Select(g => new { CategoryId = g.Key, Count = g.Count() })
            };
        }
    }
}