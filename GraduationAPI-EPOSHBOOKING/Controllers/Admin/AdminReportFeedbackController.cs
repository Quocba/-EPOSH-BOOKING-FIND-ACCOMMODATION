using GraduationAPI_EPOSHBOOKING.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/reportFeedback")]
    public class AdminReportFeedbackController : Controller
    {
        private readonly IReportFeedbackRepository repository;
        public AdminReportFeedbackController(IReportFeedbackRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("get-all-report")]
        public IActionResult GetAllReport()
        {
            var response = repository.GetAllReportFeedBack();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("confirm-report")]
        public IActionResult ConfirmReport([FromQuery] int reportID)
        {
            var response = repository.ConfirmReport(reportID);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("reject-report")]
        public IActionResult RejectReport([FromForm] int reportID, [FromForm] String ReasonReject)
        {

            var response = repository.RejectReport(reportID, ReasonReject);
            return StatusCode(response.StatusCode, response);
        }
    }
}
