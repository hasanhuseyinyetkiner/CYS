using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace CYS.Controllers
{
	public class TareController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult KantarAyari()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			return View();
		}

		public JsonResult tareBaslat(int islemModu = 51)
		{
			var guid = Guid.NewGuid();
			processCTX ppctx = new processCTX();
			process processx = new process()
			{
				processTypeId = 2,
				requestId = guid.ToString(),
				baslangicZamani = DateTime.Now,
				bitisZamani = DateTime.Now,
				tamamlandi = 0
			};
			ppctx.processEkle(processx);
			processsettingCTX settingCTX = new processsettingCTX();
			processsetting psetting = new processsetting()
			{
				islemModu = islemModu,
				mevcutIslem = 2,
				mevcutRequest = guid.ToString(),
				oncekiIslem = -1,
				guncellemeZamani = DateTime.Now

			};
			settingCTX.processtypeGuncelle(psetting);

			if(islemModu == 51)
			{
				var mevcutSetting = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
				KantarAyariRepo kaRepo = new KantarAyariRepo();
				var mevcutTare = kaRepo.KantarAyariTek("select * from kantarayari where requestId =@requestId", new { requestId = mevcutSetting.mevcutRequest });
				if(mevcutTare == null)
				{
					KantarAyari ka = new KantarAyari()
					{
						esikagirlik = "",
						requestId = mevcutSetting.mevcutRequest
					};
					kaRepo.KantarAyariEkle(ka);
				}

			}
			return Json(guid.ToString());

		
		}

		public JsonResult ModDegistir(int islemModu)
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutProcess = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
			mevcutProcess.islemModu = islemModu;
			settingCTX.processtypeGuncelle(mevcutProcess);
			return Json("1");
		}

		public JsonResult mevcutTare()
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutSetting = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
			KantarAyariRepo kaRepo = new KantarAyariRepo();
			var mevcutTare = kaRepo.KantarAyariTek("select * from kantarayari where requestId =@requestId", new { requestId = mevcutSetting.mevcutRequest });
			var deser = JsonConvert.SerializeObject(mevcutTare);
			return Json(deser);
		}

		public JsonResult agirlikGonder(string agirlik)
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutSetting = settingCTX.processsettingTek("select * from processsetting where id = 1", null);

			mevcutSetting.islemModu = 53;
			settingCTX.processtypeGuncelle(mevcutSetting);
			KantarAyariRepo kaRepo = new KantarAyariRepo();
			var mevcutTare = kaRepo.KantarAyariTek("select * from kantarayari where requestId =@requestId", new { requestId = mevcutSetting.mevcutRequest });
			mevcutTare.esikagirlik = agirlik+"|";
			kaRepo.KantarAyariGuncelle(mevcutTare);
			
			return Json("1");
		}

		public JsonResult enSonAgirlik()
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutSetting = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
			AgirlikOlcumCTX actx = new AgirlikOlcumCTX();
			var enSonOlcum = actx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId order by id desc limit 1", new { requestId = mevcutSetting.mevcutRequest });
			if (enSonOlcum == null)
				return Json("");
			return Json(enSonOlcum.agirlikOlcumu);
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

	}
}
