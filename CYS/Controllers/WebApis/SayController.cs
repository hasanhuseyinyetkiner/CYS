using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class SayController : ControllerBase
	{
		[HttpGet]
		public string Get(string guid)
		{
			olcumCTX olcum = new olcumCTX();
			olcumSessionCTX olcumSession = new olcumSessionCTX();
			var mevcut = olcumSession.olcumTek("select * from olcumsession where sessionGuid = @guid", new { guid = guid});
			if(mevcut != null)
			{
				var varmi = olcum.olcumTek("select * from olcum where olcumSessionId = @olcumSessionId", new { olcumSessionId = mevcut.id });
				if(varmi != null)
				{
					varmi.adet = varmi.adet + 1;
					varmi.sonGuncelleme = DateTime.Now;
					olcum.update(varmi);
					
					return varmi.adet.ToString();
				}
				else
				{
					olcum olc = new olcum();
					olc.adet = 1;
					olc.olcumSessionId = mevcut.id;
					olc.sonGuncelleme = DateTime.Now;
					olcum.insert(olc);
					return "1";
				}

			}
			return "";


		}
	}
}
