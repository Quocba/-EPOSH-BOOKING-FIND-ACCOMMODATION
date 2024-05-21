using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
        }

        [HttpGet]
        public IActionResult GetAllVouchers()
        {
            try
            {
                var response = _voucherRepository.GetAllVouchers();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi nếu cần
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    Success = false,
                    Message = "Internal Server Error",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
