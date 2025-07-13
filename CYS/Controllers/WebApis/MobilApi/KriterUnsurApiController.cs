using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using System.Collections.Generic;
using System.Linq;

namespace CYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KriterUnsurApi : ControllerBase
    {
        private readonly KriterCTX _kriterCtx;
        private readonly KriterUnsurCTX _kriterUnsurCtx;

        public KriterUnsurApi()
        {
            _kriterCtx = new KriterCTX();
            _kriterUnsurCtx = new KriterUnsurCTX();
        }

        // GET: api/Kriter/{id}
        [HttpGet("{id}")]
        public ActionResult<Kriter> GetKriterWithUnsur(int id)
        {
            var kriter = _kriterCtx.kriterTek("SELECT * FROM kriter WHERE id = @Id AND isActive = 1", new { Id = id });
            if (kriter == null)
            {
                return NotFound();
            }

            kriter.unsurlar = _kriterUnsurCtx.kriterUnsurList("SELECT * FROM kriterunsur WHERE kriterId = @KriterId AND isActive = 1", new { KriterId = id }).ToList();
            return kriter;
        }

        // GET: api/Kriter
        [HttpGet]
        public ActionResult<IEnumerable<Kriter>> GetAllKriterWithUnsur()
        {
            var kriterler = _kriterCtx.kriterList("SELECT * FROM kriter WHERE isActive = 1", new { });
            foreach (var kriter in kriterler)
            {
                kriter.unsurlar = _kriterUnsurCtx.kriterUnsurList("SELECT * FROM kriterunsur WHERE kriterId = @KriterId AND isActive = 1", new { KriterId = kriter.id }).ToList();
            }
            return kriterler;
        }
    }
}
