using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
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

        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
        }

        [HttpGet("get-all-voucher")]
        public IActionResult GetAllVouchers()
        {
               
               var response = _voucherRepository.GetAllVouchers();
               return StatusCode(response.StatusCode, response);
            
        }
    }
}
