using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPhoneRadio.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomingCallController : ControllerBase
    {
        private readonly ILogger<IncomingCallController> _logger;

        public IncomingCallController(ILogger<IncomingCallController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/helloworld")]
        [AllowAnonymous]
        public IActionResult hellowowrld()
        {
            return Ok();
        }
    }
}