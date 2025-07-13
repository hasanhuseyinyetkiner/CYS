using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class TareApiController : ControllerBase
	{
		[HttpGet]
		public string Get(int islemModu)
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutps = settingCTX.processsettingTek("select * from processsetting where id = 1", null);

			KantarAyariRepo kaCTX = new KantarAyariRepo();
			var mevcutKantar = kaCTX.KantarAyariTek("select * from kantarayari where requestId = @requestId", new { requestId = mevcutps.mevcutRequest });
			if (islemModu == 51)
			{
				mevcutKantar.requestId = mevcutps.mevcutRequest;
				mevcutKantar.tare3 =1;
				mevcutKantar.tarih = DateTime.Now;
				
				kaCTX.KantarAyariGuncelle(mevcutKantar);
			}
			else if (islemModu == 53)
			{
				if (mevcutKantar != null)
				{
					mevcutKantar.tare5 = 1;
					kaCTX.KantarAyariGuncelle(mevcutKantar);
				}
			}

			else if (islemModu == 52)
			{

				if (mevcutKantar != null)
				{
					mevcutKantar.tare4 = 1;
					kaCTX.KantarAyariGuncelle(mevcutKantar);
				}
				
			}
			else if (islemModu == 53)
			{

				if (mevcutKantar != null)
				{
					mevcutKantar.tare5 = 1;
					kaCTX.KantarAyariGuncelle(mevcutKantar);
				}

			}
			else if(islemModu == 99)
			{

				return mevcutKantar.esikagirlik;
			}


			return "1";
		}
	}
}
