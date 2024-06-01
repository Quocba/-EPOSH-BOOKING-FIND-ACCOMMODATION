using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Ultils;
using GraduationAPI_EPOSHBOOKING.Model;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
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
            repository = _repository;
        }

        [HttpGet("get-profile-by-account")]
        public IActionResult GetProfileByAccountId([FromQuery] int accountId)
        {
            var response = repository.GetProfileByAccountId(accountId);

            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("get-all")]
        public IActionResult GetAllAccount()
        {
            var response = repository.GetAllAccount();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("blocked-account")]
        public IActionResult BlockedAccount([FromQuery] int accountId)
        {
            var response = repository.BlockedAccount(accountId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("filter-account")]
        public IActionResult FilterAccountByStatus([FromQuery] bool isActive)
        {
            var response = repository.FilterAccountByStatus(isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("searchByName")]
        public IActionResult SearchByName([FromQuery] string name)
        {
            var response = repository.SearchAccountByName(name);
            return StatusCode(response.StatusCode, response);
        }
    }
}
