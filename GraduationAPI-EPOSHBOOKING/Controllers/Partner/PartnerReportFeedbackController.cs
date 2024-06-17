using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Partner
{
    [ApiController]
    [Route("api/v1/partner/reportFeedback")]
    public class PartnerReportFeedbackController : Controller
    {
        private readonly IReportFeedbackRepository repository;
        public PartnerReportFeedbackController(IReportFeedbackRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("create-report")]
        public IActionResult CreateReportFeedback([FromForm] int feedbackId, [FromForm] String ReporterEmail, [FromForm] String ReasonReport)
        {
            var response = repository.CreateReportFeedback(feedbackId, ReporterEmail, ReporterEmail);
            return StatusCode(response.StatusCode, response);
        }
    }
}
