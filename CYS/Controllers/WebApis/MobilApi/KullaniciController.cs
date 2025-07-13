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
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciRepo _kullaniciRepo;
        private readonly RolRepo _rolRepo;
        private readonly IzinRepo _izinRepo;
        private readonly AktiviteLogRepo _logRepo;

        public KullaniciController()
        {
            _kullaniciRepo = new KullaniciRepo();
            _rolRepo = new RolRepo();
            _izinRepo = new IzinRepo();
            _logRepo = new AktiviteLogRepo();
        }

        // GET: api/Kullanici
        [HttpGet]
        public IActionResult GetKullanicilar()
        {
            try
            {
                var kullanicilar = _kullaniciRepo.GetAll();
                return Ok(new { success = true, data = kullanicilar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Kullanici/5
        [HttpGet("{id}")]
        public IActionResult GetKullanici(int id)
        {
            try
            {
                var kullanici = _kullaniciRepo.GetById(id);
                if (kullanici == null)
                {
                    return NotFound(new { error = true, message = "Kullanıcı bulunamadı" });
                }
                return Ok(new { success = true, data = kullanici });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Kullanici
        [HttpPost]
        public IActionResult CreateKullanici([FromBody] Kullanici kullanici)
        {
            if (kullanici == null)
            {
                return BadRequest(new { error = true, message = "Geçersiz kullanıcı verisi" });
            }

            try
            {
                // Email ve kullanıcı adı kontrolü
                if (_kullaniciRepo.IsEmailExists(kullanici.Email))
                {
                    return BadRequest(new { error = true, message = "Bu email adresi zaten kullanılıyor" });
                }

                if (_kullaniciRepo.IsUsernameExists(kullanici.KullaniciAdi))
                {
                    return BadRequest(new { error = true, message = "Bu kullanıcı adı zaten kullanılıyor" });
                }

                // Yeni kullanıcı oluşturma
                kullanici.KayitTarihi = DateTime.Now;
                kullanici.Aktif = true;
                
                int yeniId = _kullaniciRepo.Add(kullanici);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    Islem = "Kullanıcı Ekleme",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = $"Yeni kullanıcı eklendi: {kullanici.KullaniciAdi}",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return CreatedAtAction(nameof(GetKullanici), new { id = yeniId }, 
                    new { success = true, message = "Kullanıcı başarıyla oluşturuldu", id = yeniId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // PUT: api/Kullanici/5
        [HttpPut("{id}")]
        public IActionResult UpdateKullanici(int id, [FromBody] Kullanici kullanici)
        {
            if (kullanici == null || id != kullanici.Id)
            {
                return BadRequest(new { error = true, message = "Geçersiz kullanıcı verisi" });
            }

            try
            {
                var mevcutKullanici = _kullaniciRepo.GetById(id);
                if (mevcutKullanici == null)
                {
                    return NotFound(new { error = true, message = "Kullanıcı bulunamadı" });
                }

                // Email kontrolü (kendi emaili hariç)
                if (mevcutKullanici.Email != kullanici.Email && _kullaniciRepo.IsEmailExists(kullanici.Email))
                {
                    return BadRequest(new { error = true, message = "Bu email adresi zaten kullanılıyor" });
                }

                // Kullanıcı adı kontrolü (kendi kullanıcı adı hariç)
                if (mevcutKullanici.KullaniciAdi != kullanici.KullaniciAdi && 
                    _kullaniciRepo.IsUsernameExists(kullanici.KullaniciAdi))
                {
                    return BadRequest(new { error = true, message = "Bu kullanıcı adı zaten kullanılıyor" });
                }

                _kullaniciRepo.Update(kullanici);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    Islem = "Kullanıcı Güncelleme",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = $"Kullanıcı güncellendi: {kullanici.KullaniciAdi}",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return Ok(new { success = true, message = "Kullanıcı başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // DELETE: api/Kullanici/5
        [HttpDelete("{id}")]
        public IActionResult DeleteKullanici(int id)
        {
            try
            {
                var kullanici = _kullaniciRepo.GetById(id);
                if (kullanici == null)
                {
                    return NotFound(new { error = true, message = "Kullanıcı bulunamadı" });
                }

                _kullaniciRepo.Delete(id);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    Islem = "Kullanıcı Silme",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = $"Kullanıcı silindi: {kullanici.KullaniciAdi}",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return Ok(new { success = true, message = "Kullanıcı başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // PUT: api/Kullanici/profil-guncelle
        [HttpPut("profil-guncelle")]
        public IActionResult UpdateProfil([FromBody] Kullanici kullanici)
        {
            if (kullanici == null)
            {
                return BadRequest(new { error = true, message = "Geçersiz kullanıcı verisi" });
            }

            try
            {
                // Oturum açmış kullanıcı kontrolü
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized(new { error = true, message = "Yetki hatası" });
                }

                int userId = int.Parse(User.Identity.Name);
                var mevcutKullanici = _kullaniciRepo.GetById(userId);
                
                if (mevcutKullanici == null)
                {
                    return NotFound(new { error = true, message = "Kullanıcı bulunamadı" });
                }

                // Sadece belirli alanları güncelle
                mevcutKullanici.Ad = kullanici.Ad;
                mevcutKullanici.Soyad = kullanici.Soyad;
                mevcutKullanici.Telefon = kullanici.Telefon;
                mevcutKullanici.ProfilResmi = kullanici.ProfilResmi;
                mevcutKullanici.Tema = kullanici.Tema;
                mevcutKullanici.Dil = kullanici.Dil;

                // Email değişikliği kontrolü
                if (mevcutKullanici.Email != kullanici.Email && _kullaniciRepo.IsEmailExists(kullanici.Email))
                {
                    return BadRequest(new { error = true, message = "Bu email adresi zaten kullanılıyor" });
                }
                mevcutKullanici.Email = kullanici.Email;

                _kullaniciRepo.Update(mevcutKullanici);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = userId,
                    Islem = "Profil Güncelleme",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = "Kullanıcı kendi profilini güncelledi",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return Ok(new { success = true, message = "Profil başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Kullanici/roller
        [HttpGet("roller")]
        public IActionResult GetRoller()
        {
            try
            {
                var roller = _rolRepo.GetAll();
                return Ok(new { success = true, data = roller });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Kullanici/roller
        [HttpPost("roller")]
        public IActionResult CreateRol([FromBody] Rol rol)
        {
            if (rol == null)
            {
                return BadRequest(new { error = true, message = "Geçersiz rol verisi" });
            }

            try
            {
                // Rol adı kontrolü
                if (_rolRepo.IsRolNameExists(rol.RolAdi))
                {
                    return BadRequest(new { error = true, message = "Bu rol adı zaten kullanılıyor" });
                }

                rol.OlusturmaTarihi = DateTime.Now;
                rol.Aktif = true;
                
                int rolId = _rolRepo.Add(rol);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    Islem = "Rol Ekleme",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = $"Yeni rol eklendi: {rol.RolAdi}",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return CreatedAtAction(nameof(GetRoller), new { success = true, message = "Rol başarıyla oluşturuldu", id = rolId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Kullanici/izinler
        [HttpGet("izinler")]
        public IActionResult GetIzinler()
        {
            try
            {
                var izinler = _izinRepo.GetAll();
                return Ok(new { success = true, data = izinler });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Kullanici/rol-izin-ata
        [HttpPost("rol-izin-ata")]
        public IActionResult AssignPermissionToRole([FromBody] RolIzinAtamaModel model)
        {
            if (model == null || model.RolId <= 0 || model.IzinId <= 0)
            {
                return BadRequest(new { error = true, message = "Geçersiz veri" });
            }

            try
            {
                var rol = _rolRepo.GetById(model.RolId);
                var izin = _izinRepo.GetById(model.IzinId);
                
                if (rol == null)
                {
                    return NotFound(new { error = true, message = "Rol bulunamadı" });
                }
                
                if (izin == null)
                {
                    return NotFound(new { error = true, message = "İzin bulunamadı" });
                }
                
                // İzin ataması yap
                _rolRepo.AssignPermission(model.RolId, model.IzinId);
                
                // Aktivite logu ekleme
                _logRepo.Add(new AktiviteLog
                {
                    KullaniciId = User.Identity.IsAuthenticated ? int.Parse(User.Identity.Name) : 0,
                    Islem = "Rol İzin Atama",
                    ModulAdi = "Kullanıcı Yönetimi",
                    IslemDetayi = $"Rol '{rol.RolAdi}' için izin '{izin.IzinAdi}' atandı",
                    IPAdresi = HttpContext.Connection.RemoteIpAddress.ToString(),
                    IslemTarihi = DateTime.Now
                });

                return Ok(new { success = true, message = "İzin başarıyla rol ile ilişkilendirildi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Kullanici/aktivite-log
        [HttpGet("aktivite-log")]
        public IActionResult GetAktiviteLog([FromQuery] int? kullaniciId = null, [FromQuery] string modul = null, 
            [FromQuery] DateTime? baslangicTarihi = null, [FromQuery] DateTime? bitisTarihi = null)
        {
            try
            {
                var logs = _logRepo.GetFiltered(kullaniciId, modul, baslangicTarihi, bitisTarihi);
                return Ok(new { success = true, data = logs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }
    }

    // İzin atama için model
    public class RolIzinAtamaModel
    {
        public int RolId { get; set; }
        public int IzinId { get; set; }
    }
} 