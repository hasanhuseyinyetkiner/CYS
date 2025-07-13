using CYS.Models;
using CYS.Models.Mobil;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		// Login işlemi - modüller ve cihazlar döndürülüyor
		[HttpPost]
		public ActionResult<CYS.Models.Mobil.ApiResponse<object>> Post(string username, string password)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("SELECT * FROM user WHERE username = @username AND password = @password", new { username, password });

			if (userVarMi != null)
			{
				// Kullanıcı başarıyla bulundu, modülleri ve cihazları döndür
				UserModuleCTX userModuleCTX = new UserModuleCTX();
				DeviceCTX deviceCTX = new DeviceCTX();

				// Kullanıcının modüllerini çekiyoruz
				List<UserModule> userModules = userModuleCTX.GetUserModulesList("SELECT * FROM usermodule WHERE userid = @userid AND aktif = 1", new { userid = userVarMi.id });

				// Kullanıcının cihazlarını çekiyoruz
				List<UserDevice> userDevices = new UserDeviceCTX().GetUserDevicesList("SELECT * FROM userdevice WHERE userid = @userid AND active = 1", new { userid = userVarMi.id });

				// Başarılı login yanıtı: Kullanıcı, modüller ve cihazlar
				var response = new
				{
					User = userVarMi,
					Modules = userModules,
					Devices = userDevices
				};

				return Ok(CYS.Models.Mobil.ApiResponse<object>.SuccessResponse(response, "Giriş başarılı"));
			}

			// Başarısız giriş yanıtı
			return Unauthorized(CYS.Models.Mobil.ApiResponse<object>.ErrorResponse("Kullanıcı adı veya parola yanlış.", 401));
		}

		// Kullanıcı cihaz linkini güncelleme
		[HttpPut]
		public ActionResult<CYS.Models.Mobil.ApiResponse<string>> Update(int userId, string link)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("SELECT * FROM user WHERE id = @id", new { id = userId });

			if (userVarMi != null)
			{
				ProfileCTX pctx = new ProfileCTX();
				var mevcutProfil = pctx.profilTek("SELECT * FROM profile WHERE userId = @userId", new { userId = userId });
				mevcutProfil.cihazLink = link;
				pctx.profilGuncelle(mevcutProfil);

				return Ok(CYS.Models.Mobil.ApiResponse<string>.SuccessResponse(link, "Cihaz linki başarıyla güncellendi."));
			}

			return NotFound(CYS.Models.Mobil.ApiResponse<string>.ErrorResponse("Kullanıcı bulunamadı.", 404));
		}
	}
}
