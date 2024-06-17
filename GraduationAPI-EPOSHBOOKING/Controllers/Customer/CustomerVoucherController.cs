using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [Route("api/v1/customer/voucher")]
    [ApiController]
    public class CustomerVoucherController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;

        public CustomerVoucherController(IVoucherRepository voucherRepository)
        {
            this._voucherRepository = voucherRepository;
        }

        [HttpGet("get-voucher-by-account")]
        public IActionResult GetVouchersByAccountId([FromQuery] int accountId)
        {
            var response = _voucherRepository.GetVouchersByAccountId(accountId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("receive-voucher")]
        public IActionResult ReceviceVoucher([FromForm] int accountID, [FromForm] int voucherID)
        {
            var response = _voucherRepository.ReceiveVoucher(accountID, voucherID);
            return StatusCode(response.StatusCode, response);
        }
    }
}
