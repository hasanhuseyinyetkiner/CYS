using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers
{
	public class UstKategoriController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ustKategoriListesi()
		{
			ustKategoriCTX ukctx = new ustKategoriCTX();
			var liste = ukctx.ustKategoriList("select * from ustkategori", null);
			return View(liste);
		}

		public JsonResult ustTurEkleJson(string ustKategoriAdi)
		{
			ustKategoriCTX ukctx = new ustKategoriCTX();
			Ustkategori uk = new Ustkategori()
			{
				name = ustKategoriAdi
			};
			ukctx.ustKategoriEkle(uk);
			return Json(new { status = "success", message = "Ekleme İşlemi Başarılı" });
		}

		public IActionResult kategoriListesi(int ustKategoriId)
		{
			KategoriCTX kctx = new KategoriCTX();
			var altKategoriListesi = kctx.KategoriList("select * from kategori where ustKategoriId = @ustKategoriId", new { ustKategoriId = ustKategoriId });
			ViewBag.ustKategoriId = ustKategoriId;
			return View(altKategoriListesi);

		}
		public JsonResult kategoriEkle(string resim, string kategoriAdi, string ustKategoriId)
		{
			KategoriCTX kctx = new KategoriCTX();
			Kategori kat = new Kategori()
			{
				kategoriAdi = kategoriAdi,
				ustKategoriId = Convert.ToInt32(ustKategoriId),
				resim = Convert.FromBase64String(resim)
			};
			kctx.KategoriEkle(kat);
			return Json(new { status = "success", message = "Ekleme işlemi başarılı" });
		}

		

	}
}