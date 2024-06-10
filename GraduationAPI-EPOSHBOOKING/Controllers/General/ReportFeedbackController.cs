using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.General
{
    [ApiController]
    [Route("api/v1/reportFeedback")]
    public class ReportFeedbackController : Controller
    {
        private readonly IReportFeedbackRepository repository;
        public ReportFeedbackController(IReportFeedbackRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet("get-all-report")]
        public IActionResult GetAllReport()
        {
            var response = repository.GetAllReportFeedBack();
            return StatusCode(response.StatusCode,response);
        }

        [HttpPut("confirm-report")]
        public IActionResult ConfirmReport([FromQuery]int reportID)
        {
            var response = repository.ConfirmReport(reportID);
            return StatusCode(response.StatusCode,response);
        }

        [HttpPut("reject-report")]
        public IActionResult RejectReport([FromForm]int reportID, [FromForm]String ReasonReject) { 
            
            var response = repository.RejectReport(reportID, ReasonReject);
            return StatusCode(response.StatusCode,response);
        }

        [HttpPost("create-report")]
        public IActionResult CreateReportFeedback([FromForm]int feedbackId, [FromForm]String ReporterEmail, [FromForm]String ReasonReport)
        {
            var response = repository.CreateReportFeedback(feedbackId, ReporterEmail, ReporterEmail);
            return StatusCode(response.StatusCode,response);
        }
    }
}
