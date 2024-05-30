using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Customer
{
    [ApiController]
    [Route("api/v1/feedback")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackRepository repository;
        public FeedbackController(IFeedbackRepository _repository)
        {
            this.repository = _repository;
        }

        [HttpPost("create-feedback")]
        public IActionResult CreateFeedBack([FromForm]int BookingID, [FromForm]FeedBack newFeedBack, [FromForm]IFormFile Image)
        {
            var response = repository.CreateFeedBack(BookingID,newFeedBack, Image);
            return StatusCode(response.StatusCode, response);
        }
        // viết controller gửi report feedback
        [HttpPost("report-feedback")]
        public IActionResult ReportFeedback([FromForm] int AccountID, [FromForm] int FeedBackID, [FromForm] string Reason)
        {
            var response = repository.ReportFeedback(AccountID, FeedBackID, Reason);
            return StatusCode(response.StatusCode, response);
        }

    }
}
