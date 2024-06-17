using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{
    [Route("api/v1/admin/voucher")]
    [ApiController]
    public class AdminVoucherController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;

        public AdminVoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [HttpPost("create-voucher")]
        public IActionResult CreateVoucher([FromForm] Voucher voucher, [FromForm] IFormFile image)
        {
            var response = _voucherRepository.CreateVoucher(voucher, image);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("delete-voucher")]
        public IActionResult DeleteVoucher([FromQuery] int voucherID)
        {
            var response = _voucherRepository.DeleteVoucher(voucherID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update-voucher")]
        public IActionResult UpdateVoucher([FromForm] int voucherID, [FromForm] Voucher voucher, [FromForm] IFormFile? image)
        {
            var response = _voucherRepository.UpdateVoucher(voucherID, voucher, image);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("search-name")]
        public IActionResult SearchVoucherName([FromQuery] String voucherName)
        {
            var response = _voucherRepository.SearchVoucherName(voucherName);
            return StatusCode(response.StatusCode, response);
        }
    }
}
