using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using GraduationAPI_EPOSHBOOKING.Model;
#pragma warning disable // tắt cảnh báo để code sạch hơn

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

        [HttpGet("get-profile-by-account")]
        public IActionResult GetProfileByAccountId([FromQuery]int accountId)
        {
            var response = repository.GetProfileByAccountId(accountId);

            return StatusCode(response.StatusCode, response);
        }

    }
}
