using GraduationAPI_EPOSHBOOKING.Controllers.Admin;
using GraduationAPI_EPOSHBOOKING.Controllers.Customer;
using GraduationAPI_EPOSHBOOKING.Controllers.Guest;
using GraduationAPI_EPOSHBOOKING.Controllers.Partner;
using GraduationAPI_EPOSHBOOKING.IRepository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestingAPI
{
    public class BookingTesting
    {
        private AdminBookingController AdminBookingController;
        private CustomerBookingController CustomerBookingController;
        private PartnerBookingController PartnerBookingController;
        private Mock<IBookingRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IBookingRepository>();
            AdminBookingController = new AdminBookingController(_mockRepository.Object);
            CustomerBookingController = new CustomerBookingController(_mockRepository.Object);
            PartnerBookingController = new PartnerBookingController(_mockRepository.Object);
        }
    }
}
