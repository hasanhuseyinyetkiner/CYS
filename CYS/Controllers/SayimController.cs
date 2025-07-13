using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.Controllers
{
	public class SayimController : Controller
	{

		public ActionResult Sayim()
		{
			olcumCTX olcumCTX = new olcumCTX();
			var tumOlcumler = olcumCTX.olcumList("select * from olcum order by id desc", null);
			return View(tumOlcumler);
		}

		public JsonResult SayimGonderJson()
		{
			olcumCTX olcumCTX = new olcumCTX();
			var tumOlcumler = olcumCTX.olcumList("select * from olcum order by id desc", null);
			var stringList = JsonConvert.SerializeObject(tumOlcumler);
			return Json(stringList);
		}

		public ActionResult Konum()
		{
			konumCTX konumCTX = new konumCTX();
			var last = konumCTX.konumTek("select * from konum order by id desc limit 1", null);
			if (last.lang[0] == -1)
				last.lang = last.lang.Trim('-');
			return View(last);
		}
		//	18	1	37.917162	-32.527688	2023-09-04 10:12:36
	}
}
