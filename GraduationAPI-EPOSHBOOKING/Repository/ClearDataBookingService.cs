using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.DTO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class ClearDataBookingService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly ILogger logger;
        private readonly IServiceProvider provider;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string exportFilePath = PathHelper.GetExportFilePath();
        public ClearDataBookingService(ILogger<ClearDataBookingService> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {     
            logger.LogInformation("Clear data service running");
            //timer = new Timer(Work, null, TimeSpan.Zero, TimeSpan.FromDays(3));
            ScheduleNextRun();
            return Task.CompletedTask;

        }
        private void ScheduleNextRun()
        {
            var now = DateTime.Now.AddHours(14);
            DateTime nextRun;

            if (now.Month != 12 && now.Day != 31)
            {
                // Nếu hôm nay là ngày 31 tháng 12, chạy ngay lập tức và lên lịch cho lần chạy tiếp theo vào năm sau.
                nextRun = now;
            }
            else
            {
                // Tính toán thời gian cho lần chạy tiếp theo vào ngày 31 tháng 12 năm nay hoặc năm sau nếu đã qua ngày này.
                nextRun = new DateTime(now.Year, 12, 31, 0, 0, 0, DateTimeKind.Utc);
                if (now > nextRun)
                {   
                    nextRun = nextRun.AddYears(1);
                }
            }

            var timeToNextRun = nextRun - now;
            timer = new Timer(Work, null, timeToNextRun, Timeout.InfiniteTimeSpan);
        }
        private void Work(object state)
        {
            ExportAllBookings();
            SendFile("eposhhotel@gmail.com", exportFilePath);
            ExportBookingsForAllHotels();
            ClearData();
            ScheduleNextRun();

        }
        public void ClearData()
        {
            try
            {
                using (var scope = provider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
                    var currentDate = DateTime.Now.AddHours(14);
                    var booking = dbContext.booking
                                           .Include(x => x.Room)
                                           .Include(x => x.Room.Hotel)
                                           .Include(x => x.Account)
                                           .Include(x => x.Account.Profile)
                                           .ToList();
                    foreach (var book in booking)
                    {
                        book.Status = "Completed";
                        dbContext.booking.Update(book);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");

            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Clear Data Stopping");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {

            timer?.Dispose();

        }
        public static String SendFile(string toEmail, string attachmentFilePath)
        {
            // Cấu hình thông tin SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Thay đổi nếu cần
            string smtpUsername = "eposhhotel@gmail.com";
            string smtpPassword = "yqgorijrzzvpmwqa";

            try
            {
                // Tạo đối tượng SmtpClient
                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true; // Sử dụng SSL để bảo vệ thông tin đăng nhập

                    // Tạo đối tượng MailMessage
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(smtpUsername);
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = "[Annual Revenue Statistics Table]";
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = "Please find the attached file.";

                        // Thêm file đính kèm
                        if (File.Exists(attachmentFilePath))
                        {
                            Attachment attachment = new Attachment(attachmentFilePath);
                            mailMessage.Attachments.Add(attachment);
                        }
                        else
                        {
                            return "Attachment file not found.";
                        }

                        // Gửi email
                        client.Send(mailMessage);
                    }
                }
                return "Email sent successfully.";
            }
            catch (Exception ex)
            {
                return $"An error occurred while sending email: {ex.Message}";
            }
        }
        public void ExportBookingsForAllHotels()
        {
            try
            {
                using (var scope = provider.CreateScope())
                {
  


                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
                    var hotels = dbContext.hotel
                                          .Include(a => a.Account)
                                          .ThenInclude(p => p.Profile)
                                          .ToList();

                    foreach (var hotel in hotels)
                    {
                        var hotelID = hotel.HotelID;
                        var hotelName = hotel.Name;
                        var hotelOwnerEmail = hotel.Account.Email; // Giả sử có trường OwnerEmail trong bảng Hotel

                        // Lấy danh sách các bookings cho khách sạn hiện tại
                        var bookings = dbContext.booking
                            .Include(b => b.Room)
                            .ThenInclude(r => r.Hotel)
                            .Include(b => b.Account)
                            .ThenInclude(a => a.Profile)
                            .Where(b => b.Room.Hotel.HotelID == hotelID)
                            .ToList();

                        if (bookings.Count == 0)
                        {
                            logger.LogInformation($"No bookings found for hotel {hotelName} (ID {hotelID}).");
                            continue;
                        }

                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                        using (var pck = new ExcelPackage())
                        {
                            var ws = pck.Workbook.Worksheets.Add("Booking List");

                            // Header row data
                            string[] headers = {
                        "Booking ID", "Check-in Date", "Check-out Date", "Total Price", "Unit Price",
                        "Taxes Price", "Number of Rooms", "Number of Guests", "Cancellation Reason",
                        "Status", "Hotel Name", "Room Type", "Account Email", "Account Full Name"
                    };

                            // Add and format header row
                            for (int i = 0; i < headers.Length; i++)
                            {
                                ws.Cells[1, i + 1].Value = headers[i];
                            }
                            ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
                            ws.Cells[1, 1, 1, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[1, 1, 1, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[1, 1, 1, headers.Length].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                            // Add data rows
                            int row = 2;
                            foreach (var booking in bookings)
                            {
                                ws.Cells[row, 1].Value = booking.BookingID;
                                ws.Cells[row, 2].Value = booking.CheckInDate.ToString("dd-MM-yyyy");
                                ws.Cells[row, 3].Value = booking.CheckOutDate.ToString("dd-MM-yyyy");
                                ws.Cells[row, 4].Value = booking.TotalPrice + " " + "VND";
                                ws.Cells[row, 5].Value = booking.UnitPrice + " " + "VND";
                                ws.Cells[row, 6].Value = booking.TaxesPrice + " " + "VND";
                                ws.Cells[row, 7].Value = booking.NumberOfRoom;
                                ws.Cells[row, 8].Value = booking.NumberGuest;
                                ws.Cells[row, 9].Value = booking.ReasonCancle;
                                ws.Cells[row, 10].Value = booking.Status;
                                ws.Cells[row, 11].Value = booking.Room?.Hotel?.Name;
                                ws.Cells[row, 12].Value = booking.Room?.TypeOfRoom;
                                ws.Cells[row, 13].Value = booking.Account?.Email;
                                ws.Cells[row, 14].Value = booking.Account?.Profile?.fullName;
                                row++;
                            }

                            // Định dạng dữ liệu
                            ws.Cells[2, 1, ws.Dimension.End.Row, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells.AutoFitColumns();

                            // Save the file to disk
                            var exportFilePath = PathHelper.GetExportFilePath();
                            File.WriteAllBytes(exportFilePath, pck.GetAsByteArray());

                            // Gửi email với file đính kèm
                            var emailResult = SendFile(hotelOwnerEmail, exportFilePath);
                            logger.LogInformation($"Email result for hotel {hotelName}: {hotelOwnerEmail}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during data export.");
            }
        }


        private void ExportAllBookings()
        {
            try
            {
                using (var scope = provider.CreateScope())
                {


                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
                    var bookings = dbContext.booking
                        .Include(b => b.Room)
                        .ThenInclude(r => r.Hotel)
                        .Include(b => b.Account)
                        .ThenInclude(a => a.Profile)
                        .ToList();

                    if (bookings.Count == 0)
                    {
                        logger.LogInformation("No bookings found.");
                        return;
                    }

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                    using (var pck = new ExcelPackage())
                    {
                        var ws = pck.Workbook.Worksheets.Add("Booking List");

                        // Header row data
                        string[] headers = {
                        "Booking ID", "Check-in Date", "Check-out Date", "Total Price", "Unit Price",
                        "Taxes Price", "Number of Rooms", "Number of Guests", "Cancellation Reason",
                        "Status", "Hotel Name", "Room Type", "Account Email", "Account Full Name"
                    };

                        // Add and format header row
                        for (int i = 0; i < headers.Length; i++)
                        {
                            ws.Cells[1, i + 1].Value = headers[i];
                        }
                        ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
                        ws.Cells[1, 1, 1, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[1, 1, 1, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[1, 1, 1, headers.Length].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        // Add data rows
                        int row = 2;
                        foreach (var booking in bookings)
                        {
                            ws.Cells[row, 1].Value = booking.BookingID;
                            ws.Cells[row, 2].Value = booking.CheckInDate.ToString("dd-MM-yyyy");
                            ws.Cells[row, 3].Value = booking.CheckOutDate.ToString("dd-MM-yyyy");
                            ws.Cells[row, 4].Value = booking.TotalPrice + " " + "VND";
                            ws.Cells[row, 5].Value = booking.UnitPrice + " " + "VND";
                            ws.Cells[row, 6].Value = booking.TaxesPrice + " " + "VND";
                            ws.Cells[row, 7].Value = booking.NumberOfRoom;
                            ws.Cells[row, 8].Value = booking.NumberGuest;
                            ws.Cells[row, 9].Value = booking.ReasonCancle;
                            ws.Cells[row, 10].Value = booking.Status;
                            ws.Cells[row, 11].Value = booking.Room?.Hotel?.Name;
                            ws.Cells[row, 12].Value = booking.Room?.TypeOfRoom;
                            ws.Cells[row, 13].Value = booking.Account?.Email;
                            ws.Cells[row, 14].Value = booking.Account?.Profile?.fullName;
                            row++;
                        }

                        // Định dạng dữ liệu
                        ws.Cells[2, 1, ws.Dimension.End.Row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells.AutoFitColumns();

                        // Grouping and summarizing booking data by month
                        var listBookingWithMonth = dbContext.booking
                            .GroupBy(booking => new
                            {
                                CheckInMonth = booking.CheckInDate.Month,
                                CheckOutMonth = booking.CheckOutDate.Month,
                                CheckInYear = booking.CheckInDate.Year
                            }).ToList();

                        var totalWithMonth = new Dictionary<string, double>();
                        foreach (var months in listBookingWithMonth)
                        {
                            var checkInMonth = months.Key.CheckInMonth;
                            var checkInYear = months.Key.CheckInYear;
                            var totalRevenueForMonth = 0.0;
                            foreach (var booking in months)
                            {
                                totalRevenueForMonth += booking.TotalPrice;
                            }
                            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(checkInMonth);
                            var monthYear = $"{monthName}/{checkInYear}";
                            if (!totalWithMonth.ContainsKey(monthYear))
                            {
                                totalWithMonth.Add(monthYear, totalRevenueForMonth);
                            }
                            else
                            {
                                totalWithMonth[monthYear] += totalRevenueForMonth;
                            }
                        }

                        var result = totalWithMonth.Select(booking => new BookingRevenuesData
                        {
                            Name = booking.Key,
                            Data = booking.Value
                        });

                        var ws2 = pck.Workbook.Worksheets.Add("Booking Revenues");
                        ws2.Cells[1, 1].Value = "Month";
                        ws2.Cells[1, 2].Value = "Total Revenue";
                        int row2 = 2;
                        foreach (var booking in result)
                        {
                            ws2.Cells[row2, 1].Value = booking.Name;
                            ws2.Cells[row2, 2].Value = booking.Data + " " + "VND";
                            row2++;
                        }

                        ws2.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                        ws2.Cells[1, 1, 1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws2.Cells[1, 1, 1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        ws2.Cells[1, 1, 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells[2, 1, ws2.Dimension.End.Row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells.AutoFitColumns();

                        // Save the file to disk    
                        File.WriteAllBytes(exportFilePath, pck.GetAsByteArray());
                        logger.LogInformation($"Excel file generated and saved to {exportFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during data export.");
            }
        }
        public static class PathHelper
        {
            // Thuộc tính tĩnh để lưu trữ đường dẫn thư mục
            public static string ExportDirectory { get; }

            static PathHelper()
            {
                // Xác định đường dẫn thư mục gốc
                string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ExportDirectory = Path.Combine(rootDirectory, "OldBookingData");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(ExportDirectory))
                {
                    Directory.CreateDirectory(ExportDirectory);
                }
            }

            // Phương thức để lấy đường dẫn tệp
            public static string GetExportFilePath()
            {
                return Path.Combine(ExportDirectory, $"BookingDataOf{DateTime.Now:yyyy-MM-dd}.xlsx");
            }
        }
    }
}
