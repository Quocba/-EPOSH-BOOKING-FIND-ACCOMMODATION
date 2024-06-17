using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/account")]
    public class AdminAccountController : Controller
    {
        private readonly IAccountRepository repository;
        private readonly IConfiguration configuration;
        public AdminAccountController(IAccountRepository _repository, IConfiguration configuration)
        {
            repository = _repository;
            this.configuration = configuration;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllAccount()
        {
            var response = repository.GetAllAccount();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("blocked-account")]
        public IActionResult BlockedAccount([FromForm] int accountId, [FromForm] String reasonBlock)
        {
            var response = repository.BlockedAccount(accountId, reasonBlock);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("filter-account")]
        public IActionResult FilterAccountByStatus([FromQuery] bool isActive)
        {
            var response = repository.FilterAccountByStatus(isActive);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("searchByName")]
        public IActionResult SearchAccountByName([FromQuery] string name)
        {
            var response = repository.SearchAccountByName(name);
            return StatusCode(response.StatusCode, response);
        }

    }
}
