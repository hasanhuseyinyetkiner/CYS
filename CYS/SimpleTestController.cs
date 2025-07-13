using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] dynamic data)
        {
            return Ok(new { success = true, message = "Test başarılı", receivedData = data });
        }
    }
}