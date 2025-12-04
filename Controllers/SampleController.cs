using Microsoft.AspNetCore.Mvc;

namespace yeni.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Hello from SampleController!",
                timestamp = DateTime.UtcNow
            });
        }
    }
    
    
    
    
    
}