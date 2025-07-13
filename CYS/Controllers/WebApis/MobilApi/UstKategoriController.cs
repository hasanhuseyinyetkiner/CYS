using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using System.Collections.Generic;

namespace CYS.Controllers.WebApis.MobilApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class UstKategoriMobilController : ControllerBase
    {
        private readonly ustKategoriCTX _ustKategoriCTX;
        private readonly KategoriCTX _kategoriCTX;

        public UstKategoriMobilController()
        {
            _ustKategoriCTX = new ustKategoriCTX();
            _kategoriCTX = new KategoriCTX();
        }

        // GET: api/UstKategori
        [HttpGet]
        public ActionResult<IEnumerable<Ustkategori>> GetUstKategoriler()
        {
            var ustKategoriler = _ustKategoriCTX.ustKategoriList("SELECT * FROM ustkategori", new { });

            foreach (var ustKategori in ustKategoriler)
            {
                // İlişkili kategorileri çekmek için KategoriCTX kullanılıyor
                var kategoriler = _kategoriCTX.KategoriList("SELECT * FROM kategori WHERE ustKategoriId = @UstKategoriId", new { UstKategoriId = ustKategori.id });
                ustKategori.kategoriler = kategoriler;
            }

            return Ok(ustKategoriler);
        }

        // Diğer HTTP metodları buraya eklenebilir
    }
}
