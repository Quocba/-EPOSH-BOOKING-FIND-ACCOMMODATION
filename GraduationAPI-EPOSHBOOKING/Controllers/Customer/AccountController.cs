using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Ultils;

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
        [HttpGet("Get-Voucher-by-Account-Id/{accountId}")]
        public IActionResult GetVouchersByAccountId(int accountId)
        {
            var response = repository.GetVouchersByAccountId(accountId);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("Get-Profile-by-Account-Id/{accountId}")]
        public IActionResult GetProfileByAccountId(int accountId)
        {
            var response = repository.GetProfileByAccountId(accountId);
            return StatusCode(response.StatusCode, response);
        }

    }
}
