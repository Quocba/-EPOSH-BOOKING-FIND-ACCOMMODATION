using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {

        public class RegisterDTO
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
        }
        public class ChangePasswordRequest
        {
            public int AccountId { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        private readonly IAccountRepository repository;
        public AuthController(IAccountRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("partner-register")]
        public IActionResult PartnerRegister([FromForm]Account account, [FromForm]String fullName)
        {
            var response = repository.RegisterPartnerAccount(account, fullName);
            return StatusCode(response.StatusCode,response);    
        }

        [HttpPost("login-phone")]
        public IActionResult LoginPhone([FromBody]String phone)
        {
            var response = repository.LoginWithNumberPhone(phone);
            return StatusCode(response.StatusCode,response);
        }

        [HttpPut("active-account")]
        public IActionResult ActiveAccount([FromForm]String email) { 
            var response = repository.ActiveAccount(email);
            return StatusCode(response.StatusCode,response);
        }



        [HttpPost("register-customer")]
        public IActionResult Register([FromBody] RegisterDTO register)
        {
            var response = repository.Register(register.Email, register.Password, register.FullName, register.Phone);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = repository.ChangePassword(request.AccountId, request.OldPassword, request.NewPassword);
            return StatusCode(response.StatusCode, response);
        }
    }
}
