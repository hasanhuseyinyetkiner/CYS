using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;

namespace CYS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MobilOlcumController : ControllerBase
	{
		private readonly MobilOlcumCTX _mobilOlcumCTX;
		private readonly HayvanCTX _hayvanCTX;

		// Geçerli ağırlık aralığı (kg)
		private const double MIN_VALID_WEIGHT = 0.1;
		private const double MAX_VALID_WEIGHT = 2000.0;

		public MobilOlcumController()
		{
			_mobilOlcumCTX = new MobilOlcumCTX();
			_hayvanCTX = new HayvanCTX();
		}

		// GET: api/MobilOlcum
		[HttpGet]
		public IActionResult GetAll()
		{
			try
			{
				var result = _mobilOlcumCTX.MobilOlcumList("SELECT * FROM mobilolcum", null);
				
				// If empty result, return a more informative message
				if (result == null || result.Count == 0)
				{
					return Ok(new { 
						message = "Veri bulunamadı. Veritabanında hiç ölçüm verisi yok.", 
						success = true, 
						data = new List<MobilOlcum>() 
					});
				}
				
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/createSampleData
		[HttpGet("createSampleData")]
		public IActionResult CreateSampleData()
		{
			try
			{
				bool success = _mobilOlcumCTX.EkleOrnekVeriler();
				
				if (success)
				{
					return Ok(new { 
						success = true, 
						message = "Örnek veriler başarıyla eklendi." 
					});
				}
				else
				{
					return BadRequest(new { 
						success = false, 
						message = "Örnek veri eklenirken bir hata oluştu." 
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Örnek veri ekleme hatası: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/diagnostics
		[HttpGet("diagnostics")]
		public IActionResult Diagnostics()
		{
			try
			{
				// Test veritabanı bağlantısı
				bool connectionSuccess = false;
				string connectionError = "";
				
				try
				{
					// Basit bir sorgu yap
					var testQuery = _mobilOlcumCTX.MobilOlcumList("SELECT 1", null);
					connectionSuccess = true;
				}
				catch (Exception dbEx)
				{
					connectionError = dbEx.ToString();
				}
				
				// Tablo kontrolü - mobilolcum tablosu var mı?
				bool tableExists = false;
				int recordCount = 0;
				
				try
				{
					if (connectionSuccess)
					{
						var testTableQuery = _mobilOlcumCTX.MobilOlcumList("SHOW TABLES LIKE 'mobilolcum'", null);
						tableExists = testTableQuery != null && testTableQuery.Count > 0;
						
						if (tableExists)
						{
							var countQuery = _mobilOlcumCTX.MobilOlcumList("SELECT COUNT(*) AS count FROM mobilolcum", null);
							if (countQuery != null && countQuery.Count > 0)
							{
								// Bu düzgün çalışmayabilir, ama diagnostik amaçlı deniyoruz
								recordCount = countQuery.Count;
							}
						}
					}
				}
				catch (Exception tableEx)
				{
					// Hata durumunda sadece logla ve devam et
					Console.WriteLine($"Tablo kontrolü hatası: {tableEx.Message}");
				}
				
				return Ok(new { 
					timestamp = DateTime.Now,
					connectionTest = new {
						success = connectionSuccess,
						error = connectionError
					},
					databaseInfo = new {
						tableExists = tableExists,
						approximateRecordCount = recordCount,
						connectionString = "**HIDDEN**" // Güvenlik için gerçek bağlantı bilgisini gösterme
					},
					applicationInfo = new {
						mobilOlcumControllerInitialized = (_mobilOlcumCTX != null),
						hayvanControllerInitialized = (_hayvanCTX != null)
					}
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Teşhis çalıştırılamadı: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var result = _mobilOlcumCTX.MobilOlcumTek("SELECT * FROM mobilolcum WHERE Id = @Id", new { Id = id });
				if (result == null)
				{
					return NotFound(new { error = true, message = $"ID {id} bulunamadı" });
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		[HttpPost]
		public IActionResult Create([FromBody] MobilOlcum mobilOlcum)
		{
			if (mobilOlcum == null)
			{
				return BadRequest(new { error = true, message = "MobilOlcum null olamaz" });
			}

			if (string.IsNullOrEmpty(mobilOlcum.Rfid))
			{
				return BadRequest(new { error = true, message = "RFID gereklidir" });
			}

			if (!IsValidWeight(mobilOlcum.Weight))
			{
				return BadRequest(new { error = true, message = $"Ağırlık {MIN_VALID_WEIGHT} kg ile {MAX_VALID_WEIGHT} kg arasında olmalıdır" });
			}

			try
			{
				// RFID kontrolü - Hayvan veritabanında var mı?
				var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE rfidKodu = @rf", new { rf = mobilOlcum.Rfid });
				if (hayvan != null)
				{
					mobilOlcum.HayvanId = hayvan.id;
				}

				// Çift kayıt kontrolü - son 5 dakika içinde aynı RFID ve benzer ağırlıkta kayıt var mı?
				if (IsDuplicateMeasurement(mobilOlcum.Rfid, mobilOlcum.Weight, TimeSpan.FromMinutes(5)))
				{
					return BadRequest(new { 
						error = true, 
						message = "Son 5 dakika içinde benzer bir ölçüm zaten kaydedilmiş"
					});
				}

				// Tarih ayarla - eğer belirtilmemişse şimdiki zaman
				if (mobilOlcum.Tarih == DateTime.MinValue)
				{
					mobilOlcum.Tarih = DateTime.Now;
				}

				// Ölçüm amacını ayarla
				if (mobilOlcum.Amac == null)
				{
					mobilOlcum.Amac = GetOlcumTipiAdi(mobilOlcum.OlcumTipi);
				}

				int id = _mobilOlcumCTX.MobilOlcumEkle(mobilOlcum);
				
				if (id > 0)
				{
					mobilOlcum.Id = id;
					return Ok(new { 
						success = true, 
						message = "Ölçüm başarıyla kaydedildi", 
						data = mobilOlcum
					});
				}
				else
				{
					return BadRequest(new { error = true, message = "Ölçüm kaydedilemedi" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Sunucu hatası: {ex.Message}" });
			}
		}

		// PUT: api/MobilOlcum/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] MobilOlcum mobilOlcum)
		{
			if (mobilOlcum == null || mobilOlcum.Id != id)
			{
				return BadRequest(new { error = true, message = "Geçersiz veri" });
			}

			try
			{
				var existingMobilOlcum = _mobilOlcumCTX.MobilOlcumTek("SELECT * FROM mobilolcum WHERE Id = @Id", new { Id = id });
				if (existingMobilOlcum == null)
				{
					return NotFound(new { error = true, message = $"ID {id} bulunamadı" });
				}

				if (!IsValidWeight(mobilOlcum.Weight))
				{
					return BadRequest(new { error = true, message = $"Ağırlık {MIN_VALID_WEIGHT} kg ile {MAX_VALID_WEIGHT} kg arasında olmalıdır" });
				}

				// RFID kontrolü - Hayvan veritabanında var mı?
				var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE rfidKodu = @rf", new { rf = mobilOlcum.Rfid });
				if (hayvan != null)
				{
					mobilOlcum.HayvanId = hayvan.id;
				}

				// Ölçüm amacını ayarla
				SetMeasurementPurpose(mobilOlcum);

				_mobilOlcumCTX.MobilOlcumGuncelle(mobilOlcum);
				return Ok(new { success = true, message = "Ölçüm güncellendi" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Ölçüm güncellenemedi: {ex.Message}" });
			}
		}

		// DELETE: api/MobilOlcum/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				var existingMobilOlcum = _mobilOlcumCTX.MobilOlcumTek("SELECT * FROM mobilolcum WHERE Id = @Id", new { Id = id });
				if (existingMobilOlcum == null)
				{
					return NotFound(new { error = true, message = $"ID {id} bulunamadı" });
				}

				// Silme işlemi eklendi
				bool result = _mobilOlcumCTX.MobilOlcumSil(id);
				if (result)
				{
					return Ok(new { success = true, message = "Ölçüm silindi" });
				}
				else
				{
					return StatusCode(500, new { error = true, message = "Silme işlemi başarısız" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Silme hatası: {ex.Message}" });
			}
		}

		[HttpGet("GetLast20")]
		public IActionResult GetLast20MobilOlcum([FromQuery] int limit = 15)
		{
			try
			{
				// Güvenlik için limit kontrolü
				if (limit <= 0 || limit > 100)
				{
					limit = 15; // Varsayılan değer
				}

				MobilOlcumCTX ctx = new MobilOlcumCTX();
				var data = ctx.MobilOlcumList($"SELECT * FROM mobilolcum ORDER BY tarih DESC LIMIT {limit}", null);
				HayvanCTX hctx = new HayvanCTX();
				foreach(var item in data)
				{
					item.hayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rf", new {rf = item.Rfid});
					SetMeasurementPurpose(item);
				}
				return Ok(data);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// POST: api/MobilOlcum/bulk
		[HttpPost("bulk")]
		public IActionResult CreateBulk([FromBody] MobilOlcumBulkRequest request)
		{
			if (request == null || request.Measurements == null || !request.Measurements.Any())
			{
				return BadRequest(new { error = true, message = "Ölçüm verileri boş olamaz" });
			}

			// Güvenlik için veri sayısı kontrolü
			if (request.Measurements.Count > 1000)
			{
				return BadRequest(new { error = true, message = "En fazla 1000 ölçüm gönderilebilir" });
			}

			try
			{
				int successCount = 0;
				List<MobilOlcum> failedItems = new List<MobilOlcum>();
				List<MobilOlcum> duplicateItems = new List<MobilOlcum>();

				// Hayvan bilgilerini önbellekte tut (performans için)
				Dictionary<string, int> rfidToHayvanIdCache = new Dictionary<string, int>();

				foreach (var olcum in request.Measurements)
				{
					try
					{
						// Veri doğrulama
						if (string.IsNullOrEmpty(olcum.Rfid) || !IsValidWeight(olcum.Weight))
						{
							failedItems.Add(olcum);
							continue;
						}

						// RFID kontrolü - Hayvan veritabanında var mı?
						if (!rfidToHayvanIdCache.ContainsKey(olcum.Rfid))
						{
							var hayvan = _hayvanCTX.hayvanTek("SELECT id FROM Hayvan WHERE rfidKodu = @rf", new { rf = olcum.Rfid });
							if (hayvan != null)
							{
								rfidToHayvanIdCache[olcum.Rfid] = hayvan.id;
							}
						}

						if (rfidToHayvanIdCache.ContainsKey(olcum.Rfid))
						{
							olcum.HayvanId = rfidToHayvanIdCache[olcum.Rfid];
						}

						// Çift kayıt kontrolü - son 5 dakika içinde aynı RFID ve benzer ağırlıkta kayıt var mı?
						var lastMeasurement = _mobilOlcumCTX.MobilOlcumTek(
							"SELECT Id FROM mobilolcum WHERE rfid = @Rfid AND ABS(weight - @Weight) < 0.5 AND tarih > DATE_SUB(NOW(), INTERVAL 5 MINUTE)",
							new { Rfid = olcum.Rfid, Weight = olcum.Weight });

						if (lastMeasurement != null)
						{
							duplicateItems.Add(olcum);
							continue;
						}

						// Ölçüm amacını ayarla
						SetMeasurementPurpose(olcum);

						// Tarih kontrolü
						if (olcum.Tarih == DateTime.MinValue)
						{
							olcum.Tarih = DateTime.Now;
						}

						_mobilOlcumCTX.MobilOlcumEkle(olcum);
						successCount++;
					}
					catch (Exception ex)
					{
						// Hata logla
						Console.WriteLine($"Ölçüm kayıt hatası (RFID: {olcum.Rfid}): {ex.Message}");
						failedItems.Add(olcum);
					}
				}

				return Ok(new { 
					success = true, 
					message = $"{successCount} ölçüm başarıyla kaydedildi. {failedItems.Count} ölçüm başarısız. {duplicateItems.Count} ölçüm mükerrer.",
					failed_count = failedItems.Count,
					duplicate_count = duplicateItems.Count,
					success_count = successCount
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Toplu kayıt hatası: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/byRfid/{rfid}
		[HttpGet("byRfid/{rfid}")]
		public IActionResult GetByRfid(string rfid, [FromQuery] int limit = 10)
		{
			if (string.IsNullOrEmpty(rfid))
			{
				return BadRequest(new { error = true, message = "RFID gereklidir" });
			}

			try
			{
				// Güvenlik için limit kontrolü
				if (limit <= 0 || limit > 100)
				{
					limit = 10; // Varsayılan değer
				}

				var result = _mobilOlcumCTX.MobilOlcumList(
					"SELECT * FROM mobilolcum WHERE rfid = @Rfid ORDER BY tarih DESC LIMIT @Limit", 
					new { Rfid = rfid, Limit = limit }
				);

				if (result == null || !result.Any())
				{
					return NotFound(new { error = true, message = "Bu RFID'ye ait ölçüm bulunamadı" });
				}

				HayvanCTX hctx = new HayvanCTX();
				foreach (var item in result)
				{
					item.hayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rf", new { rf = item.Rfid });
					SetMeasurementPurpose(item);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/byDateRange
		[HttpGet("byDateRange")]
		public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			if (startDate > endDate)
			{
				return BadRequest(new { error = true, message = "Bitiş tarihi başlangıç tarihinden önce olamaz" });
			}

			// Güvenlik için tarih aralığı sınırlaması (max 1 ay)
			TimeSpan maxDateRange = TimeSpan.FromDays(30);
			if (endDate - startDate > maxDateRange)
			{
				return BadRequest(new { error = true, message = "Tarih aralığı en fazla 30 gün olabilir" });
			}

			try
			{
				var result = _mobilOlcumCTX.MobilOlcumList(
					"SELECT * FROM mobilolcum WHERE tarih BETWEEN @StartDate AND @EndDate ORDER BY tarih DESC", 
					new { StartDate = startDate, EndDate = endDate }
				);

				if (result == null || !result.Any())
				{
					return NotFound(new { error = true, message = "Bu tarih aralığında ölçüm bulunamadı" });
				}

				HayvanCTX hctx = new HayvanCTX();
				foreach (var item in result)
				{
					item.hayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rf", new { rf = item.Rfid });
					SetMeasurementPurpose(item);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// GET: api/MobilOlcum/listele
		[HttpGet("listele")]
		public IActionResult ListeAgirlikOlcumleri(
			[FromQuery] string rfid,
			[FromQuery] int? olcumTipi = null,
			[FromQuery] string siralama = "agirlik_azalan",
			[FromQuery] DateTime? baslangicTarihi = null,
			[FromQuery] DateTime? bitisTarihi = null)
		{
			try
			{
				if (string.IsNullOrEmpty(rfid))
				{
					return BadRequest(new { error = true, message = "RFID parametresi zorunludur" });
				}

				// Sorguyu oluştur
				string baseQuery = "SELECT * FROM mobilolcum WHERE Rfid = @rfid";
				var parameters = new { rfid };

				// Dinamik sorgu parametreleri
				var queryParams = new DynamicParameters(parameters);
				
				// Ölçüm tipi filtresi
				if (olcumTipi.HasValue)
				{
					baseQuery += " AND OlcumTipi = @olcumTipi";
					queryParams.Add("olcumTipi", olcumTipi.Value);
				}
				
				// Tarih aralığı filtresi
				if (baslangicTarihi.HasValue)
				{
					baseQuery += " AND Tarih >= @baslangicTarihi";
					queryParams.Add("baslangicTarihi", baslangicTarihi.Value);
				}
				
				if (bitisTarihi.HasValue)
				{
					baseQuery += " AND Tarih <= @bitisTarihi";
					queryParams.Add("bitisTarihi", bitisTarihi.Value);
				}
				
				// Sıralama
				switch (siralama.ToLower())
				{
					case "agirlik_azalan":
						baseQuery += " ORDER BY Weight DESC";
						break;
					case "agirlik_artan":
						baseQuery += " ORDER BY Weight ASC";
						break;
					case "tarih":
						baseQuery += " ORDER BY Tarih DESC";
						break;
					default:
						baseQuery += " ORDER BY Weight DESC";
						break;
				}
				
				// Sorguyu çalıştır
				var olcumler = _mobilOlcumCTX.MobilOlcumList(baseQuery, queryParams);
				
				if (olcumler == null || olcumler.Count == 0)
				{
					return Ok(new { 
						success = true, 
						message = "Belirtilen kriterlere uygun ölçüm bulunamadı.", 
						data = new List<MobilOlcum>() 
					});
				}
				
				// Ölçüm tiplerini isim olarak ayarla
				foreach (var olcum in olcumler)
				{
					olcum.Amac = GetOlcumTipiAdi(olcum.OlcumTipi);
				}
				
				return Ok(new { 
					success = true, 
					message = $"{olcumler.Count} ölçüm bulundu.", 
					data = olcumler
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// Ölçüm tipine göre filtreleme
		[HttpGet("tipler")]
		public IActionResult GetByOlcumTipi([FromQuery] int olcumTipi = 0)
		{
			try
			{
				var query = "SELECT * FROM mobilolcum WHERE OlcumTipi = @olcumTipi ORDER BY Tarih DESC";
				var olcumler = _mobilOlcumCTX.MobilOlcumList(query, new { olcumTipi });
				
				if (olcumler == null || olcumler.Count == 0)
				{
					return Ok(new { 
						success = true, 
						message = $"'{GetOlcumTipiAdi((OlcumTipi)olcumTipi)}' tipinde ölçüm bulunamadı.", 
						data = new List<MobilOlcum>()
					});
				}
				
				foreach (var olcum in olcumler)
				{
					olcum.Amac = GetOlcumTipiAdi(olcum.OlcumTipi);
				}
				
				return Ok(new { 
					success = true, 
					message = $"{olcumler.Count} ölçüm bulundu.", 
					data = olcumler
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = true, message = $"Veri alınamadı: {ex.Message}" });
			}
		}

		// Ölçüm tipinin adını döndüren yardımcı metod
		private string GetOlcumTipiAdi(OlcumTipi olcumTipi)
		{
			switch (olcumTipi)
			{
				case OlcumTipi.Normal:
					return "Normal Ağırlık";
				case OlcumTipi.SuttenKesim:
					return "Sütten Kesim Ağırlığı";
				case OlcumTipi.YeniDogmus:
					return "Yeni Doğmuş Ağırlık";
				default:
					return "Bilinmeyen Tip";
			}
		}

		// Yardımcı metodlar
		private bool IsValidWeight(double weight)
		{
			return weight >= MIN_VALID_WEIGHT && weight <= MAX_VALID_WEIGHT;
		}

		private void SetMeasurementPurpose(MobilOlcum olcum)
		{
			if (olcum.AmacId == 1)
				olcum.Amac = "Normal Tartım";
			else if (olcum.AmacId == 2)
				olcum.Amac = "İlk Doğum Ağırlığı";
			else if (olcum.AmacId == 3)
				olcum.Amac = "Sütten Kesim";
			else
				olcum.Amac = "Bilinmeyen Amaç";
		}

		private bool IsDuplicateMeasurement(string rfid, double weight, TimeSpan timeSpan)
		{
			var lastMeasurement = _mobilOlcumCTX.MobilOlcumTek(
				"SELECT * FROM mobilolcum WHERE rfid = @Rfid AND ABS(weight - @Weight) < 0.5 AND tarih > DATE_SUB(NOW(), INTERVAL @TimeSpan)",
				new { Rfid = rfid, Weight = weight, TimeSpan = timeSpan });

			return lastMeasurement != null;
		}
	}

	// Toplu ölçüm isteği için model
	public class MobilOlcumBulkRequest
	{
		public List<MobilOlcum> Measurements { get; set; }
	}
}
