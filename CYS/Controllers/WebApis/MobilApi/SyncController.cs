using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;

namespace CYS.Controllers.WebApis.MobilApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly AnimalRepository _hayvanRepo;
        private readonly MobilOlcumCTX _olcumRepo;
        private readonly SyncLogRepo _syncLogRepo;
        private readonly KullaniciRepo _kullaniciRepo;

        public SyncController()
        {
            _hayvanRepo = new AnimalRepository();
            _olcumRepo = new MobilOlcumCTX();
            _syncLogRepo = new SyncLogRepo();
            _kullaniciRepo = new KullaniciRepo();
        }

        // GET: api/Sync/son-degisiklikler
        [HttpGet("son-degisiklikler")]
        public IActionResult GetSonDegisiklikler([FromQuery] DateTime sonSyncTarihi)
        {
            try
            {
                // Tarih kontrolü
                if (sonSyncTarihi > DateTime.Now)
                {
                    return BadRequest(new { error = true, message = "Geçersiz senkronizasyon tarihi" });
                }
                
                // Hayvanlar
                var hayvanlar = _hayvanRepo.GetChangedSince(sonSyncTarihi);
                
                // Ölçümler
                var olcumler = _olcumRepo.MobilOlcumList(
                    "SELECT * FROM mobilolcum WHERE tarih > @SonTarih", 
                    new { SonTarih = sonSyncTarihi });
                
                // Kullanıcı bilgisi için
                int kullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
                
                // User-Agent işleme
                string cihazId = Request.Headers["User-Agent"].ToString();
                cihazId = cihazId.Substring(0, Math.Min(20, cihazId.Length));
                
                // Senkronizasyon logu
                var syncLog = new SyncLog
                {
                    KullaniciId = kullaniciId,
                    SyncTarihi = DateTime.Now,
                    CihazId = cihazId,
                    CihazIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IndirilenKayitSayisi = hayvanlar.Count + (olcumler?.Count ?? 0),
                    YuklenenKayitSayisi = 0,
                    SyncDurumu = "Başarılı",
                    HataMesaji = null
                };
                
                _syncLogRepo.Add(syncLog);
                
                return Ok(new { 
                    success = true, 
                    timestamp = DateTime.Now,
                    hayvanlar = hayvanlar,
                    olcumler = olcumler
                });
            }
            catch (Exception ex)
            {
                // User-Agent işleme
                string cihazId = Request.Headers["User-Agent"].ToString();
                cihazId = cihazId.Substring(0, Math.Min(20, cihazId.Length));
                
                // Hata durumunda log ekle
                _syncLogRepo.Add(new SyncLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    SyncTarihi = DateTime.Now,
                    CihazId = cihazId,
                    CihazIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IndirilenKayitSayisi = 0,
                    YuklenenKayitSayisi = 0,
                    SyncDurumu = "Başarısız",
                    HataMesaji = ex.Message
                });
                
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Sync/toplu-gonder
        [HttpPost("toplu-gonder")]
        public IActionResult TopluGonder([FromBody] TopluVeriGonderModel model)
        {
            if (model == null)
            {
                return BadRequest(new { error = true, message = "Geçersiz veri" });
            }

            try
            {
                int eklenenHayvanSayisi = 0;
                int eklenenOlcumSayisi = 0;
                
                // Hayvanlar varsa ekle
                if (model.Hayvanlar != null && model.Hayvanlar.Any())
                {
                    foreach (var hayvan in model.Hayvanlar)
                    {
                        try
                        {
                            // Aynı RFID veya küpe no ile hayvan var mı kontrol et
                            var mevcutHayvan = _hayvanRepo.GetByRfidOrTag(hayvan.RfidNumber, hayvan.TagNumber);
                            
                            if (mevcutHayvan != null)
                            {
                                // Güncelleme yap
                                _hayvanRepo.Update(hayvan);
                            }
                            else
                            {
                                // Yeni ekle
                                _hayvanRepo.Add(hayvan);
                                eklenenHayvanSayisi++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Loglama yapılabilir
                            Console.WriteLine($"Hayvan ekleme hatası: {ex.Message}");
                        }
                    }
                }
                
                // Ölçümler varsa ekle
                if (model.Olcumler != null && model.Olcumler.Any())
                {
                    foreach (var olcum in model.Olcumler)
                    {
                        try
                        {
                            // Aynı ölçüm var mı kontrol et (RFID + Tarih + Ağırlık)
                            var mevcutOlcum = _olcumRepo.MobilOlcumTek(
                                "SELECT * FROM mobilolcum WHERE rfid = @Rfid AND ABS(weight - @Weight) < 0.1 AND tarih BETWEEN DATE_SUB(@Tarih, INTERVAL 1 MINUTE) AND DATE_ADD(@Tarih, INTERVAL 1 MINUTE)",
                                new { Rfid = olcum.Rfid, Weight = olcum.Weight, Tarih = olcum.Tarih });
                            
                            if (mevcutOlcum == null)
                            {
                                // Yeni ekle
                                _olcumRepo.MobilOlcumEkle(olcum);
                                eklenenOlcumSayisi++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Loglama yapılabilir
                            Console.WriteLine($"Ölçüm ekleme hatası: {ex.Message}");
                        }
                    }
                }
                
                // Kullanıcı bilgisi
                int kullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
                
                // User-Agent işleme
                string cihazId = Request.Headers["User-Agent"].ToString();
                cihazId = cihazId.Substring(0, Math.Min(20, cihazId.Length));
                
                // Senkronizasyon logu
                var syncLog = new SyncLog
                {
                    KullaniciId = kullaniciId,
                    SyncTarihi = DateTime.Now,
                    CihazId = cihazId,
                    CihazIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IndirilenKayitSayisi = 0,
                    YuklenenKayitSayisi = eklenenHayvanSayisi + eklenenOlcumSayisi,
                    SyncDurumu = "Başarılı",
                    HataMesaji = null
                };
                
                _syncLogRepo.Add(syncLog);
                
                return Ok(new { 
                    success = true, 
                    message = $"{eklenenHayvanSayisi} hayvan ve {eklenenOlcumSayisi} ölçüm başarıyla kaydedildi.",
                    eklenenHayvanSayisi = eklenenHayvanSayisi,
                    eklenenOlcumSayisi = eklenenOlcumSayisi
                });
            }
            catch (Exception ex)
            {
                // User-Agent işleme
                string cihazId = Request.Headers["User-Agent"].ToString();
                cihazId = cihazId.Substring(0, Math.Min(20, cihazId.Length));
                
                // Hata durumunda log ekle
                _syncLogRepo.Add(new SyncLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    SyncTarihi = DateTime.Now,
                    CihazId = cihazId,
                    CihazIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IndirilenKayitSayisi = 0,
                    YuklenenKayitSayisi = 0,
                    SyncDurumu = "Başarısız",
                    HataMesaji = ex.Message
                });
                
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Sync/durum
        [HttpGet("durum")]
        public IActionResult GetSyncDurumu([FromQuery] string cihazId = null)
        {
            try
            {
                // Kullanıcı bilgisi
                int kullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0;
                
                // Son senkronizasyon bilgisini getir
                var sonSync = _syncLogRepo.GetLastSync(kullaniciId, cihazId);
                
                // User-Agent işleme
                string suAnkiCihazId = Request.Headers["User-Agent"].ToString();
                suAnkiCihazId = suAnkiCihazId.Substring(0, Math.Min(20, suAnkiCihazId.Length));
                
                // Son 24 saat içindeki senkronizasyon loglarını getir
                var syncLogs = _syncLogRepo.GetRecentSyncs(kullaniciId, DateTime.Now.AddHours(-24));
                
                return Ok(new { 
                    success = true, 
                    cihazId = suAnkiCihazId,
                    sonSenkronizasyon = sonSync,
                    son24SaatSenkronizasyonlar = syncLogs,
                    sunucuZamani = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }
    }

    // Toplu veri gönderme modeli
    public class TopluVeriGonderModel
    {
        public List<Animal> Hayvanlar { get; set; }
        public List<MobilOlcum> Olcumler { get; set; }
    }
} 