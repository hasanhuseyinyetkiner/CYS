using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using RestSharp;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CYS.Controllers
{
	public class HayvanController : Controller
	{
		IMemoryCache _memoryCache;

		public HayvanController(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult HayvanListesi()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			HayvanCTX hctx = new HayvanCTX();
			var user = HttpContext.Session.GetString("user");
			var userObj = JsonConvert.DeserializeObject<Hayvan>(user);
			var hayvanListesi = hctx.hayvanList("select * from Hayvan where userId = @userId and aktif = 1 order by id desc", new { userId = userObj.id });
			
			ViewBag.list = hayvanTurListe();
			ViewBag.kriterler = kriterListe();
			return View(hayvanListesi);
		}
		[HttpGet]
		public IActionResult HayvanListesiJson()
		{
			// Oturum kontrolü
			if (sessionKontrol() == false)
				return Unauthorized();

			// Kullanıcıyı session'dan al
			var user = HttpContext.Session.GetString("user");
			var userObj = JsonConvert.DeserializeObject<User>(user); // Kullanıcı objesini al

			// Hayvanları ilgili kullanıcıya göre sıralı bir şekilde al
			HayvanCTX hctx = new HayvanCTX();
			var hayvanListesi = hctx.hayvanList("select * from Hayvan WHERE userId = @userId AND aktif = 1 ORDER BY id DESC", new { userId = userObj.id });

			// JSON formatında döndür
			return Json(new { data = hayvanListesi });
		}

		public IActionResult BuyukBas()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			HayvanCTX hctx = new HayvanCTX();
			var user = HttpContext.Session.GetString("user");
			agirliksuCTX aoctx = new agirliksuCTX();
			var liste = aoctx.agirliksuList("select * from agirliksu  ORDER BY tarih DESC LIMIT 20", null);
			return View(liste);
		}

		public IActionResult HayvanEkle()
		{
			return View();
		}
		public IActionResult HayvanSil(int hayvanId)
		{
			HayvanCTX hctx = new HayvanCTX();
			var hayvan = hctx.hayvanTek("select * from Hayvan where id = @id", new { id = hayvanId });
			if(hayvan != null)
			{
				hayvan.aktif = 0;
				hctx.hayvanGuncelle(hayvan);
			}
			return RedirectToAction("HayvanListesi");
		}
		public IActionResult HayvanDuzenle(int hayvanId)
		{
			ViewBag.HayvanId = hayvanId;
			HayvanCTX hctx = new HayvanCTX();
			var hayvan = hctx.hayvanTek("select * from Hayvan where id = @id", new { id = hayvanId });
			ViewBag.list = hayvanTurListe();

			return View(hayvan);
		}
		bool sessionKontrol()
		{
			try
			{

				var user = HttpContext.Session.GetString("user");
				var profile = HttpContext.Session.GetString("profile");

				if (user == null)
					return false;
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public JsonResult agirlikDondur(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if(user != null && profile != null)
			{
			var userObj = JsonConvert.DeserializeObject<User>(user);
				AgirlikOlcumCTX actx = new AgirlikOlcumCTX();
				var sonAgirlik = actx.agirlikOlcumTek("select * from agirlikolcum where userId = @userId and requestId = @requestId ", new { userId = userObj.id, requestId = requestId });
				if(sonAgirlik != null)
				{
					return Json(new { status = sonAgirlik.agirlikOlcumu, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
				}
				return Json(new { status = "0" });
			}
			return Json(new { status = "-1" });

		}
		public JsonResult rfidDondur(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				try
				{
					var userObj = JsonConvert.DeserializeObject<User>(user);
					kupeatamaCTX actx = new kupeatamaCTX();
					var sonAgirlik = actx.kupeAtamaTek("select * from kupeatama where userId = @userId and requestId = @requestId order by id asc limit 1", new { userId = userObj.id, requestId = requestId });
					if (sonAgirlik != null)
					{
						HayvanCTX hctx = new HayvanCTX();
						var hayvanVarmi = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu and aktif = 1", new { rfidKodu = sonAgirlik.kupeRfid });
						if (hayvanVarmi != null)
						{
							var mevcutHayvan = System.Text.Json.JsonSerializer.Serialize(hayvanVarmi);
							return Json(new { status = "mevcut", message = mevcutHayvan });

						}
						return Json(new { status = sonAgirlik.kupeRfid, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
					}
					return Json(new { status = "0" });

				}catch(Exception ex)

				{
					return Json(new { status = "err", message = ex.ToString() });

				}


			}
			return Json(new { status = "-1" });


		}
		public JsonResult hayvanEkleJson(string rfid, string hayvanAdi, int cinsiyet, int altTurId, string agirlik)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				string cinsiyetS = "";
				switch(cinsiyet)
				{
					case 0:
						cinsiyetS = "Erkek";
						break;
					case 1:
						cinsiyetS = "Dişi";
						break;
					case 2:
						cinsiyetS = "Bilinmeyen";
						break;
				}
				var userObj = JsonConvert.DeserializeObject<User>(user);
				HayvanCTX actx = new HayvanCTX();
				var hayvanVarMi = actx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu", new { rfidKodu = rfid });
				if(hayvanVarMi != null)
				{
					hayvanVarMi.cinsiyet = cinsiyetS;
					hayvanVarMi.kupeIsmi = hayvanAdi;
					hayvanVarMi.agirlik = agirlik;
					hayvanVarMi.rfidKodu = rfid;
					hayvanVarMi.userId = userObj.id;
					hayvanVarMi.kategoriId = Convert.ToInt32(altTurId);

					actx.hayvanGuncelle(hayvanVarMi);
					AgirlikHayvanCTX ahctx = new AgirlikHayvanCTX();
					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanVarMi.agirlik,
						tarih = DateTime.Now
					};
					ahctx.agirlikHayvanEkle(ah);
					return Json(new { status = "Success", message = "Mevcut Hayvan Başarıyla Güncellendi" });

				}
				Hayvan hy = new Hayvan()
				{
					cinsiyet = cinsiyetS,
					kupeIsmi = hayvanAdi,
					agirlik = agirlik,
					rfidKodu = rfid,
					userId = userObj.id,
                    kategoriId = Convert.ToInt32(altTurId)
                };
				actx.hayvanEkle(hy);
				var eklenenson = actx.hayvanTek("select * from Hayvan order by id desc limit 1", null);
				AgirlikHayvanCTX ahctx1 = new AgirlikHayvanCTX();
				agirlikHayvan ah1 = new agirlikHayvan()
				{
					hayvanId = eklenenson.id,
					agirlikId = eklenenson.agirlik,
					tarih = DateTime.Now
				};
				ahctx1.agirlikHayvanEkle(ah1);

				return Json(new { status = "Success", message = "Hayvan Başarıyla Eklendi" });

			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });
		}
		public JsonResult HayvanAgirlikGuncelleJson(string id, string agirlik)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				HayvanCTX actx = new HayvanCTX();
				var ilgiliHayvan = actx.hayvanTek("select * from Hayvan where id = @id", new { id = id });
				if(ilgiliHayvan != null)
				{

					ilgiliHayvan.agirlik = agirlik;
					actx.hayvanGuncelle(ilgiliHayvan);
					return Json(new { status = "Success", message = "Hayvan Başarıyla Güncellendi" });

				}
			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}
		public JsonResult HayvanAgirlikGuncelleJsonEnBuyuk(string requestId, string agirlik)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				AgirlikOlcumCTX actx = new AgirlikOlcumCTX();
				var ilgiliHayvan = actx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
				if (ilgiliHayvan != null)
				{

					ilgiliHayvan.agirlikOlcumu= agirlik;
					actx.agirlikOlcumGuncelle(ilgiliHayvan);
					return Json("");

				}
			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}
		public JsonResult HayvanDuzenleJson(string hayvanId, string rfid, string hayvanAdi, string agirlik, int cinsiyet, int kategoriId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				string cinsiyetS = "";
				switch (cinsiyet)
				{
					case 0:
						cinsiyetS = "Erkek";
						break;
					case 1:
						cinsiyetS = "Dişi";
						break;
					case 2:
						cinsiyetS = "Bilinmeyen";
						break;
				}
				HayvanCTX hctx = new HayvanCTX();
				var hayvan = hctx.hayvanTek("select * from Hayvan where id = @id", new { id = hayvanId });
				int eklenecekKategoriId = hayvan.kategoriId;
				if (kategoriId != -1)
					eklenecekKategoriId = kategoriId;
				if(hayvan != null)
				{
					hayvan.rfidKodu = rfid;
					hayvan.agirlik = agirlik;
					hayvan.cinsiyet = cinsiyetS;
					hayvan.kupeIsmi = hayvanAdi;
					hayvan.kategoriId = eklenecekKategoriId;
					hctx.hayvanGuncelle(hayvan);
					return Json(new { status = "Success", message = "Hayvan Başarıyla Güncellendi" });
				}
				

			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}
		public class hayvanTuru
		{
			public int Id;
			public string Text = string.Empty;
		}
		public List<SelectListItem> hayvanTurListe()
		{
            List<hayvanTuru> ustKategoriList = new List<hayvanTuru>();
			List<SelectListItem> itList = new List<SelectListItem>();
            ustKategoriCTX hctx = new ustKategoriCTX();
            var list = hctx.ustKategoriList("select * from ustkategori where isActive = 1", null);
            foreach (var item in list)
			{
				SelectListItem it = new SelectListItem() { Value = item.id.ToString(), Text = item.name };
				itList.Add(it);

            }

            return itList;
        }
		public List<SelectListItem> kriterListe()
		{
			KriterCTX kritCTX = new KriterCTX();
			List<Kriter> ustKategoriList = new List<Kriter>();
			List<SelectListItem> itList = new List<SelectListItem>();
			ustKategoriCTX hctx = new ustKategoriCTX();
			var kriterListesi = kritCTX.kriterList("select * from kriter", null);
			foreach (var item in kriterListesi)
			{
				SelectListItem it = new SelectListItem() { Value = item.id.ToString(), Text = item.kriterAdi };
				itList.Add(it);

			}

			return itList;
		}
		public JsonResult HayvanListesiJson(string q)
		{
            List<hayvanTuru> ustKategoriList = new List<hayvanTuru>();
			// Add parts to the list.
			ustKategoriCTX hctx = new ustKategoriCTX();
			var list = hctx.ustKategoriList("select * from ustkategori where isActive = 1",null);
			foreach(var item in list)
			{
                ustKategoriList.Add(new hayvanTuru() { Id = item.id, Text = item.name });

            }

			if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
			{
				ustKategoriList = ustKategoriList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
			}
			return Json(new { items = ustKategoriList });
        }
        public JsonResult hayvanAltTurJson(string value)
        {
            KategoriCTX hctx = new KategoriCTX();
            var ustKategoriList = hctx.KategoriList("select * from kategori where ustKategoriId = @ustKategoriId and isActive = 1", new {ustKategoriId = Convert.ToString(value)});
            var eleman = System.Text.Json.JsonSerializer.Serialize(ustKategoriList);
            return Json(eleman);
        }
		public JsonResult hayvanOzellikGetirJson(string value)
		{
			KriterUnsurCTX hctx = new KriterUnsurCTX();
			var ozellikList = hctx.kriterUnsurList("select * from kriterunsur where kriterId = @kriterId and isActive = 1", new { kriterId = Convert.ToString(value) });
			var eleman = System.Text.Json.JsonSerializer.Serialize(ozellikList);
			return Json(eleman);
		}
		public JsonResult hayvaninOzellikleri(int hayvanId)
		{
			KriterUnsurCTX hctx = new KriterUnsurCTX();
			var ozellikList = hctx.kriterUnsurList("select * from Hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = Convert.ToString(hayvanId) });
			var eleman = System.Text.Json.JsonSerializer.Serialize(ozellikList);
			return Json(eleman);
		}
		public JsonResult HayvanOzellikEkleJson( int kriterId, int unsurId, int hayvanId)
		{
			HayvanKriterUnsurCTX hayvanKriterUnsurCTX = new HayvanKriterUnsurCTX();
			KriterUnsurCTX kctx = new KriterUnsurCTX();

			var varmi = kctx.kriterUnsurTek("SELECT kriterunsur.* FROM kriter, kriterunsur, hayvankriterunsur, hayvan where hayvankriterunsur.kriterUnsurId = kriterunsur.id and kriterunsur.kriterId = kriter.id and kriter.id = @kriterId and hayvankriterunsur.isActive = 1 and hayvan.id = @hayvanId and hayvan.id = hayvankriterunsur.hayvanId", new { hayvanId = hayvanId, kriterId = kriterId });
			if(varmi != null)
			{
				var mevcut = hayvanKriterUnsurCTX.HayvanKriterUnsurTek("select * from Hayvankriterunsur where kriterUnsurId = @kriterUnsurId and hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId, kriterUnsurId = varmi.id });
				if(mevcut != null)
				{
					mevcut.kriterUnsurId = unsurId;
					hayvanKriterUnsurCTX.HayvanKriterUnsurGuncelle(mevcut);
					return Json(new { status = "Success", message = "Özellik Güncellendi..." });

				}
				return Json(new { status = "Error", message = "Bir Hata Oluştu..." });

			}

			HayvanKriterUnsur hayvanKriterUnsur = new HayvanKriterUnsur()
			{
				kriterUnsurId = unsurId,
				hayvanId = hayvanId
			};
			hayvanKriterUnsurCTX.HayvanKriterUnsurEkle(hayvanKriterUnsur);
			return Json(new { status = "Success", message = "Özellik Eklendi..." });

		}
		public JsonResult HayvanOzellikSilJson(int id)
		{
			HayvanKriterUnsurCTX hayvanKriterUnsurCTX = new HayvanKriterUnsurCTX();
			var varmi = hayvanKriterUnsurCTX.HayvanKriterUnsurTek("select * from Hayvankriterunsur where id = @id and isActive = 1", new { id = id});
			if (varmi != null)
			{
				varmi.isActive = 0;
				hayvanKriterUnsurCTX.HayvanKriterUnsurGuncelle(varmi);
				return Json(new { status = "Success", message = "Özellik Silindi..." });


			}
			return Json(new { status = "Error", message = "Özellik bulunamadı" });


		}
		public JsonResult rfidIstekJson(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
				kupeatamaCTX hctx = new kupeatamaCTX();
				KupeAtama eklenenId = null;

				try
				{
					eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
				}catch(Exception ex)
				{

					return Json(new { status = "error", message = ex.ToString() });

				}
				if (eklenenId == null)
				{
					KupeAtama kupe = new KupeAtama()
					{
						requestId = requestId,
						userId = userObj.id,
						kupeRfid = ""
					};
					try{
						hctx.kupeAtamaEkle(kupe);

					}catch(Exception ex)
					{
						return Json(new { status = "error", message = ex.ToString() });

					}
					eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
				}

				if(eklenenId == null)
				{
					return Json(new { status = "error", message = "Veri Eklenmede Hata" });

				}

				try
				{
					var client = new RestClient(profileObj.cihazLink + "/RFIDApi");
					client.Timeout = -1;
					var request = new RestRequest(Method.GET);
					IRestResponse response = client.Execute(request);
					var cevap = response.Content;
					//var gelen = JsonConvert.DeserializeObject<string>(cevap);
					if (cevap == "")
					{
						return Json(new { status = "error", message = "Boş Veri Geldi..." });

					}


					eklenenId.kupeRfid = cevap;

					hctx.kupeAtamaGuncelle(eklenenId);
					{ return Json(new { status = "success", message = cevap }); }
				}
				catch
				(Exception ex)
				{ return Json(new { status = "error", message = ex.ToString() }); }
				

			}
			return Json("");


		}
		public JsonResult agirlikIstekJson(string requestId)
		{
			if(requestId == null)
				return Json(new { status = "error", message = "Request Id Null" });

			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);

				AgirlikOlcum eklenenId = null;
				AgirlikOlcumCTX hctx = new AgirlikOlcumCTX();

				try
				{
					//bu request id ile eklenmiş veri var mı kontrolü
					eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
				}
				catch(Exception ex) 
				{

					return Json(new { status = "error", message = ex.ToString() }) ;

				}


				//Eğer ilk defa ölçüm yapılacaksa veri eklenecek...
				if (eklenenId == null)
				{
					AgirlikOlcum agirlik = new AgirlikOlcum()
					{
						requestId = requestId,
						userId = userObj.id,
						agirlikOlcumu = ""
					};
					try
					{
						hctx.agirlikOlcumEkle(agirlik);

					}catch(Exception ex)
					{
						return Json(new { status = "error", message = ex.ToString() });
					}
					//Veri eklendikten sonra eklenen veri tekrar elde ediliyor
					eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId order by id desc limit 1", new { requestId = requestId });
				}
				
				var client = new RestClient(profileObj.cihazLink+"/AgirlikApi");
				client.Timeout = 1000;
				var request = new RestRequest(Method.GET);
				try
				{
					IRestResponse response = client.Execute(request);
					var cevap = response.Content;
					//var gelen = JsonConvert.DeserializeObject<string>(cevap);
					if (cevap == "")
						return Json(new { status = "error", message = "Okuma işlemi gerçekleştirilemedi" });

					eklenenId.agirlikOlcumu = cevap;

					hctx.agirlikOlcumGuncelle(eklenenId);
					return Json(new { status = "success", message = cevap });

				}
				catch
				{

				}
				
			}
			return Json("");

			
		}

		public JsonResult agirlikIsteksadece()
		{
			
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				try
				{
					var userObj = JsonConvert.DeserializeObject<User>(user);
					var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
					var client = new RestClient(profileObj.cihazLink + "/AgirlikApi");
					client.Timeout = -1;
					var request = new RestRequest(Method.GET);
					IRestResponse response = client.Execute(request);
					var cevap = response.Content;
					//var gelen = JsonConvert.DeserializeObject<string>(cevap);
					if (cevap == "")
					{
						return Json("0.0");

					}
					return Json(cevap);
				}
				catch
				{
					return Json("0.0");
				}
			}
			return Json("");


		}

		public JsonResult hayvaninOzellikleriJson(int hayvanId)
		{
			HayvanKriterUnsurCTX hkctx = new HayvanKriterUnsurCTX();
			var list = hkctx.HayvanKriterUnsurList("select * from Hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId });
			var jsonObject = JsonConvert.SerializeObject(list);
			return Json(jsonObject);

		}

		public JsonResult son5Hayvan()
		{
			HayvanCTX hctx = new HayvanCTX();
			var son5 = hctx.hayvanList("select * from Hayvan where aktif = 1 order by id desc limit 5", null);
			var jsonObject = JsonConvert.SerializeObject(son5);
			return Json(jsonObject);
		}

		public JsonResult tumHayvanlar(string q)
		{
			HayvanCTX hctx = new HayvanCTX();
			var hepsi = hctx.hayvanList("select * from Hayvan where aktif = 1 order by id desc", null);
			var jsonObject = JsonConvert.SerializeObject(hepsi);
			return Json(jsonObject);
		}

		public JsonResult ustSoyEkleJson(int ustHayvanId, int hayvanId)
		{
			if (ustHayvanId == hayvanId)
				return Json("");
			UstSoyCTX usctx = new UstSoyCTX();
			var VarMi = usctx.soyagaciTek("select * from soyagaci where ustHayvanId = @ustHayvanId and hayvanId = @hayvanId and isActive = 1", new { ustHayvanId = ustHayvanId, hayvanId = hayvanId });
			if (VarMi != null)
				return Json("");
			soyagaci us = new soyagaci()
			{
				ustHayvanId = ustHayvanId,
				hayvanId = hayvanId
			};
			usctx.soyagaciEkle(us);
			return Json("");
		}
		public class miniTur
		{
			public int id { get; set; }

			public string HayvanTur { get; set; } = string.Empty;
			public string AltTur { get; set; } = string.Empty;
			public string RFIDKodu { get; set; } = string.Empty;
			public string Agirlik { get; set; } = string.Empty;
			public string img { get; set; } = string.Empty;

		}
		public JsonResult hayvanAkraba(int hayvanId)
		{
			UstSoyCTX usctx = new UstSoyCTX();
			List<soyagaci> soyList = new List<soyagaci>();
			var ebeveyns = usctx.soyagaciList("select * from soyagaci where hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId });
			soyList.AddRange(ebeveyns);
			foreach(var item in ebeveyns)
			{
				soyList.AddRange( usctx.soyagaciList("select * from soyagaci where ustHayvanId = @ustHayvanId and isActive = 1", new {ustHayvanId = item.ustHayvanId}));
			}
			soyList.AddRange(usctx.soyagaciList("select * from soyagaci where ustHayvanId = @ustHayvanId and isActive = 1", new { ustHayvanId = hayvanId }));
			List<miniTur> minTurList = new List<miniTur>();
			int flag = 0;
			foreach(var item in soyList)
			{
				miniTur mt = new miniTur();

				if (flag == 0)
					flag = 1;
	

				mt.id = item.id;
				mt.HayvanTur = item.hayvan.kategori.ustkategori.name;
				mt.AltTur = item.hayvan.kategori.kategoriAdi;
				mt.RFIDKodu = item.hayvan.rfidKodu;
				mt.Agirlik = item.hayvan.agirlik;
				mt.img = "https://cdn.balkan.app/shared/5.jpg";
				minTurList.Add(mt);

			}
			
			var userObj = System.Text.Json.JsonSerializer.Serialize(minTurList);
			var sey = Json(userObj);
			return sey;
		}
		public JsonResult kapiTetikle(int kapiId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);

				try
				{
					var client = new RestClient(profileObj.cihazLink + "/Secim?secenek=" + kapiId.ToString());
					client.Timeout = 1000;
					var request = new RestRequest(Method.GET);
					IRestResponse response = client.Execute(request);
					var cevap = response.Content;
					//var gelen = JsonConvert.DeserializeObject<string>(cevap);
					if (cevap == "")
					{
						return Json(new { status = "warning", message = "Boş Cevap" });
					}

					return Json(new { status = "success", message = "" });
				}
				catch (Exception ex)
				{
					return Json(new { status = "error", message = ex.ToString() });

				}

			}
			return Json("");
		}
		public JsonResult otomatiksurecIptal(string requestId)
		{
			surecCTX sctX = new surecCTX();
			var requestIdVarMi = sctX.surecTek("select * from surec where requestId = @requestId", new { requestId = requestId });
			if(requestId != null)
			{
				requestIdVarMi.tamamlandi = -1;
				requestIdVarMi.tamamlanmaTarihi = DateTime.Now;
				sctX.surecGuncelle(requestIdVarMi);
				_memoryCache.Set(requestIdVarMi.requestId, -1, new MemoryCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddMinutes(2),
					Priority = CacheItemPriority.Normal
				});
				return Json("");
			}
			return Json("");
		}
		public JsonResult otomatiksurec(string requestId)
		{
			if (requestId == null)
				return Json(new { status = "error", message = "Request Id Null" });
			int agirlikOlcumCounter = 0;
			int rfidOlcumCounter = 0;
			List<double> agirlikOlcumleri = new List<double>();
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
				surecCTX sctx = new surecCTX();
				var requestIdVarMi = sctx.surecTek("select * from surec where requestId = @requestId", new { requestId = requestId });
				if(requestIdVarMi != null)
				{
					return Json(new { status = "error", message = "Bu request Id zaten Var...." });
				}
				else
				{
					surec sc = new surec()
					{
						requestId = requestId,
						userId = userObj.id,
						tarih = DateTime.Now,
						tamamlandi = 0
					};
					sctx.surecEkle(sc);
					requestIdVarMi = sctx.surecTek("select * from surec where requestId = @requestId", new { requestId = requestId });
				}

				_memoryCache.Set(requestIdVarMi.requestId, 0, new MemoryCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddMinutes(2),
					Priority = CacheItemPriority.Normal
				});

				kupekontrol(requestId, userObj.id);
				agirlikOlcumKontrol(requestId, userObj.id);
				

				//tumKapilariKapa();
				//Giriş Kapısını Açıyoruz
				var cevap = webServisSorgu("/Secim?secenek=6");

				// 5 kg ve fazlası gelinceye kadar bekliyoruz....
				double olculenDeger = 0.00;
				while(olculenDeger < 2)
				{
					//if(agirlikOlcumCounter > 10)
					//{
					//	return Json(new { status = "error", message = "Ağırlık gelmedi" });
					//}
					int sonuc = Convert.ToInt32(_memoryCache.Get(requestId));
					if(sonuc == -1)
					{
						tumKapilariKapa();
						return Json(new { status = "cancel", message = "Süreç İptal Edildi" });
					}
					olculenDeger = agirlikOlcumOtomatik(requestId, userObj.id, olculenDeger);
					//Task.Delay(200).Wait();
				}
				//Giriş Kapısı Kapanıyor...
				cevap = webServisSorgu("/Secim?secenek=17");
				//Nihai Ağırlık Ölçümü
				olculenDeger = agirlikOlcumOtomatik(requestId, userObj.id, olculenDeger);
				//while(agirlikOlcumCounter < 3)
				//{
				//	olculenDeger = agirlikOlcumOtomatik(requestId, userObj.id, olculenDeger);
				//	if (olculenDeger > 5)
				//		agirlikOlcumleri.Add(olculenDeger);
				//	agirlikOlcumCounter++;
				//}
				//AgirlikOlcum eklenenId = agirlikOlcumKontrol(requestId, userObj.id);
				

				agirlikOlcumCounter = 0;
				

				string rfid = "";
				while(rfid == "")
				{
					int sonuc = Convert.ToInt32(_memoryCache.Get(requestId));
					if (sonuc == -1)
					{
						tumKapilariKapa();
						return Json(new { status = "cancel", message = "Süreç İptal Edildi" });
					}
					rfid = rfidOlcumOtomatik(requestId, userObj.id);
				}
				//rfid verisi geldi demek
				rfidOlcumCounter = 0;
				//Yönlenirme kapısı açılıyor...
				int kapanan = 18;
				cevap = webServisSorgu("/Secim?secenek=7");

				//if (olculenDeger >5 && olculenDeger < 20)
				//else if (olculenDeger > 19 && olculenDeger < 40)
				//{
				//	cevap = webServisSorgu("/Secim?secenek=8");
				//	kapanan = 19;
				//}
				//else if (olculenDeger > 39 && olculenDeger < 1000)
				//{
				//	cevap = webServisSorgu("/Secim?secenek=9");
				//	kapanan = 20;

				//}

				while (olculenDeger > 5)
				{
					int sonuc = Convert.ToInt32(_memoryCache.Get(requestId));
					if (sonuc == -1)
					{
						return Json(new { status = "cancel", message = "Süreç İptal Edildi" });
					}
					olculenDeger = agirlikOlcumOtomatik(requestId, userObj.id, olculenDeger);
					agirlikOlcumCounter++;
				}
				Task.Delay(100);

				cevap = webServisSorgu("/Secim?secenek="+kapanan);

				requestIdVarMi.tamamlandi = 1;
				requestIdVarMi.tamamlanmaTarihi = DateTime.Now;
				sctx.surecGuncelle(requestIdVarMi);
				_memoryCache.Set(requestIdVarMi.requestId, 1, new MemoryCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddMinutes(2),
					Priority = CacheItemPriority.Normal
				});
				return Json(new { status = "success", message = "Ölçüm Süreci Başarıyla Bitti" });

			}
			return Json(new { status = "error", message = "" });
		}
		public string rfidOlcumOtomatik(string requestId, int userId)
		{

			kupeatamaCTX hctx = new kupeatamaCTX();
			KupeAtama eklenenId = kupekontrol( requestId,  userId);
	
			if(eklenenId.kupeRfid == "")
			{
				var gelen = webServisSorgu("/RFIDApi");
				eklenenId.kupeRfid = gelen;
				hctx.kupeAtamaGuncelle(eklenenId);

				sureclogCTX slctx = new sureclogCTX();
				sureclog sl = new sureclog()
				{
					processId = 1,
					sorguSonucu = "RFID Ölçümü",
					sorguCevap = gelen,
					fonksiyonAdi = "kupekontrol"
				};
				slctx.sureclogEkle(sl);
				return gelen;

			}
			return eklenenId.kupeRfid;


		}

		public KupeAtama kupekontrol(string requestId, int userId)
		{
			kupeatamaCTX hctx = new kupeatamaCTX();
			KupeAtama eklenenId = null;
			try
			{
				eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
			}
			catch (Exception ex)
			{
				sureclogCTX slctx = new sureclogCTX();
				sureclog sl = new sureclog()
				{
					processId = 1,
					sorguSonucu = "Fonksiyon Hatası null döndü",
					sorguCevap = ex.ToString(),
					fonksiyonAdi = "kupekontrol"
				};
				slctx.sureclogEkle(sl);
				return null;

			}
			if (eklenenId == null)
			{
				eklenenId = new KupeAtama()
				{
					requestId = requestId,
					userId = userId,
					tarih = DateTime.Now,
					kupeRfid = ""
				};
				try
				{
					hctx.kupeAtamaEkle(eklenenId);

				}
				catch (Exception ex)
				{
					sureclogCTX slctx = new sureclogCTX();
					sureclog sl = new sureclog()
					{
						processId = 1,
						sorguSonucu = "Fonksiyon Hatası null dönmedi",
						sorguCevap = ex.ToString(),
						fonksiyonAdi = "kupekontrol"
					};
					slctx.sureclogEkle(sl);
				}
				eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
			}
			return eklenenId;
		}
		public double agirlikOlcumOtomatik(string requestId, int userId, double olculenDeger)
		{
			AgirlikOlcum eklenenId = agirlikOlcumKontrol(requestId, userId);
			AgirlikOlcumCTX hctx = new AgirlikOlcumCTX();
			double olcum;

			var gelenCevap = webServisSorgu("/AgirlikApi");
			if(gelenCevap != "-1")
			{
				if(Double.TryParse(gelenCevap, out olcum))
				{
					if(olcum > olculenDeger)
					{
						eklenenId.agirlikOlcumu = olcum.ToString();
						hctx.agirlikOlcumGuncelle(eklenenId);
					}
				
					return olcum;
				}
			}
			return -0.00;


		}

		public AgirlikOlcum agirlikOlcumKontrol(string requestId, int userId)
		{
			AgirlikOlcum eklenenId = null;
			AgirlikOlcumCTX hctx = new AgirlikOlcumCTX();
			try
			{
				//bu request id ile eklenmiş veri var mı kontrolü
				eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
			}
			catch (Exception ex)
			{
				sureclogCTX slctx = new sureclogCTX();
				sureclog sl = new sureclog()
				{
					processId = 1,
					sorguSonucu = "Fonksiyon Hatası null döndü",
					sorguCevap = ex.ToString(),
					fonksiyonAdi = "agirlikOlcumKontrol"
				};
				slctx.sureclogEkle(sl);
				return null;
			}

			if (eklenenId == null)
			{
				AgirlikOlcum agirlik = new AgirlikOlcum()
				{
					requestId = requestId,
					userId = userId,
					agirlikOlcumu = ""
				};
				try
				{
					hctx.agirlikOlcumEkle(agirlik);
				}
				catch (Exception ex)
				{
					sureclogCTX slctx = new sureclogCTX();
					sureclog sl = new sureclog()
					{
						processId = 1,
						sorguSonucu = "Fonksiyon Hatası null dönmedi süreç devam etti",
						sorguCevap = ex.ToString(),
						fonksiyonAdi = "agirlikOlcumKontrol"
					};
					slctx.sureclogEkle(sl);
				}
				//Veri eklendikten sonra eklenen veri tekrar elde ediliyor
				eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
			}
			return eklenenId;
		}
		public void tumKapilariKapa()
		{
			var cevap = webServisSorgu("/Secim?secenek=17");
			Task.Delay(200).Wait();
			cevap = webServisSorgu("/Secim?secenek=18");
			Task.Delay(200).Wait();
			cevap = webServisSorgu("/Secim?secenek=19");
			Thread.Sleep(200);
			cevap = webServisSorgu("/Secim?secenek=20");
			Thread.Sleep(200);
		}
		public string webServisSorgu(string fonksiyon)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				try
				{
					var userObj = JsonConvert.DeserializeObject<User>(user);
					var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
					var client = new RestClient(profileObj.cihazLink + fonksiyon);
					client.Timeout = 5000;
					var request = new RestRequest(Method.GET);
					IRestResponse response = client.Execute(request);
					if(response.IsSuccessful == false)
					{
						sureclogCTX sctx = new sureclogCTX();
						sureclog sl = new sureclog()
						{
							processId = 1,
							fonksiyonAdi = fonksiyon,
							sorguCevap = response.ErrorMessage.ToString(),
							sorguSonucu = "error"
						};
						sctx.sureclogEkle(sl);
					}
					var cevap = response.Content;
					//var gelen = JsonConvert.DeserializeObject<string>(cevap);
					return cevap;
				}
				catch
				{
					return "-1";
				}
				
			}
			return "-1";
				
		}

		public static double GetMedian(double[] sourceNumbers)
		{
			//Framework 2.0 version of this method. there is an easier way in F4        
			if (sourceNumbers == null || sourceNumbers.Length == 0)
				throw new System.Exception("Median of empty array not defined.");

			//make sure the list is sorted, but use a new array
			double[] sortedPNumbers = (double[])sourceNumbers.Clone();
			Array.Sort(sortedPNumbers);

			//get the median
			int size = sortedPNumbers.Length;
			int mid = size / 2;
			double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
			return median;
		}

		public JsonResult agirlikHayvanListesi(int hayvanId)
		{
			AgirlikHayvanCTX ahctx = new AgirlikHayvanCTX();
			var list = ahctx.agirlikHayvanList("select * from agirlikhayvan where hayvanId = @hayvanId order by tarih", new { hayvanId = hayvanId });
			if (list.Count == 0)
				return Json("");
			return Json(JsonConvert.SerializeObject(list));
		}
	}
}
