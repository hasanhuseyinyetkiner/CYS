using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class CihazController : ControllerBase
	{
		[HttpGet]
		public int Get()
		{
			processsettingCTX processsettingCTX = new processsettingCTX();
			var mevcutSetting = processsettingCTX.processsettingTek("select * from processsetting where id = 1", null);
			return mevcutSetting.islemModu;
		}
	}
}
