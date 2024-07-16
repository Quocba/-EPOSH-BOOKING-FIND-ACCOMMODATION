using GraduationAPI_EPOSHBOOKING.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GraduationAPI_EPOSHBOOKING.BackgroundService
{
    public class BookingStatusServcie : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        public BookingStatusServcie(IServiceProvider serviceProvider, ILogger<BookingStatusServcie> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Booking Status Service Is Running");
            timer = new Timer(UpdateBookingStatus, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }
        private void UpdateBookingStatus(object state)
        {

            logger.LogInformation("Checking and updating booking statuses.");
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
                    var currentDate = DateTime.Now.AddHours(14);
                    var bookings = dbContext.booking
                        .Include(b => b.Room)
                        .Include(account => account.Account)
                        .Where(b => b.Status.Equals("Awaiting Check-in") && currentDate.Date > b.CheckInDate)
                        .ToList();

                    foreach (var booking in bookings)
                    {
                        booking.Status = "Canceled";
                        booking.ReasonCancle = "Your reservation has been canceled due to your failure to arrive to check in.";
                        booking.Room.Quantity += booking.NumberOfRoom;
                        dbContext.booking.Update(booking);
                        dbContext.room.Update(booking.Room);
                        Ultils.Utils.SendMailRegistration(booking.Account.Email, booking.ReasonCancle);

                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating booking statuses.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("BookingStatusService is stopping.");
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;

        }
    }
}
