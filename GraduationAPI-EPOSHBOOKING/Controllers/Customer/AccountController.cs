using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using GraduationAPI_EPOSHBOOKING.Model;

namespace EPOSH_BOOKING.Controllers
{
    [ApiController]
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository repository;
        private readonly IConfiguration configuration;
        private readonly Utils utils;

        public AccountController(IAccountRepository _repository, IConfiguration configuration, Utils utils)
        {
            this.configuration = configuration;
            this.utils = utils;
            this.repository = _repository;
        }
        [HttpPost("send-mail")]
        public IActionResult SendOTPForgot([FromBody]String email)
        {
             var response = Utils.sendMail(email);
             return Ok(response);
        }

        [HttpPut("update-new-password")]
        public IActionResult UpdateNewPassword([FromForm]String newPassword, [FromForm]String email)
        {
            var response = repository.UpdateNewPassword(email, newPassword);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-profile")]
        public IActionResult UpdateProfile([FromForm]int accountID, [FromForm]Profile profile, [FromForm]IFormFile Avatar)
        {
            var response = repository.UpdateProfileByAccount(accountID, profile, Avatar);
            return StatusCode(response.StatusCode, response);
        }

    }
}
