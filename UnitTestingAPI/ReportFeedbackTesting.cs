using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Partner;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;

namespace UnitTestingAPI
{
    [TestFixture]
    public class ReportFeedbackTesting
    {
        private PartnerReportFeedbackController partnerReportController;
        private AdminReportFeedbackController adminReportController;
        private Mock<IReportFeedbackRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IReportFeedbackRepository>();
            partnerReportController = new PartnerReportFeedbackController(_mockRepository.Object);
            adminReportController = new AdminReportFeedbackController(_mockRepository.Object);
        }

        [Test]
        public void CreateReportFeedback_ReturnsOk()
        {
            int feedbackId = 1;
            string reporterEmail = "reporter@example.com";
            string reasonReport = "Inappropriate content";
            var newReport = new ReportFeedBack
            {
                FeedBack = new FeedBack { FeedBackID = feedbackId },
                ReporterEmail = reporterEmail,
                ReasonReport = reasonReport,
                Status = "Awaiting Approval"
            };

            _mockRepository.Setup(repo => repo.CreateReportFeedback(feedbackId, reporterEmail, reasonReport))
                .Returns(new ResponseMessage { Success = true, Data = newReport, Message = "Report Successfully", StatusCode = (int)HttpStatusCode.OK });

            var result = partnerReportController.CreateReportFeedback(feedbackId, reporterEmail, reasonReport) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Report Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(newReport));
        }

        [Test]
        public void CreateReportFeedback_FeedbackNotFound_ReturnsNotFound()
        {
            int feedbackId = 999;
            string reporterEmail = "reporter@example.com";
            string reasonReport = "Inappropriate content";

            _mockRepository.Setup(repo => repo.CreateReportFeedback(feedbackId, reporterEmail, reasonReport))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            var result = partnerReportController.CreateReportFeedback(feedbackId, reporterEmail, reasonReport) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        [Test]
        public void GetAllReportFeedBack_ReturnsData_ReturnsOk()
        {
            var reports = new List<ReportFeedBack>
            {
                new ReportFeedBack { ReportID = 1, ReporterEmail = "user1@example.com", ReasonReport = "Reason 1", Status = "Pending" },
                new ReportFeedBack { ReportID = 2, ReporterEmail = "user2@example.com", ReasonReport = "Reason 2", Status = "Approved" }
            };

            _mockRepository.Setup(repo => repo.GetAllReportFeedBack())
                .Returns(new ResponseMessage { Success = true, Data = reports, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            var result = adminReportController.GetAllReport() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(responseMessage.Data, Is.EqualTo(reports));
        }

        [Test]
        public void GetAllReportFeedBack_NoDataFound_ReturnsNotFound()
        {
            var reports = new List<ReportFeedBack>();

            _mockRepository.Setup(repo => repo.GetAllReportFeedBack())
                .Returns(new ResponseMessage { Success = false, Data = reports, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            var result = adminReportController.GetAllReport() as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        [Test]
        public void ConfirmReport_ReturnsOk()
        {
            int reportID = 1;
            var report = new ReportFeedBack { ReportID = reportID, Status = "Pending" };
            string emailContent = "Your request for a feedback report has been processed.";

            _mockRepository.Setup(repo => repo.ConfirmReport(reportID))
                .Returns(new ResponseMessage { Success = true, Data = report, Message = emailContent, StatusCode = (int)HttpStatusCode.OK });

            var result = adminReportController.ConfirmReport(reportID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo(emailContent));
            Assert.That(report.Status, Is.EqualTo("Approved"));
        }

        [Test]
        public void ConfirmReport_ReportNotFound_ReturnsNotFound()
        {
            int reportID = 999;

            _mockRepository.Setup(repo => repo.ConfirmReport(reportID))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            var result = adminReportController.ConfirmReport(reportID) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }

        [Test]
        public void RejectReport_ReturnsOk()
        {
            int reportID = 1;
            string reasonReject = "Not enough evidence";
            var report = new ReportFeedBack { ReportID = reportID, Status = "Pending" };

            _mockRepository.Setup(repo => repo.RejectReport(reportID, reasonReject))
                .Returns(new ResponseMessage { Success = true, Data = report, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK });

            var result = adminReportController.RejectReport(reportID, reasonReject) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(200, result.StatusCode);
            Assert.That(responseMessage.Success, Is.True);
            Assert.That(responseMessage.Message, Is.EqualTo("Successfully"));
            Assert.That(report.Status, Is.EqualTo("Rejected"));
        }

        [Test]
        public void RejectReport_ReportNotFound_ReturnsNotFound()
        {
            int reportID = 999;
            string reasonReject = "Not enough evidence";

            _mockRepository.Setup(repo => repo.RejectReport(reportID, reasonReject))
                .Returns(new ResponseMessage { Success = false, Data = null, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound });

            var result = adminReportController.RejectReport(reportID, reasonReject) as ObjectResult;
            var responseMessage = result.Value as ResponseMessage;

            Assert.AreEqual(404, result.StatusCode);
            Assert.That(responseMessage.Success, Is.False);
            Assert.That(responseMessage.Message, Is.EqualTo("Data not found"));
        }
    }
}