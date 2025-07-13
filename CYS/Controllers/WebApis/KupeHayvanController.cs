using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;

namespace CYS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KupeHayvanController : ControllerBase
	{
		private readonly kupehayvanCTX _kupehayvanCTX;

		public KupeHayvanController()
		{
			_kupehayvanCTX = new kupehayvanCTX();
		}

		// GET: api/KupeHayvan
		[HttpGet]
		public IActionResult GetAll()
		{
			var result = _kupehayvanCTX.kupehayvanList("SELECT * FROM kupehayvan", null);
			return Ok(result);
		}

		// GET: api/KupeHayvan/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var result = _kupehayvanCTX.kupehayvanTek("SELECT * FROM kupehayvan WHERE Id = @Id", new { Id = id });
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		// POST: api/KupeHayvan
		[HttpPost]
		public IActionResult Create([FromBody] kupehayvan kupehayvan)
		{
			if (kupehayvan == null)
			{
				return BadRequest();
			}
			_kupehayvanCTX.kupehayvanEkle(kupehayvan);
			return CreatedAtAction(nameof(GetById), new { id = kupehayvan.id }, kupehayvan);
		}

		// PUT: api/KupeHayvan/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] kupehayvan kupehayvan)
		{
			if (kupehayvan == null || kupehayvan.id != id)
			{
				return BadRequest();
			}

			var existingKupeHayvan = _kupehayvanCTX.kupehayvanTek("SELECT * FROM kupehayvan WHERE Id = @Id", new { Id = id });
			if (existingKupeHayvan == null)
			{
				return NotFound();
			}

			_kupehayvanCTX.kupehayvanGuncelle(kupehayvan);
			return NoContent();
		}

		// DELETE: api/KupeHayvan/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var existingKupeHayvan = _kupehayvanCTX.kupehayvanTek("SELECT * FROM kupehayvan WHERE Id = @Id", new { Id = id });
			if (existingKupeHayvan == null)
			{
				return NotFound();
			}

			// SİLME FONKSİYONU OLUŞTURULMADI, gerekirse eklenebilir
			return NoContent();
		}
	}
}
