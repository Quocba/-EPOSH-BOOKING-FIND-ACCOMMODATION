﻿using DocumentFormat.OpenXml.InkML;
using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;
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
            var getBooking = db.booking.Where(booking => booking.Account.AccountID == accountID).Include(room => room.Room).ThenInclude(hotel => hotel.Hotel)
                .ToList();
            if (getBooking.Any())
            {
                return new ResponseMessage { Success = true, Data = getBooking, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false, Data = getBooking, Message = "No Booking", StatusCode = (int)HttpStatusCode.NotFound };
        }

        public ResponseMessage CancleBooking(int bookingID, String Reason)
        {
            var getBooking = db.booking.Include(account => account.Account).FirstOrDefault(booking => booking.BookingID == bookingID);
            if (getBooking != null)
            {
                if (CanCancelBooking(getBooking.CheckInDate))
                {
                    getBooking.Status = "Cancle";
                    getBooking.ReasonCancle = Reason;
                    db.booking.Update(getBooking);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = getBooking, Message = "Cancle Success", StatusCode = (int)HttpStatusCode.OK };
                }

            }
            return new ResponseMessage
            {
                Success = false,
                Data = getBooking,
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

        public ResponseMessage CreateBooking(int accountID, int voucherID, int RoomID, Booking? booking)
        {
            try
            {
                var account = db.accounts
               .Include(profile => profile.Profile)
               .FirstOrDefault(account => account.AccountID == accountID);
                var room = db.room.Include(hotel => hotel.Hotel)
                    .Include(service => service.RoomService)
                    .ThenInclude(subService => subService.RoomSubServices)
                    .Include(specialPrice => specialPrice.SpecialPrice)
                    .FirstOrDefault(room => room.RoomID == RoomID);
                var voucher = db.voucher.FirstOrDefault(voucher => voucher.VoucherID == voucherID && voucher.QuantityUseed > 0);

                double unitPrice = room.Price;
                var specialPrice = room.SpecialPrice
                       .FirstOrDefault(sp => sp.StartDate <= booking.CheckInDate && booking.CheckOutDate <= sp.EndDate);
                if (specialPrice != null)
                {
                    unitPrice = specialPrice.Price;
                };
                if (voucher != null && voucher.QuantityUseed > 0)
                {
                    double totalPrice = unitPrice * booking.NumberOfRoom * (booking.CheckOutDate - booking.CheckInDate).Days;
                    double disscount = totalPrice * (voucher.Discount / 100);
                    double totalPriceDisscount = totalPrice - disscount;
                    double taxesPrice = totalPriceDisscount * 0.05;
                    Booking createBookingWithVoucher = new Booking
                    {
                        Account = account,
                        Room = room,
                        Voucher = voucher,
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate,
                        TotalPrice = totalPriceDisscount,
                        UnitPrice = unitPrice,
                        TaxesPrice = taxesPrice,
                        NumberGuest = booking.NumberGuest,
                        NumberOfRoom = booking.NumberOfRoom,
                        Status = "Wait For Confirm"
                    };
                    voucher.QuantityUseed = voucher.QuantityUseed - 1;
                    room.Quantity = room.Quantity - booking.NumberOfRoom;
                    db.voucher.Update(voucher);
                    db.booking.Add(createBookingWithVoucher);
                    db.room.Update(room);
                    if (voucher.QuantityUseed == 0)
                    {
                        var myvoucher = db.myVoucher
                             .Include(account => account.Account)
                             .Include(voucher => voucher.Voucher)
                             .FirstOrDefault(myvoucher => myvoucher.Voucher.VoucherID == voucherID && myvoucher.Account.AccountID == accountID);
                        if (myvoucher != null)
                        {
                            myvoucher.IsVoucher = false;
                            db.myVoucher.Update(myvoucher);
                        };
                    }
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = createBookingWithVoucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {

                    double totalPrice = unitPrice * booking.NumberOfRoom * (booking.CheckOutDate - booking.CheckInDate).Days;
                    double taxesPrice = totalPrice * 0.05;
                    Booking createBooking = new Booking
                    {
                        Account = account,
                        Room = room,
                        Voucher = voucher,
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate,
                        TotalPrice = totalPrice,
                        UnitPrice = unitPrice,
                        TaxesPrice = taxesPrice,
                        NumberGuest = booking.NumberGuest,
                        NumberOfRoom = booking.NumberOfRoom,
                        Status = "Wait For Confirm"

                    };
                    room.Quantity = room.Quantity - booking.NumberOfRoom;
                    db.room.Update(room);
                    db.booking.Add(createBooking);
                    db.SaveChanges();
                    return new ResponseMessage { Success = true, Data = createBooking, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK };
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = true, Data = ex, Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
        public ResponseMessage GetAllBookings()
        {
            try
            {
                var bookings = db.booking
                                  .Include(b => b.Room)
                                  .Include(b => b.Account)
                                  .ToList();

                return new ResponseMessage { Success = true, Data = bookings, Message = "Successfully retrieved all bookings.", StatusCode = (int)HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Success = false, Data = null, Message = "Internal Server Error", StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
        // Xuất bằng trình duyệt Edge nhé, mấy cái khác export lỗi không bt nguyên nhân
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
                    ws.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
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
    }
}
