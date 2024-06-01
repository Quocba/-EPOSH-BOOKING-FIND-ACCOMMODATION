using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [ApiController]
    [Route("api/v1/feedback")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackRepository repository;
        public FeedbackController(IFeedbackRepository _repository)
        {
            repository = _repository;
        }

        [HttpPost("create-feedback")]
        public IActionResult CreateFeedBack([FromForm] int BookingID, [FromForm] FeedBack newFeedBack, [FromForm] IFormFile Image)
        {
            var response = repository.CreateFeedBack(BookingID, newFeedBack, Image);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("get-all-feedback")]
        public IActionResult GetFeedbackByHotel([FromQuery] int hotelID)
        {
            var response = repository.GetAllFeedbackHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }


    }
}
