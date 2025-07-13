using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers
{
	public class KriterController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult KriterListesi()
		{
			KriterCTX kctx = new KriterCTX();
			var liste = kctx.kriterList("select * from kriter", null);
			return View(liste);
		}

		public IActionResult kriterEkleJson(string kriterAdi)
		{
			KriterCTX kctx = new KriterCTX();
			Kriter kriter = new Kriter() { kriterAdi = kriterAdi };
			kctx.KriterEkle(kriter);
			return Json(new { status = "success", message = "Ekleme işlemi başarılı" });
		}

		public IActionResult kriterUnsurEkle(int kriterId)
		{
			KriterUnsurCTX kuctx = new KriterUnsurCTX();
			var kriterUnsurList = kuctx.kriterUnsurList("select * from kriterunsur where kriterId = @kriterId", new { kriterId = kriterId });
			ViewBag.kriterId = kriterId;
			return View(kriterUnsurList);
		}

		public IActionResult kriterUnsurEkleJson(string kunName, int kriterId)
		{
			KriterUnsurCTX kctx = new KriterUnsurCTX();
			KriterUnsur kriter = new KriterUnsur() {  unsurAdi= kunName, kriterId = kriterId };
			kctx.KriterUnsurEkle(kriter);
			return Json(new { status = "success", message = "Ekleme işlemi başarılı" });
		}
	}
}
