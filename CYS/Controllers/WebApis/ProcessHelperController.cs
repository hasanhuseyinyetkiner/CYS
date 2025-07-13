using Microsoft.AspNetCore.Mvc;
using Dapper;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/[controller]")]
public class ProcessHelperController : ControllerBase
{
	private readonly ProcessHelper _helper;
	private readonly string _connectionString = "Server=localhost;Database=CYS;User Id=root;Password=Merlab.702642;";

	public ProcessHelperController()
	{
		_helper = new ProcessHelper();
	}

	[HttpPost("BaslaVeYeniSurecEkle")]
	public IActionResult BaslaVeYeniSurecEkle([FromBody] ProcessStartRequest request)
	{
		// Gelen verileri kullanarak süreci başlatıyoruz
		string yeniGuid = _helper.BaslaVeYeniSurecEkle(
			request.GirisMax,
			request.CikisMin,
			request.CihazId,
			request.Kapi1Agirlik,
			request.Kapi2Agirlik,
			request.Kapi3Agirlik,
			request.GirisSonrasiBekleme,
			request.giriskapikapandiktansonrakibekleme,
			request.CikisKapisiSonrasiBekleme,
			request.KupeOkumaSonrasiBekleme,
			request.SonAgirlikBekleme
		);

		// GUID'i geri döndürüyoruz
		return Ok(new { guid = yeniGuid });
	}



	// GetProcessDataByGuid fonksiyonu
	[HttpGet("GetProcessDataByGuid")]
	public IActionResult GetProcessDataByGuid([FromQuery] string guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			return BadRequest(new { message = "GUID belirtilmedi." });
		}

		using (var connection = new MySqlConnection(_connectionString))
		{
			// Sorgu: guid ile eşleşen veriyi getir
			string sorgu = "SELECT * FROM processmanagement WHERE guid = @guid";

			var result = connection.QueryFirstOrDefault(sorgu, new { guid });

			if (result == null)
			{
				return NotFound(new { message = "Veri bulunamadı." });
			}

			// Veriyi döndürüyoruz
			return Ok(new
			{
				id = result.id,
				guid = result.guid,
				hayvangirdi = result.hayvangirdi,
				ilkkapikapandi = result.ilkkapikapandi,
				kupeokundu = result.kupeokundu,
				okunankupe = result.okunankupe,
				sonagirlikalindimi = result.sonagirlikalindimi,
				sonagirlik = result.sonagirlik,
				minimumhassasiyetagirlik = result.minimumhassasiyetagirlik,
				cihazId = result.cihazId,
				isTamamlandi = result.isTamamlandi,
				mevcutmod = result.mevcutmod,
				hayvanid = result.hayvanid,

			});

		}
	}
}

// Request için bir model sınıfı oluşturuyoruz
public class ProcessStartRequest
{
	public float GirisMax { get; set; }
	public float CikisMin { get; set; }
	public int CihazId { get; set; }
	public float Kapi1Agirlik { get; set; }
	public float Kapi2Agirlik { get; set; }
	public float Kapi3Agirlik { get; set; }

	// Yeni eklenen alanlar
	public int GirisSonrasiBekleme { get; set; }
	public int giriskapikapandiktansonrakibekleme { get; set; }
	public int CikisKapisiSonrasiBekleme { get; set; }
	public int KupeOkumaSonrasiBekleme { get; set; }
	public int SonAgirlikBekleme { get; set; }
}


