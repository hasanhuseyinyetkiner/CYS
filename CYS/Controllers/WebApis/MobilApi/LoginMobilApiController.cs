using CYS.Models.Mobil;
using CYS.Models.Mobil.CYS.Models;
using CYS.Repos;
using CYS.Repos.MobilRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CYS.Controllers.WebApis
{
    [Route("api/[controller]")]
	[ApiController]
	public class LoginMobilApiController : ControllerBase
	{
		// Login işlemi
		[HttpPost]
		public ActionResult<ApiResponse<UserSession>> Post(string username, string password, string devicename, string ipaddress)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("select * from user where username = @username and password = @password", new { username, password });

			if (userVarMi != null)
			{
				// Kullanıcı bulundu, şimdi UserSession nesnesi oluşturuluyor
				UserSessionCTX sessionCTX = new UserSessionCTX();
				UserSession newUserSession = new UserSession
				{
					userid = userVarMi.id,
					devicename = devicename,
					deviceguid = Guid.NewGuid().ToString(), // Benzersiz cihaz kimliği
					devicekey = Guid.NewGuid().ToString(), // Benzersiz cihaz anahtarı
					sessiontimeout = DateTime.Now.AddHours(1), // Oturumun bitiş süresi (örnek 1 saat)
					ipaddress = ipaddress
				};

				// Veritabanına kaydet
				sessionCTX.userSessionEkle(newUserSession);

				// Başarılı login, ApiResponse ile UserSession nesnesini döndür
				return Ok(ApiResponse<UserSession>.SuccessResponse(newUserSession, "Giriş başarılı"));
			}

			return Unauthorized(ApiResponse<UserSession>.ErrorResponse("Kullanıcı adı veya parola yanlış", 401));
		}

		// Cihaz linki güncelleme işlemi
		[HttpPut]
		public IActionResult Update(int userId, string link)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("select * from user where id = @id", new { id = userId });

			if (userVarMi != null)
			{
				ProfileCTX pctx = new ProfileCTX();
				var mevcutProfil = pctx.profilTek("select * from profile where userId = @userId", new { userId = userId });
				mevcutProfil.cihazLink = link;
				pctx.profilGuncelle(mevcutProfil);

				return Ok(ApiResponse<string>.SuccessResponse(link, "Cihaz linki başarıyla güncellendi"));
			}

			return NotFound(ApiResponse<string>.ErrorResponse("Kullanıcı bulunamadı", 404));
		}
	}
}
