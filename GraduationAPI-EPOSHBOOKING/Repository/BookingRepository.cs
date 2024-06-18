using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.DTO;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Net.payOS;
using Net.payOS.Types;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SkiaSharp;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
#pragma warning disable // tắt cảnh báo để code sạch hơn

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DBContext db;
 
        public BookingRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetBookingByAccount(int accountID)
        {
            var getBooking = db.booking
                               .Include(room => room.Room)
                               .ThenInclude(hotel => hotel.Hotel)
                               .Where(booking => booking.Account.AccountID == accountID)
                               .ToList();
            var responseData = getBooking.Select(booking => new
            {
               BookingID = booking.BookingID,
               CheckInDate = booking.CheckInDate,
               CheckOutDate = booking.CheckOutDate,
               TotalPrice = booking.TotalPrice,
               UnitPrice = booking.UnitPrice,
               TaxesPrice = booking.TaxesPrice,
               NumberOfRoom = booking.NumberOfRoom,
               NumberOfGuest = booking.NumberGuest,
               Status = booking.Status,
               Room = new
               {
                   RoomID = booking.Room.RoomID,
                   TypeOfRoom = booking.Room.TypeOfRoom,
                   NumberOfCapacity = booking.Room.NumberCapacity,
                   Price = booking.Room.Price,
                   Quantity = booking.Room.Quantity,
                   SizeOfRoom = booking.Room.SizeOfRoom,
                   TypeOfBed = booking.Room.TypeOfBed
               },
               Hotel = new
               {
                   HotelID = booking.Room.Hotel.HotelID,
                   MainImage = booking.Room.Hotel.MainImage,
                   Name = booking.Room.Hotel.Name,
                   OpenIn = booking.Room.Hotel.OpenedIn,
                   Description = booking.Room.Hotel.Description,
                   HotelStandar = booking.Room.Hotel.HotelStandar,
                   IsRegister = booking.Room.Hotel.isRegister,
                   Status = booking.Room.Hotel.Status
               }
            });
            if (getBooking.Any())
            {
                return new ResponseMessage { Success = true, Data = responseData, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = responseData, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage CancleBooking(int bookingID, String Reason)
        {
            var booking = db.booking
                            .Include(room => room.Room)
                            .ThenInclude(hotel => hotel.Hotel)
                            .Include(account => account.Account)
                            .FirstOrDefault(booking => booking.BookingID == bookingID);
            if (booking != null)
            {
                if (CanCancelBooking(booking.CheckInDate))
                {
                    booking.Status = "Cancle";
                    booking.ReasonCancle = Reason;
                    db.booking.Update(booking);
                    db.SaveChanges();
                    var responseData = new
                    {
                        BookingID = booking.BookingID,
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate,
                        TotalPrice = booking.TotalPrice,
                        UnitPrice = booking.UnitPrice,
                        TaxesPrice = booking.TaxesPrice,
                        NumberOfRoom = booking.NumberOfRoom,
                        NumberOfGuest = booking.NumberGuest,
                        Status = booking.Status,
                        ReasonCancel = booking.ReasonCancle,
                        Room = booking.Room == null ? null : new
                        {
                            RoomID = booking.Room.RoomID,
                            TypeOfRoom = booking.Room.TypeOfRoom,
                            NumberOfCapacity = booking.Room.NumberCapacity,
                            Price = booking.Room.Price,
                            Quantity = booking.Room.Quantity,
                            SizeOfRoom = booking.Room.SizeOfRoom,
                            TypeOfBed = booking.Room.TypeOfBed,
                            Hotel = booking.Room.Hotel == null ? null : new
                            {
                                HotelID = booking.Room.Hotel.HotelID,
                                MainImage = booking.Room.Hotel.MainImage,
                                Name = booking.Room.Hotel.Name,
                                OpenedIn = booking.Room.Hotel.OpenedIn,
                                Description = booking.Room.Hotel.Description,
                                HotelStandard = booking.Room.Hotel.HotelStandar,
                                IsRegister = booking.Room.Hotel.isRegister,
                                Status = booking.Room.Hotel.Status
                            }
                        }
                    };
                    return new ResponseMessage { Success = true, Data = responseData, Message = "Cancle Success", StatusCode = (int)HttpStatusCode.OK };
                };
            }

            return new ResponseMessage
            {
                Success = false,
                Message = "Cancel failed. You must cancel 24 hours before check-in date.",
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        private bool CanCancelBooking(DateTime checkInDate)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan timeDifference = checkInDate - currentDateTime;

            // Check if the check-in date is more than 24 hours from now
            return timeDifference.TotalHours > 24;
        }
        public ResponseMessage ChangeStatusWaitForPayment(int bookingID)
        {
            var getBooking = db.booking.FirstOrDefault(booking => booking.BookingID == bookingID);
            if (getBooking != null)
            {
                getBooking.Status = "Wait For Payment";
                db.booking.Update(getBooking);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getBooking, Message = "Confirm Successfully", StatusCode= (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = getBooking, Message = "Fail", StatusCode = (int)(HttpStatusCode.NotFound)};  
        }

        public ResponseMessage ChangeStatusComplete(int bookingID)
        {
            var getBooking = db.booking.FirstOrDefault(booking => booking.BookingID==bookingID);
            if (getBooking != null)
            {
                getBooking.Status = "Complete";
                db.booking.Update(getBooking);
                db.SaveChanges();
                return new ResponseMessage { Success = true, Data = getBooking, Message = "Successfully", StatusCode=(int)HttpStatusCode.OK};
            }
            return new ResponseMessage { Success = false,Data = getBooking, Message = "Fail", StatusCode =(int)(HttpStatusCode.NotFound)};
        }

        public ResponseMessage ExportBookingbyHotelID(int hotelID)
        {
            var bookings = db.booking
                 .Include(b => b.Room)
                 .ThenInclude(r => r.Hotel)
                 .Include(b => b.Account)
                 .ThenInclude(a => a.Profile)
                 .Where(b => b.Room.Hotel.HotelID == hotelID)
                 .ToList();
            if (bookings.Count == 0)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "No bookings found for this hotel.",
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Booking List by Hotel");
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
                var listBookingWithMonth = db.booking
                    .Where(booking => booking.Room.Hotel.HotelID == hotelID)
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
                var ws2 = pck.Workbook.Worksheets.Add("Booking Revenues by Hotel");
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
                // Save to memory stream    
                MemoryStream stream = new MemoryStream();
                pck.SaveAs(stream);
                return new ResponseMessage
                {
                    Success = true,
                    Data = stream.ToArray(),
                    Message = "Excel file generated successfully.",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
        }
        public ResponseMessage GetAllBooking()
        {
            try
            {
                var bookings = db.booking
                                  .Include(b => b.Room)
                                  .ThenInclude(hotel => hotel.Hotel)
                                  .Include(b => b.Account)
                                  .ThenInclude(profile => profile.Profile)
                                  .ToList();

                return new ResponseMessage { Success = true, Data = bookings, Message = "Successfully retrieved all bookings.", StatusCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = false, Data = null, Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        } // chưa biết có dùng hay không

        public ResponseMessage ExportBookingsByAccountID(int accountID)
        {
            try
            {
                var bookings = db.booking
                    .Where(booking => booking.Account.AccountID == accountID && booking.Status == "Complete")
                    .Include(booking => booking.Room)
                    .ThenInclude(room => room.Hotel)
                    .Include(booking => booking.Voucher)
                    .Include(booking => booking.Account)
                    .ToList();
                // Check if there are any bookings
                if (bookings.Count == 0)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Data = null,
                        Message = "No bookings found for this account.",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                // Create an Excel package
                using (ExcelPackage pck = new ExcelPackage())
                {
                    // Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Booking List");
                    // Định dạng tiêu đề cột
                    ws.Cells[1, 1, 1, 9].Style.Font.Bold = true;
                    ws.Cells[1, 1, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    ws.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Set column headers
                    ws.Cells[1, 1].Value = "BookingID";
                    ws.Cells[1, 2].Value = "CheckInDate";
                    ws.Cells[1, 3].Value = "CheckOutDate";
                    ws.Cells[1, 4].Value = "TotalPrice";
                    ws.Cells[1, 5].Value = "UnitPrice";
                    ws.Cells[1, 6].Value = "TaxesPrice";
                    ws.Cells[1, 7].Value = "NumberOfRoom";
                    ws.Cells[1, 8].Value = "NumberGuest";
                    ws.Cells[1, 9].Value = "ReasonCancel";
                    //ws.Cells[1, 10].Value = "Status";

                    // Add data to the worksheet
                    int row = 2;
                    foreach (var booking in bookings)
                    {
                        ws.Cells[row, 1].Value = booking.BookingID;
                        ws.Cells[row, 2].Value = booking.CheckInDate.ToString("dd-MM-yyyy"); ;
                        ws.Cells[row, 3].Value = booking.CheckOutDate.ToString("dd-MM-yyyy"); ;
                        ws.Cells[row, 4].Value = booking.TotalPrice + " " + "VND";
                        ws.Cells[row, 5].Value = booking.UnitPrice + " " + "VND";
                        ws.Cells[row, 6].Value = booking.TaxesPrice;
                        ws.Cells[row, 7].Value = booking.NumberOfRoom;
                        ws.Cells[row, 8].Value = booking.NumberGuest;
                        ws.Cells[row, 9].Value = booking.ReasonCancle;
                        //ws.Cells[row, 10].Value = booking.Status;
                        row++;
                    }
                    // Định dạng dữ liệu
                    ws.Cells[2, 1, ws.Dimension.End.Row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Autofit columns
                    ws.Columns.AutoFit();

                    // Save the Excel file to a memory stream
                    MemoryStream stream = new MemoryStream();
                    pck.SaveAs(stream);

                    // Return the Excel file as a response
                    return new ResponseMessage
                    {
                        Success = true,
                        Data = stream.ToArray(),
                        Message = "Excel file generated successfully.",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Error generating Excel file.",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public ResponseMessage ExportAllBookings()
        {
            try
            {
                var bookings = db.booking
                    .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                    .Include(b => b.Account)
                    .ThenInclude(a => a.Profile)
                    .ToList();

                if (bookings.Count == 0)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Data = null,
                        Message = "No bookings found.",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
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
                    var listBookingWithMonth = db.booking
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
                    // Save to memory stream
                    MemoryStream stream = new MemoryStream();
                    pck.SaveAs(stream);

                    return new ResponseMessage
                    {
                        Success = true,
                        Data = stream.ToArray(),
                        Message = "Excel file generated successfully.",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
            }

            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Error generating Excel file.",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public ResponseMessage ExportRevenues()
        {
            try
            {
                var bookings = db.booking
                    .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                    .Include(b => b.Account)
                    .ThenInclude(a => a.Profile)
                    .ToList();

                if (bookings.Count == 0)
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Data = null,
                        Message = "No bookings found.",
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var pck = new ExcelPackage())
                {

                    var listBookingWithMonth = db.booking
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
                    // Save to memory stream
                    MemoryStream stream = new MemoryStream();
                    pck.SaveAs(stream);

                    return new ResponseMessage
                    {
                        Success = true,
                        Data = stream.ToArray(),
                        Message = "Excel file generated successfully.",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
            }

            catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Data = null,
                    Message = "Error generating Excel file.",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public ResponseMessage AnalysisRevenueBookingSystem()
        {
            var listBookingWithMonth = db.booking
                                         .GroupBy(booking => new
                                         {
                                             CheckInDate = booking.CheckInDate.Month,
                                             CheckOutDate = booking.CheckOutDate.Month,
                                         }).ToList();
            var totalWithMonth = new Dictionary<int, double>();
            foreach (var months in listBookingWithMonth)
            {
                var checkInMonth = months.Key.CheckInDate;
                var totalRevenueForMonth = 0.0;
                foreach (var booking in months)
                {
                    totalRevenueForMonth += booking.TotalPrice;
                }
                if (!totalWithMonth.ContainsKey(checkInMonth))
                {
                    totalWithMonth.Add(checkInMonth, totalRevenueForMonth);
                }
                else
                {
                    totalWithMonth[checkInMonth] += totalRevenueForMonth;
                }
            }
            var monthName = new[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                 };
            var result = totalWithMonth.Select(booking => new BookingRevenuesData
            {
                Name = monthName[booking.Key - 1],
                Data = booking.Value
            });
            return new ResponseMessage { Success = true,Data = result,Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
        }

        public ResponseMessage AnalysisRevenueBookingHotel(int hotelID)
        {
            var listHotelBookingWithMonth = db.booking
                                              .Include(room => room.Room)
                                              .ThenInclude(hotel => hotel.Hotel)
                                              .Where(booking => booking.Room.Hotel.HotelID == hotelID)
                                              .GroupBy(booking => new
                                              {
                                                  CheckInDate = booking.CheckInDate.Month,
                                                  CheckOutDate = booking.CheckOutDate.Month
                                              }).ToList();
            var totalWithMonth = new Dictionary<int,double>();
            foreach (var monthGroup in listHotelBookingWithMonth)
            {
                var checkInMonth = monthGroup.Key.CheckInDate;
                var totalRevenueForMonth = 0.0;

                foreach (var booking in monthGroup)
                {
                    // Assume TotalAmount is the property storing the total amount of each booking
                    totalRevenueForMonth += booking.TotalPrice;
                }

                if (!totalWithMonth.ContainsKey(checkInMonth))
                {
                    totalWithMonth.Add(checkInMonth, totalRevenueForMonth);
                }
                else
                {
                    totalWithMonth[checkInMonth] += totalRevenueForMonth;
                }

               
            }
            var monthName = new[]
                {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                 };
            var result = totalWithMonth.Select(booking => new BookingRevenuesData
            {
                Name = monthName[booking.Key - 1],
                Data = booking.Value
            });
            return new ResponseMessage { Success = true, Data = result, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
        }

        public ResponseMessage CountBookingSystem()
        {
            var listBookingWithMonth = db.booking
                                         .GroupBy(booking => new
                                        {
                                           CheckInDate = booking.CheckInDate,
                                           CheckOutDate = booking.CheckOutDate
                                         }).ToList();
            var QuantityBookingWithMonth = new Dictionary<int, int>();

            foreach (var months in listBookingWithMonth)
            {
                var CheckInMonth = months.Key.CheckInDate.Month;

                if (!QuantityBookingWithMonth.ContainsKey(CheckInMonth))
                {
                    QuantityBookingWithMonth.Add(CheckInMonth, months.Count());
                }
                else
                {
                    QuantityBookingWithMonth[CheckInMonth] += months.Count();
                }
            }
            var monthName = new[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };
            var result = QuantityBookingWithMonth.Select(qb => new BookingDataDTO
            {
                Name = monthName[qb.Key - 1],
                Data = qb.Value
            }).ToList();
            return new ResponseMessage { Success = true, Data = result, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };

        }

        public ResponseMessage CountBookingHotel(int hotelID)
        {
            var listBookingWithMonth = db.booking
                                         .Include(room => room.Room)
                                         .ThenInclude(hotel => hotel.Hotel)
                                         .Where(booking => booking.Room.Hotel.HotelID == hotelID)
                                        .GroupBy(booking => new
                                        {
                                            CheckInDate = booking.CheckInDate,
                                            CheckOutDate = booking.CheckOutDate
                                        }).ToList();
            var QuantityBookingWithMonth = new Dictionary<int, int>();

            foreach (var months in listBookingWithMonth)
            {
                var CheckInMonth = months.Key.CheckInDate.Month;

                if (!QuantityBookingWithMonth.ContainsKey(CheckInMonth))
                {
                    QuantityBookingWithMonth.Add(CheckInMonth, months.Count());
                }
                else
                {
                    QuantityBookingWithMonth[CheckInMonth] += months.Count();
                }
            }

            var monthName = new[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };
            var result = QuantityBookingWithMonth.Select(qb => new BookingDataDTO
            {
                Name = monthName[qb.Key - 1],
                Data = qb.Value
            }).ToList();
            return new ResponseMessage { Success = true, Data = result, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };

        }

        public ResponseMessage Top5Booking()
        {
            var listBooking = db.booking
                                .Include(account => account.Account)
                                .ThenInclude(profile => profile.Profile)
                                .Include(room => room.Room)
                                .ThenInclude(hotel => hotel.Hotel)
                                .ToList();
            var top5Booking = listBooking.GroupBy(account => account.Account.AccountID)
                                         .Select(group => new
                                         {
                                             AccountID = group.Key,
                                             TotalBooking = group.Count(),
                                             Spending = group.Sum(booking => booking.TotalPrice),
                                             Account = group.First().Account
                                         })
                                         .OrderByDescending(spending => spending.Spending)
                                         .Select(top => new Top5BookingDTO
                                         {
                                             avatar = top.Account.Profile.Avatar,
                                             fullName = top.Account.Profile.fullName,
                                             TotalBooking = top.TotalBooking,
                                             Spending = (int)top.Spending
                                         }).ToList(); 
            return new ResponseMessage { Success = true,Data = top5Booking, Message = "Top 5 Booking", StatusCode= (int)HttpStatusCode.OK };
        }

        public ResponseMessage GetBookingByHotel(int hotelID)
        {
            var listBooking = db.booking
                                .Include(account => account.Account)
                                .ThenInclude(profile => profile.Profile)
                                .Include(room => room.Room)
                                .ThenInclude(roomImage => roomImage.RoomImages)
                                .Include(service => service.Room.RoomService)
                                .ThenInclude(subService => subService.RoomSubServices)
                                .Include(hotel => hotel.Room.Hotel)
                                .Where(h => h.Room.Hotel.HotelID == hotelID)
                                .ToList().Select(booking => new
                                {
                                    BookingID = booking.BookingID,
                                    CheckInDate = booking.CheckInDate,
                                    CheckOutDate = booking.CheckOutDate,
                                    TotalPrice = booking.TotalPrice,
                                    TaxesPrice = booking.TaxesPrice,
                                    NubmerOfRoom = booking.NumberOfRoom,
                                    NumberOfGuest = booking.NumberGuest,
                                    Status = booking.Status,
                                    Room = new
                                    {
                                       RoomID = booking.Room.RoomID,
                                       TypeOfRoom = booking.Room.TypeOfRoom,
                                       NumberCapacity = booking.Room.NumberCapacity,
                                       Price = booking.Room.Price,
                                       Quantity = booking.Room.Quantity,
                                       SizeOfRoom = booking.Room.SizeOfRoom,
                                       TypeOfBed = booking.Room.TypeOfBed,
                                       RoomImage = booking.Room.RoomImages.Select(img => new
                                       {
                                           Image = img.Image
                                       }).ToList(),
                                        RoomService = booking.Room.RoomService.Select(service => new
                                        {
                                            ServiceName = service.Type,
                                            RoomSubServices = service.RoomSubServices.Select(subService => new
                                            {
                                                SubServiceName = subService.SubName
                                            }).ToList()
                                        }).ToList()
                                    },
                                   
                                    Account = new
                                    {
                                        Email  = booking.Account.Email,
                                        Phone = booking.Account.Phone,

                                    },
                                    Profile = new
                                    {
                                        FullName = booking.Account.Profile.fullName,
                                        BirthDay = booking.Account.Profile.BirthDay,
                                        Gender = booking.Account.Profile.Gender,
                                        Address = booking.Account.Profile.Address,
                                        Avatar = booking.Account.Profile.Avatar
                                    }
                                });
            return new ResponseMessage { Success = true,Data = listBooking,Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                                
        }

        public ResponseMessage CreateBooking(CreateBookingDTO newBooking)
        {
            var account = db.accounts
                            .Include(profile => profile.Profile)
                            .FirstOrDefault(account => account.AccountID == newBooking.AccountID);
            var voucher = db.voucher.FirstOrDefault(voucher => voucher.VoucherID == newBooking.VoucherID);
            var room = db.room.Include(hotel => hotel.Hotel)
                              .FirstOrDefault(room => room.RoomID == newBooking.RoomID);
            if (voucher != null && voucher.QuantityUse >0)
            {
                Booking createBooking = new Booking
                {
                    Account = account,
                    Voucher = voucher,
                    Room = room,
                    CheckInDate = newBooking.CheckInDate,
                    CheckOutDate = newBooking.CheckOutDate,
                    UnitPrice = CheckRoomPrice(newBooking.RoomID,newBooking.CheckInDate,newBooking.CheckOutDate), 
                    TotalPrice = newBooking.TotalPrice,
                    TaxesPrice = newBooking.TaxesPrice,
                    NumberGuest = newBooking.NumberOfGuest,
                    NumberOfRoom = newBooking.NumberOfRoom,
                    Status = "WaitWait For Check-In"
                };
                db.booking.Add(createBooking);
                voucher.QuantityUse = voucher.QuantityUse - 1;
                room.Quantity = room.Quantity - newBooking.NumberOfRoom;
                db.voucher.Update(voucher);
                db.room.Update(room);
                if (voucher.QuantityUse == 0)
                {
                    var myvoucher = db.myVoucher
                                      .Include(account => account.Account)
                                      .Include(voucher => voucher.Voucher)
                                      .FirstOrDefault(myVoucher => myVoucher.VoucherID == newBooking.VoucherID);
                    if (myvoucher != null)
                    {
                        myvoucher.IsVoucher = false;
                        db.myVoucher.Update(myvoucher);
                        Ultils.Utils.sendMail(createBooking.Account.Email);
                    }
                }
                db.SaveChanges();
                var responseData = new
                {
                    BookingID = createBooking.BookingID,
                    CheckInDate = createBooking.CheckInDate,
                    CheckOutDate = createBooking.CheckOutDate,
                    TotalPrice = createBooking.TotalPrice,
                    UnitPrice = createBooking.UnitPrice,
                    TaxesPrice = createBooking.TaxesPrice,
                    NumberOfRoom = createBooking.NumberOfRoom,
                    NumberOfGuest = createBooking.NumberGuest,
                    Status = createBooking.Status,
                    ReasonCancel = createBooking.ReasonCancle,
                    Room = createBooking.Room == null ? null : new
                    {
                        RoomID = createBooking.Room.RoomID,
                        TypeOfRoom = createBooking.Room.TypeOfRoom,
                        NumberOfCapacity = createBooking.Room.NumberCapacity,
                        Price = createBooking.Room.Price,
                        Quantity = createBooking.Room.Quantity,
                        SizeOfRoom = createBooking.Room.SizeOfRoom,
                        TypeOfBed = createBooking.Room.TypeOfBed,
                        Hotel = createBooking.Room.Hotel == null ? null : new
                        {
                            HotelID = createBooking.Room.Hotel.HotelID,
                            MainImage = createBooking.Room.Hotel.MainImage,
                            Name = createBooking.Room.Hotel.Name,
                            OpenedIn = createBooking.Room.Hotel.OpenedIn,
                            Description = createBooking.Room.Hotel.Description,
                            HotelStandard = createBooking.Room.Hotel.HotelStandar,
                            IsRegister = createBooking.Room.Hotel.isRegister,
                            Status = createBooking.Room.Hotel.Status
                        }
                    }
                };
                return new ResponseMessage { Success = true, Data = responseData, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                Booking createBooking = new Booking
                {
                    Account = account,
                    Voucher = voucher,
                    Room = room,
                    CheckInDate = newBooking.CheckInDate,
                    CheckOutDate = newBooking.CheckOutDate,
                    UnitPrice = CheckRoomPrice(newBooking.RoomID, newBooking.CheckInDate, newBooking.CheckOutDate),
                    TotalPrice = newBooking.TotalPrice,
                    TaxesPrice = newBooking.TaxesPrice,
                    NumberGuest = newBooking.NumberOfGuest,
                    NumberOfRoom = newBooking.NumberOfRoom,
                    Status = "WaitWait For Check-In"
                };
                db.booking.Add(createBooking);
                room.Quantity = room.Quantity - newBooking.NumberOfRoom;
                db.room.Update(room);
                db.booking.Add(createBooking);
                db.SaveChanges();
                Ultils.Utils.SendMailBooking(createBooking.Account.Email,createBooking);

                var responseData = new
                {
                    BookingID = createBooking.BookingID,
                    CheckInDate = createBooking.CheckInDate,
                    CheckOutDate = createBooking.CheckOutDate,
                    TotalPrice = createBooking.TotalPrice,
                    UnitPrice = createBooking.UnitPrice,
                    TaxesPrice = createBooking.TaxesPrice,
                    NumberOfRoom = createBooking.NumberOfRoom,
                    NumberOfGuest = createBooking.NumberGuest,
                    Status = createBooking.Status,
                    ReasonCancel = createBooking.ReasonCancle,
                    Room = createBooking.Room == null ? null : new
                    {
                        RoomID = createBooking.Room.RoomID,
                        TypeOfRoom = createBooking.Room.TypeOfRoom,
                        NumberOfCapacity = createBooking.Room.NumberCapacity,
                        Price = createBooking.Room.Price,
                        Quantity = createBooking.Room.Quantity,
                        SizeOfRoom = createBooking.Room.SizeOfRoom,
                        TypeOfBed = createBooking.Room.TypeOfBed,
                        Hotel = createBooking.Room.Hotel == null ? null : new
                        {
                            HotelID = createBooking.Room.Hotel.HotelID,
                            MainImage = createBooking.Room.Hotel.MainImage,
                            Name = createBooking.Room.Hotel.Name,
                            OpenedIn = createBooking.Room.Hotel.OpenedIn,
                            Description = createBooking.Room.Hotel.Description,
                            HotelStandard = createBooking.Room.Hotel.HotelStandar,
                            IsRegister = createBooking.Room.Hotel.isRegister,
                            Status = createBooking.Room.Hotel.Status
                        }
                    }
                };
                return new ResponseMessage { Success = true, Data = responseData, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };

            }

        }

        public double CheckRoomPrice(int roomID, DateTime CheckInDate, DateTime CheckOutDate)
        {
            var room = db.room
                         .Include(specialPrice => specialPrice.SpecialPrice)
                         .FirstOrDefault(room => room.RoomID == roomID);
            var specialPrice = room.SpecialPrice
                              .FirstOrDefault(sp => CheckInDate >= sp.StartDate && CheckOutDate <= sp.EndDate);
            if (specialPrice != null)
            {
                room.Price = specialPrice.Price;
            }
            return room.Price;
        }

        public async Task<string> GeneratePaymentLink(PaymentRequestDTO request)
        {
             string clientId = "8e20b398-7a60-4ea7-ae11-c8b1267de435";
             string apiKey = "aa5333b5-b17c-4428-89de-2613fa7847ec";
             string checksumKey = "d00595dd40ad4898adf0cf4d636a26c237d8f0a72807074c111d67a0a426855e";

            var payOs = new PayOS(clientId,apiKey,checksumKey);
        var items = new List<ItemData>
             {
                 new ItemData(request.Description, request.TotalPrice, request.TotalPrice)
              };

            var paymentData = new PaymentData(
                request.BookingID,
                request.TotalPrice,
                request.Description,
                items,
                request.SuccessUrl,
                request.FailureUrl
            );

            var createPayment = await payOs.createPaymentLink(paymentData);
            return createPayment.checkoutUrl;
        }

 
    }
}
