using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;

namespace CYS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SoyAgaciController : ControllerBase
	{
		private readonly UstSoyCTX _ustSoyCTX;

		public SoyAgaciController()
		{
			_ustSoyCTX = new UstSoyCTX();
		}

		// GET: api/SoyAgaci
		[HttpGet]
		public IActionResult GetAll()
		{
			var result = _ustSoyCTX.soyagaciList("SELECT * FROM soyagaci", null);
			return Ok(result);
		}

		// GET: api/SoyAgaci/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var result = _ustSoyCTX.soyagaciTek("SELECT * FROM soyagaci WHERE Id = @Id", new { Id = id });
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		// POST: api/SoyAgaci
		[HttpPost]
		public IActionResult Create([FromBody] soyagaci soyagaci)
		{
			if (soyagaci == null)
			{
				return BadRequest();
			}
			var inserted = _ustSoyCTX.soyagaciEkle(soyagaci);
			if (inserted > 0)
			{
				return CreatedAtAction(nameof(GetById), new { id = soyagaci.id }, soyagaci);
			}
			return StatusCode(500, "Soy ağacı eklenirken bir hata oluştu.");
		}

		// PUT: api/SoyAgaci/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] soyagaci soyagaci)
		{
			if (soyagaci == null || soyagaci.id != id)
			{
				return BadRequest();
			}

			var existingSoyagaci = _ustSoyCTX.soyagaciTek("SELECT * FROM soyagaci WHERE Id = @Id", new { Id = id });
			if (existingSoyagaci == null)
			{
				return NotFound();
			}

			var updated = _ustSoyCTX.soyagaciGuncelle(soyagaci);
			if (updated > 0)
			{
				return NoContent();
			}

			return StatusCode(500, "Soy ağacı güncellenirken bir hata oluştu.");
		}

		// DELETE işlemi istenirse, DELETE endpoint eklenebilir
	}
}
