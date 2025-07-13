using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using Newtonsoft.Json;

namespace CYS.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GirisYap()
		{
			return View();
		}

		public IActionResult LogOff()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("GirisYap","Login");
		}

		public JsonResult girisKontrolJson(string username, string password)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("select * from user where username = @username and password = @password", new {username, password});
			if(userVarMi != null)
			{
				ProfileCTX profileCTX = new ProfileCTX();
				var profilCek = profileCTX.profilTek("select * from profile where userId = @userId", new { userId = userVarMi.id });
				profilCek.cihazLink = ReplaceFirst(profilCek.cihazLink, "tcp", "http");
				var userJson = JsonConvert.SerializeObject(userVarMi);
				var profileJson = JsonConvert.SerializeObject(profilCek);

				HttpContext.Session.SetString("user", userJson);
				HttpContext.Session.SetString("profile", profileJson);

				return Json(new { status = "Success", message = "Giriş Başarılı Yönleniyorsunuz" });
			}
			return Json(new { status = "Error", message = "Kullanıcı Adı ya da Parola Yanlış" });

		}

		public string ReplaceFirst(string text, string search, string replace)
		{
			int pos = text.IndexOf(search);
			if (pos < 0)
			{
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}

	}
}
