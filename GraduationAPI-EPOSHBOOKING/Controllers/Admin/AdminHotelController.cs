using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{

    [ApiController]
    [Route("api/v1/admin/hotel")]
    public class AdminHotelController : Controller
    {
        private readonly IHotelRepository repository;
        public AdminHotelController(IHotelRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("get-all-hotel-infomation")]
        public IActionResult GetAllHotelInfomation()
        {
            var response = repository.GetAllHotelInfomation();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("blocked-hotel")]
        public IActionResult BlockedHotel([FromForm] int hotelId,[FromForm] String reaseonBlock)
        {
            var response = repository.BlockedHotel(hotelId, reaseonBlock);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("confirm-registration")]
        public IActionResult ConfirmRegistration([FromForm] int hotelID)
        {
            var response = repository.ConfirmRegistration(hotelID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("reject-registration")]
        public IActionResult RejectRegistration([FromForm] int hotelID, [FromForm] String reasonReject)
        {
            var response = repository.RejectRegistration(hotelID, reasonReject);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("searchByName")]
        public IActionResult SearchByName([FromQuery] string hotelName)
        {
            var response = repository.SearchHotelByName(hotelName);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-all-registration-form")]
        public IActionResult GetAllHotelWaitForApproval()
        {
            var response = repository.GetAllHotelWaitForApproval();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("analyze-hotelStander")]
        public IActionResult AnalyzeHotelStandar()
        {
            var response = repository.AnalyzeHotelStandar();
            return StatusCode(response.StatusCode, response);
        }

    }
}
