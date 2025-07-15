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
		// GEÇİCİ ÇÖZÜM: Veritabanına bağlanmadan mock verilerle giriş
		if ((username == "admin" && password == "123") || (username == "user" && password == "123"))
		{
			// Mock user verisi
			var mockUser = new CYS.Models.User
			{
				id = 1,
				userName = username,
				password = password,
				isActive = 1
			};

			// Mock profile verisi
			var mockProfile = new CYS.Models.Profile
			{
				id = 1,
				userId = 1,
				companyName = "Test Company",
				companyDescription = "Test Description",
				companyId = 1,
				address = "Test Address",
				phoneNumber = "0123456789",
				cellPhoneNumber = "0987654321",
				cihazLink = "http://localhost:8080"
			};

			var userJson = JsonConvert.SerializeObject(mockUser);
			var profileJson = JsonConvert.SerializeObject(mockProfile);

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
