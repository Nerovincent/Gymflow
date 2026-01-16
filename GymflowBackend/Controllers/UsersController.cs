using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymFlow_Backend_Phase4E.Constants;

namespace GymFlow_Backend_Phase4E.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok("User profile");
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("promote")]
        public IActionResult Promote()
        {
            return Ok("User promoted");
        }
    }
}
