using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;

namespace CYS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KupeAtamaController : ControllerBase
	{
		private readonly kupeatamaCTX _kupeatamaCTX;

		public KupeAtamaController()
		{
			_kupeatamaCTX = new kupeatamaCTX();
		}

		// GET: api/KupeAtama
		[HttpGet]
		public IActionResult GetAll()
		{
			var result = _kupeatamaCTX.kupeAtamaOlcumList("SELECT * FROM kupeatama", null);
			return Ok(result);
		}

		// GET: api/KupeAtama/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var result = _kupeatamaCTX.kupeAtamaTek("SELECT * FROM kupeatama WHERE Id = @Id", new { Id = id });
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		// POST: api/KupeAtama
		[HttpPost]
		public IActionResult Create([FromBody] KupeAtama kupeAtama)
		{
			if (kupeAtama == null)
			{
				return BadRequest();
			}
			_kupeatamaCTX.kupeAtamaEkle(kupeAtama);
			return CreatedAtAction(nameof(GetById), new { id = kupeAtama.id }, kupeAtama);
		}

		// PUT: api/KupeAtama/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] KupeAtama kupeAtama)
		{
			if (kupeAtama == null || kupeAtama.id != id)
			{
				return BadRequest();
			}

			var existingKupeAtama = _kupeatamaCTX.kupeAtamaTek("SELECT * FROM kupeatama WHERE Id = @Id", new { Id = id });
			if (existingKupeAtama == null)
			{
				return NotFound();
			}

			_kupeatamaCTX.kupeAtamaGuncelle(kupeAtama);
			return NoContent();
		}

		// DELETE: api/KupeAtama/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existingKupeAtama = _kupeatamaCTX.kupeAtamaTek("SELECT * FROM kupeatama WHERE Id = @Id", new { Id = id });
			if (existingKupeAtama == null)
			{
				return NotFound();
			}

			// SİLME FONKSİYONU OLUŞTURULMADI, gerekirse eklenebilir
			return NoContent();
		}
	}
}
