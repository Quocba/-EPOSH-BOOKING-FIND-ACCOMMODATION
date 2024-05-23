using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IAccountRepository repository;
        public AuthController(IAccountRepository repository)
        {
            this.repository = repository;
        }

        [HttpPut("partner-register")]
        public IActionResult PartnerRegister([FromForm]Account account, [FromForm]String fullName)
        {
            var response = repository.RegisterPartnerAccount(account, fullName);
            return StatusCode(response.StatusCode,response);
        }

        [HttpPut("active-account")]
        public IActionResult ActiveAccount([FromForm]String email) { 
            var response = repository.ActiveAccount(email);
            return StatusCode(response.StatusCode,response);
        }
    }
}
