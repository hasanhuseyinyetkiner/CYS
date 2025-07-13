using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CYS.Controllers.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class RFIDApiController : ControllerBase
    {
        // GET: api/<RFIDApiController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

		// GET api/<RFIDApiController>/5
		[HttpGet]
		public IEnumerable<string> Get()
        {
            return new string[] { "fefefeefefebbaaaccc", "value2" };
		}

        [HttpPost]
        public void Post(string rfid)
        {
            kupeatamaCTX kupeatama = new kupeatamaCTX();
            KupeAtama kam = new KupeAtama()
            {
                kupeRfid = rfid,
                userId = 1,
                tarih = DateTime.Now
            };
            kupeatama.kupeAtamaEkle(kam);
        }


		// PUT api/<RFIDApiController>/5
		[HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RFIDApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
