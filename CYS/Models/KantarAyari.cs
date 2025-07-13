using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Models
{
	[Route("api/[controller]")]
	[ApiController]
	public class KantarAyari : ControllerBase
	{
		public int id { get; set; }
		public string requestId { get; set; }
		public int tare3 { get; set; }
		public int tare4 { get; set; }
		public int tare5 { get; set; }
		public string esikagirlik { get; set; }
		public DateTime tarih { get; set; }
	}
}
