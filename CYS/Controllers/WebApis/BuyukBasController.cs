using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class BuyukBasController : ControllerBase
	{
		[HttpGet]
		public string Get()
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
				islemModu = 75,
				mevcutIslem = 2,
				mevcutRequest = guid.ToString(),
				oncekiIslem = -1,
				guncellemeZamani = DateTime.Now

			};
			settingCTX.processtypeGuncelle(psetting);

			return guid.ToString();
		}



		
	}
}
