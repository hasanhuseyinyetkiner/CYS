using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CYS.Repos
{
	public class MobilOlcumCTX
	{
		private readonly string _connectionString;
		private bool _initialized = false;

		public MobilOlcumCTX()
		{
			try
			{
				// Veritabanı bağlantısını yapılandırma dosyasından oku
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.Build();

				_connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
					"Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";
				_initialized = true;
				
				// Bağlantı testi
				TestConnection();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] MobilOlcumCTX başlatılamadı: {ex.Message}");
				_initialized = false;
			}
		}
		
		// Bağlantıyı test et
		private bool TestConnection()
		{
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					Console.WriteLine("[BILGI] Veritabanı bağlantısı başarılı");
					
					// mobilolcum tablosunu kontrol et
					bool tableExists = connection.ExecuteScalar<bool>("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'mobilolcum'");
					if (!tableExists)
					{
						Console.WriteLine("[UYARI] mobilolcum tablosu bulunamadı. Tablo oluşturuluyor...");
						
						// Tablo yoksa oluştur
						string createTableSql = @"
CREATE TABLE IF NOT EXISTS mobilolcum (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Rfid VARCHAR(100) NOT NULL,
    Weight FLOAT NOT NULL,
    CihazId INT DEFAULT 0,
    AmacId INT DEFAULT 1,
    HayvanId INT DEFAULT 0,
    Tarih DATETIME DEFAULT CURRENT_TIMESTAMP,
    OlcumTipi INT DEFAULT 0
)";
						connection.Execute(createTableSql);
						Console.WriteLine("[BILGI] mobilolcum tablosu oluşturuldu");
					}
					
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı bağlantı testi başarısız: {ex.Message}");
				return false;
			}
		}

		// MobilOlcum Listesi
		public List<MobilOlcum> MobilOlcumList(string sorgu, object param)
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return new List<MobilOlcum>();
			}
			
			try
			{
				Console.WriteLine($"[BILGI] Sorgu çalıştırılıyor: {sorgu}");
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					var result = connection.Query<MobilOlcum>(sorgu, param).ToList();
					Console.WriteLine($"[BILGI] Sorgu sonucu: {result.Count} kayıt");
					return result;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı sorgu hatası: {ex.Message}");
				Console.WriteLine($"[HATA] Sorgu: {sorgu}");
				if (param != null)
				{
					Console.WriteLine($"[HATA] Parametreler: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
				}
				return new List<MobilOlcum>();
			}
		}

		// Tek bir MobilOlcum getir
		public MobilOlcum MobilOlcumTek(string sorgu, object param)
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return null;
			}
			
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					return connection.Query<MobilOlcum>(sorgu, param).FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı sorgu hatası: {ex.Message}");
				Console.WriteLine($"[HATA] Sorgu: {sorgu}");
				if (param != null)
				{
					Console.WriteLine($"[HATA] Parametreler: {Newtonsoft.Json.JsonConvert.SerializeObject(param)}");
				}
				return null;
			}
		}

		// Yeni MobilOlcum ekle
		public int MobilOlcumEkle(MobilOlcum olcum)
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return 0;
			}
			
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					var sql = "INSERT INTO mobilolcum (rfid, weight, cihazid, amacid, hayvanid, tarih, olcumtipi) VALUES (@Rfid, @Weight, @CihazId, @AmacId, @HayvanId, @Tarih, @OlcumTipi); SELECT LAST_INSERT_ID();";
					int id = connection.ExecuteScalar<int>(sql, olcum);
					olcum.Id = id;
					Console.WriteLine($"[BILGI] Yeni ölçüm eklendi. ID: {id}, RFID: {olcum.Rfid}, Ağırlık: {olcum.Weight}, Ölçüm Tipi: {olcum.OlcumTipi}");
					return id;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı ekleme hatası: {ex.Message}");
				Console.WriteLine($"[HATA] Ölçüm: {Newtonsoft.Json.JsonConvert.SerializeObject(olcum)}");
				return 0;
			}
		}

		// Mevcut bir MobilOlcum güncelle
		public int MobilOlcumGuncelle(MobilOlcum olcum)
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return 0;
			}
			
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					var sql = "UPDATE mobilolcum SET rfid = @Rfid, weight = @Weight, cihazid = @CihazId, amacid = @AmacId, hayvanid = @HayvanId, tarih = @Tarih, olcumtipi = @OlcumTipi WHERE id = @Id";
					int affectedRows = connection.Execute(sql, olcum);
					Console.WriteLine($"[BILGI] Ölçüm güncellendi. ID: {olcum.Id}, Etkilenen satır: {affectedRows}");
					return affectedRows;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı güncelleme hatası: {ex.Message}");
				Console.WriteLine($"[HATA] Ölçüm: {Newtonsoft.Json.JsonConvert.SerializeObject(olcum)}");
				return 0;
			}
		}

		// Bir MobilOlcum sil
		public bool MobilOlcumSil(int id)
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return false;
			}
			
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					var sql = "DELETE FROM mobilolcum WHERE id = @Id";
					int affectedRows = connection.Execute(sql, new { Id = id });
					Console.WriteLine($"[BILGI] Ölçüm silindi. ID: {id}, Etkilenen satır: {affectedRows}");
					return affectedRows > 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Veritabanı silme hatası: {ex.Message}");
				Console.WriteLine($"[HATA] ID: {id}");
				return false;
			}
		}

		// Çift kayıt kontrolü
		public bool IsDuplicateMeasurement(string rfid, double weight, TimeSpan timeWindow)
		{
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					var sql = "SELECT COUNT(*) FROM mobilolcum WHERE rfid = @Rfid AND ABS(weight - @Weight) < 0.5 AND tarih > DATE_SUB(NOW(), INTERVAL @Minutes MINUTE)";
					int count = connection.ExecuteScalar<int>(sql, new { 
						Rfid = rfid, 
						Weight = weight, 
						Minutes = timeWindow.TotalMinutes 
					});
					return count > 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Çift kayıt kontrolü hatası: {ex.Message}");
				return false;
			}
		}

		// Örnek veri ekleme (test için)
		public bool EkleOrnekVeriler()
		{
			if (!_initialized)
			{
				Console.WriteLine("[HATA] MobilOlcumCTX başlatılmadı");
				return false;
			}
			
			try
			{
				using (var connection = new MySqlConnection(_connectionString))
				{
					connection.Open();
					
					// Önce mevcut kayıt sayısını kontrol et
					int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM mobilolcum");
					
					if (count > 0)
					{
						Console.WriteLine($"[BILGI] Tabloda {count} kayıt zaten var. Örnek veri eklenmiyor.");
						return true;
					}
					
					// Rastgele RFID kodu oluştur
					Random random = new Random();
					string GenerateRfid() => $"RFID{random.Next(10000, 99999)}";
					
					// Örnek veri oluştur (5 kayıt)
					List<MobilOlcum> ornekler = new List<MobilOlcum>();
					
					for (int i = 0; i < 5; i++)
					{
						ornekler.Add(new MobilOlcum
						{
							Rfid = GenerateRfid(),
							Weight = (float)(random.NextDouble() * 500 + 100), // 100-600 kg arası
							CihazId = 1,
							AmacId = random.Next(1, 4), // 1-3 arası
							HayvanId = random.Next(1, 10), // 1-9 arası hayvan ID
							Tarih = DateTime.Now.AddDays(-random.Next(0, 30)) // Son 30 gün içinde
						});
					}
					
					// Verileri ekle
					int eklenenKayit = 0;
					foreach (var ornek in ornekler)
					{
						var sql = "INSERT INTO mobilolcum (rfid, weight, cihazid, amacid, hayvanid, tarih) VALUES (@Rfid, @Weight, @CihazId, @AmacId, @HayvanId, @Tarih)";
						eklenenKayit += connection.Execute(sql, ornek);
					}
					
					Console.WriteLine($"[BILGI] {eklenenKayit} örnek kayıt eklendi");
					return eklenenKayit > 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[HATA] Örnek veri ekleme hatası: {ex.Message}");
				return false;
			}
		}
	}
}
