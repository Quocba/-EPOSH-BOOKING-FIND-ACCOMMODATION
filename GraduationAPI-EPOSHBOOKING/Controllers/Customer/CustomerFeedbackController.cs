using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/customer/feedback")]
    public class CustomerFeedbackController : Controller
    {
        private readonly IFeedbackRepository repository;
        
        public CustomerFeedbackController(IFeedbackRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("create-feedback")]
        public IActionResult CreateFeedBack([FromForm] int BookingID, [FromForm] FeedBack newFeedBack, [FromForm] IFormFile Image)
        {
            var response = repository.CreateFeedBack(BookingID, newFeedBack, Image);
            return StatusCode(response.StatusCode, response);
        }
    }
}
