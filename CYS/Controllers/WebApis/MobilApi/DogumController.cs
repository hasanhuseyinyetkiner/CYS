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
    public class DogumController : ControllerBase
    {
        private readonly DogumRepo _dogumRepo;
        private readonly DogumYavruRepo _dogumYavruRepo;
        private readonly AnimalRepository _hayvanRepo;

        public DogumController()
        {
            _dogumRepo = new DogumRepo();
            _dogumYavruRepo = new DogumYavruRepo();
            _hayvanRepo = new AnimalRepository();
        }

        // GET: api/Dogum
        [HttpGet]
        public IActionResult GetDogumlar()
        {
            try
            {
                var dogumlar = _dogumRepo.GetAll();
                return Ok(new { success = true, data = dogumlar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Dogum/5
        [HttpGet("{id}")]
        public IActionResult GetDogum(int id)
        {
            try
            {
                var dogum = _dogumRepo.GetById(id);
                if (dogum == null)
                {
                    return NotFound(new { error = true, message = "Doğum kaydı bulunamadı" });
                }
                
                // Yavru hayvanları da getir
                dogum.Yavrular = _dogumYavruRepo.GetByDogumId(id);
                
                return Ok(new { success = true, data = dogum });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Dogum
        [HttpPost]
        public IActionResult CreateDogum([FromBody] DogumEkleModel model)
        {
            if (model == null || model.Dogum == null)
            {
                return BadRequest(new { error = true, message = "Geçersiz doğum verisi" });
            }

            try
            {
                // Anne hayvanı kontrol et
                var anne = _hayvanRepo.GetById(model.Dogum.AnneId);
                if (anne == null)
                {
                    return NotFound(new { error = true, message = "Anne hayvan bulunamadı" });
                }
                
                // Eğer baba ID'si varsa kontrol et
                if (model.Dogum.BabaId.HasValue)
                {
                    var baba = _hayvanRepo.GetById(model.Dogum.BabaId.Value);
                    if (baba == null)
                    {
                        return NotFound(new { error = true, message = "Baba hayvan bulunamadı" });
                    }
                }
                
                // Doğum tarihi kontrolü
                if (model.Dogum.DogumTarihi > DateTime.Now)
                {
                    return BadRequest(new { error = true, message = "Doğum tarihi gelecek bir tarih olamaz" });
                }
                
                // Kayıt tarihini ayarla
                model.Dogum.KayitTarihi = DateTime.Now;
                
                // Kullanıcı ID'sini belirle
                model.Dogum.KaydedenKullaniciId = User.Identity.IsAuthenticated ? 
                    int.Parse(User.Identity.Name) : 1; // Varsayılan olarak 1 kullanılıyor
                
                // Doğum kaydını oluştur
                int dogumId = _dogumRepo.Add(model.Dogum);
                
                // Yavrular varsa ekle
                if (model.Yavrular != null && model.Yavrular.Any())
                {
                    foreach (var yavru in model.Yavrular)
                    {
                        // Yavruyu hayvan tablosuna ekle
                        var hayvan = new Animal
                        {
                            kupeIsmi = yavru.HayvanAdi,
                            Species = anne.Species,
                            Gender = yavru.Cinsiyet == "E" ? "Male" : "Female",
                            DateOfBirth = model.Dogum.DogumTarihi,
                            cinsiyet = yavru.Cinsiyet,
                            tarih = model.Dogum.DogumTarihi,
                            kategoriId = anne.kategoriId,
                            aktif = 1,
                            ParentAnimalId = model.Dogum.AnneId,
                            FatherAnimalId = model.Dogum.BabaId
                        };
                        
                        int hayvanId = _hayvanRepo.Add(hayvan);
                        
                        // DogumYavru tablosuna ekle
                        var dogumYavru = new DogumYavru
                        {
                            DogumId = dogumId,
                            HayvanId = hayvanId,
                            Cinsiyet = yavru.Cinsiyet,
                            DogumAgirligi = yavru.DogumAgirligi,
                            KupeNo = yavru.KupeNo,
                            Notlar = yavru.Notlar,
                            Yasayan = !model.Dogum.OluDogum,
                            KayitTarihi = DateTime.Now
                        };
                        
                        _dogumYavruRepo.Add(dogumYavru);
                    }
                }
                
                return CreatedAtAction(nameof(GetDogum), new { id = dogumId }, 
                    new { success = true, message = "Doğum kaydı başarıyla oluşturuldu", id = dogumId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // PUT: api/Dogum/5
        [HttpPut("{id}")]
        public IActionResult UpdateDogum(int id, [FromBody] Dogum dogum)
        {
            if (dogum == null || id != dogum.Id)
            {
                return BadRequest(new { error = true, message = "Geçersiz doğum verisi" });
            }

            try
            {
                var mevcutDogum = _dogumRepo.GetById(id);
                if (mevcutDogum == null)
                {
                    return NotFound(new { error = true, message = "Doğum kaydı bulunamadı" });
                }

                // Anne hayvanı kontrol et
                var anne = _hayvanRepo.GetById(dogum.AnneId);
                if (anne == null)
                {
                    return NotFound(new { error = true, message = "Anne hayvan bulunamadı" });
                }
                
                // Eğer baba ID'si varsa kontrol et
                if (dogum.BabaId.HasValue)
                {
                    var baba = _hayvanRepo.GetById(dogum.BabaId.Value);
                    if (baba == null)
                    {
                        return NotFound(new { error = true, message = "Baba hayvan bulunamadı" });
                    }
                }
                
                // Doğum tarihi kontrolü
                if (dogum.DogumTarihi > DateTime.Now)
                {
                    return BadRequest(new { error = true, message = "Doğum tarihi gelecek bir tarih olamaz" });
                }
                
                _dogumRepo.Update(dogum);
                
                return Ok(new { success = true, message = "Doğum kaydı başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // DELETE: api/Dogum/5
        [HttpDelete("{id}")]
        public IActionResult DeleteDogum(int id)
        {
            try
            {
                var dogum = _dogumRepo.GetById(id);
                if (dogum == null)
                {
                    return NotFound(new { error = true, message = "Doğum kaydı bulunamadı" });
                }

                // Önce ilişkili yavruları sil
                _dogumYavruRepo.DeleteByDogumId(id);
                
                // Sonra doğum kaydını sil
                _dogumRepo.Delete(id);
                
                return Ok(new { success = true, message = "Doğum kaydı başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // POST: api/Dogum/toplu-ekle
        [HttpPost("toplu-ekle")]
        public IActionResult TopluYavruEkle([FromBody] DogumYavruTopluEkleModel model)
        {
            if (model == null || model.DogumId <= 0 || model.Yavrular == null || !model.Yavrular.Any())
            {
                return BadRequest(new { error = true, message = "Geçersiz yavru verisi" });
            }

            try
            {
                var dogum = _dogumRepo.GetById(model.DogumId);
                if (dogum == null)
                {
                    return NotFound(new { error = true, message = "Doğum kaydı bulunamadı" });
                }
                
                var anne = _hayvanRepo.GetById(dogum.AnneId);
                if (anne == null)
                {
                    return NotFound(new { error = true, message = "Anne hayvan bulunamadı" });
                }
                
                int basariliCount = 0;
                List<YavruEkleModel> basarisizlar = new List<YavruEkleModel>();
                
                foreach (var yavru in model.Yavrular)
                {
                    try
                    {
                        // Yavruyu hayvan tablosuna ekle
                        var hayvan = new Animal
                        {
                            kupeIsmi = yavru.HayvanAdi,
                            Species = anne.Species,
                            Gender = yavru.Cinsiyet == "E" ? "Male" : "Female",
                            DateOfBirth = dogum.DogumTarihi,
                            cinsiyet = yavru.Cinsiyet,
                            tarih = dogum.DogumTarihi,
                            kategoriId = anne.kategoriId,
                            aktif = 1,
                            ParentAnimalId = dogum.AnneId,
                            FatherAnimalId = dogum.BabaId
                        };
                        
                        int hayvanId = _hayvanRepo.Add(hayvan);
                        
                        // DogumYavru tablosuna ekle
                        var dogumYavru = new DogumYavru
                        {
                            DogumId = model.DogumId,
                            HayvanId = hayvanId,
                            Cinsiyet = yavru.Cinsiyet,
                            DogumAgirligi = yavru.DogumAgirligi,
                            KupeNo = yavru.KupeNo,
                            Notlar = yavru.Notlar,
                            Yasayan = !dogum.OluDogum,
                            KayitTarihi = DateTime.Now
                        };
                        
                        _dogumYavruRepo.Add(dogumYavru);
                        basariliCount++;
                    }
                    catch (Exception)
                    {
                        basarisizlar.Add(yavru);
                    }
                }
                
                // Doğum kaydındaki yavru sayısını güncelle
                var mevcutYavruSayisi = _dogumYavruRepo.GetByDogumId(model.DogumId).Count;
                dogum.YavruSayisi = mevcutYavruSayisi;
                _dogumRepo.Update(dogum);
                
                return Ok(new { 
                    success = true, 
                    message = $"{basariliCount} yavru başarıyla eklendi. {basarisizlar.Count} yavru eklenemedi.", 
                    basariliSayisi = basariliCount,
                    basarisizSayisi = basarisizlar.Count,
                    basarisizlar = basarisizlar
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // GET: api/Dogum/anneye-gore/{anneId}
        [HttpGet("anneye-gore/{anneId}")]
        public IActionResult GetDogumlarByAnne(int anneId)
        {
            try
            {
                var hayvan = _hayvanRepo.GetById(anneId);
                if (hayvan == null)
                {
                    return NotFound(new { error = true, message = "Hayvan bulunamadı" });
                }
                
                var dogumlar = _dogumRepo.GetByAnneId(anneId);
                
                return Ok(new { success = true, data = dogumlar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }
    }

    // Doğum kayıt modeli
    public class DogumEkleModel
    {
        public Dogum Dogum { get; set; }
        public List<YavruEkleModel> Yavrular { get; set; }
    }

    // Doğum yavru toplu ekleme modeli
    public class DogumYavruTopluEkleModel
    {
        public int DogumId { get; set; }
        public List<YavruEkleModel> Yavrular { get; set; }
    }

    // Yavru ekleme modeli
    public class YavruEkleModel
    {
        public string HayvanAdi { get; set; }
        public string Cinsiyet { get; set; }
        public double? DogumAgirligi { get; set; }
        public string KupeNo { get; set; }
        public string Notlar { get; set; }
    }
} 