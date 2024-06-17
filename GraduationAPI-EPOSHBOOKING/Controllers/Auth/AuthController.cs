﻿using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Repository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable // tắt cảnh báo để code sạch hơn

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

        [HttpPost("partner-register")]
        public IActionResult RegisterPartnerAccount([FromForm]Account account, [FromForm]String fullName)
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

        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = repository.ChangePassword(request.AccountId, request.OldPassword, request.NewPassword);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("send-mail")]
        public IActionResult SendOTPForgot([FromBody] String email)
        {
            var response = Utils.sendMail(email);
            return Ok(response);
        }

        [HttpPut("update-new-password")]
        public IActionResult UpdateNewPassword([FromForm] String newPassword, [FromForm] String email)
        {
            var response = repository.UpdateNewPassword(email, newPassword);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-profile")]
        public IActionResult UpdateProfileByAccount([FromForm] int accountID, [FromForm] Profile profile, [FromForm] IFormFile Avatar)
        {
            var response = repository.UpdateProfileByAccount(accountID, profile, Avatar);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDTO login)
        {
            var response = repository.Login(login.Email, login.Password);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("get-time-server")]
        public IActionResult GetTime()
        {
            var time = DateTime.Now;
            return Ok(time);
        }

        [HttpGet("get-profile-by-account")]
        public IActionResult GetProfileByAccountId([FromQuery] int accountId)
        {
            var response = repository.GetProfileByAccountId(accountId);

            return StatusCode(response.StatusCode, response);
        }
    }
}
