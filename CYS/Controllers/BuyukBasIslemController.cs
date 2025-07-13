using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace CYS.Controllers
{
	public class BuyukBasIslemController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public JsonResult iptalEt()
		{
			processsettingCTX processsettingCTX = new processsettingCTX();
			var mevcut = processsettingCTX.processsettingTek("select * from processsetting where id = 1", null);
			mevcut.islemModu = 121;
			processsettingCTX.processtypeGuncelle(mevcut);

			Thread.Sleep(1000);
			mevcut.islemModu = -1;
			mevcut.mevcutRequest = "";
			processsettingCTX.processtypeGuncelle(mevcut);

			return Json("1");
		}

		public JsonResult olcum(string requestId)
		{
			agirliksuCTX agirliksuCTX = new agirliksuCTX();
			processsettingCTX processsettingCTX = new processsettingCTX();
			var mevcutp = processsettingCTX.processsettingTek("select * from processsetting where id = 1", null);
			var mevcut = agirliksuCTX.agirliksuTek("select * from agirliksu where reqestId = @requestId limit 1", new {requestId = mevcutp.mevcutRequest});
			if (mevcut == null)
				return Json("");
			else
			{
				var gonderilecek = JsonConvert.SerializeObject(mevcut);
				return Json(gonderilecek);
			}
		}
	}
}
