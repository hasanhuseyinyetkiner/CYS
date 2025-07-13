using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class SurecApiController : ControllerBase
	{
		[HttpGet]
		public String surecBaslat(int userId)
		{
			surecCTX sctx = new surecCTX();
			Guid id = Guid.NewGuid();
			return id.ToString();

		}

		[HttpPut]
		public int surecBitir(int userId, string requestId, int iptal)
		{
			surecCTX sctx = new surecCTX();
			Guid id = Guid.NewGuid();
			var idVarMi = sctx.surecTek("select * from surec where requestId = @requestId", new { requestId = id.ToString() });
			if (idVarMi != null)
			{
				idVarMi.tamamlandi = iptal;
				idVarMi.tamamlanmaTarihi = DateTime.Now;
				var mevcut = sctx.surecGuncelle(idVarMi);
				return 1;

			}
			return 0;

		}
	}
}
