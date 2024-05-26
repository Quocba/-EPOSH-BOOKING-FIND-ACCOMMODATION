using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [Route("api/v1/voucher")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfiguration configuration;
        private readonly Utils utils;

        public VoucherController(IVoucherRepository voucherRepository)
        {

            this._voucherRepository = voucherRepository;
        }

        [HttpGet("get-all-voucher")]
        public IActionResult GetAllVouchers()
        {

            var response = _voucherRepository.GetAllVouchers();
            return StatusCode(response.StatusCode, response);

        }
        [HttpGet("get-voucher-id")]
        public IActionResult GetVoucherById(int voucherId)
        {
            var response = _voucherRepository.GetVoucherById(voucherId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("get-voucher-by-account")]
        public IActionResult GetVouchersByAccountId([FromQuery] int accountId)
        {
            var response = _voucherRepository.GetVouchersByAccountId(accountId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("receive-voucher")]
        public IActionResult ReceviceVoucher([FromForm]int accountID, [FromForm]int voucherID)
        {
            var response = _voucherRepository.ReceiveVoucher(accountID, voucherID);
            return StatusCode(response.StatusCode, response);
        }
     


    }
}
