using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Guest
{
    [ApiController]
    [Route("api/v1/feedback")]
    public class GeneralFeedbackController : Controller
    {
        private readonly IFeedbackRepository repository;
        public GeneralFeedbackController(IFeedbackRepository _repository)
        {
            repository = _repository;
        }

       
        [HttpGet("get-all-hotel-feedback")]
        public IActionResult GetFeedbackByHotel([FromQuery] int hotelID)
        {
            var response = repository.GetAllFeedbackHotel(hotelID);
            return StatusCode(response.StatusCode, response);
        }


    }
}
