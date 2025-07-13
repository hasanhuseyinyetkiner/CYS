using CYS.Models;
using CYS.Models.HelperModel;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CYS.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			if(sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			return RedirectToAction("MobilHayvanYonetimi", "MobilHayvan");
		}

		public IActionResult Ayarlar()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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