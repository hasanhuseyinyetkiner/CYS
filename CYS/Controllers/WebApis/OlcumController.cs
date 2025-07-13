using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class OlcumController : ControllerBase
    {
		[HttpGet]
		public string Get(int last = 0)
		{
			olcumSessionCTX olcumSession = new olcumSessionCTX();
			if (last == 0)
			{
				
				Guid sessionId = Guid.NewGuid();
				olcumSession os = new olcumSession()
				{
					sessionGuid = sessionId.ToString(),
					tarih = DateTime.Now
				};
				olcumSession.insert(os);
				return sessionId.ToString();
			}
			else
			{
				var enson = olcumSession.olcumTek("select * from olcumsession order by desc limit 1", null);
				return enson.sessionGuid;
			}
			

		}
	}
}
