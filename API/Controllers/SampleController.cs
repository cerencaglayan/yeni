using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace yeni.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        [Authorize]
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