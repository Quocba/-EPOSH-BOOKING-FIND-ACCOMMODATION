﻿using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;
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
                return new ResponseMessage { Success = false,Data = getBooking, Message = "No Booking",StatusCode = (int)HttpStatusCode.NotFound };
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
            return new ResponseMessage { Success = false,Data = getBooking, Message = "Cancel failed. You must cancel 24 hours before check-in date.", 
                StatusCode = (int)HttpStatusCode.NotFound };
        }

        private bool CanCancelBooking(DateTime checkInDate)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeSpan timeDifference = checkInDate - currentDateTime;

            // Check if the check-in date is more than 24 hours from now
            return timeDifference.TotalHours > 24;
        }

        public ResponseMessage CreateBooking(int accountID, int voucherID, int RoomID,Booking? booking)
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
        // đã format thêm màu
        public byte[] ExportBookingsByAccountID(int accountID)
        {
            var bookings = db.booking.Where(booking => booking.Account.AccountID == accountID)
                .Include(room => room.Room)
                .ThenInclude(hotel => hotel.Hotel)
                .ToList();
            if (!bookings.Any())
            {
                return null;
            } 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Bookings");

                // Header row
                var headers = new[]
                {
            "BookingID", "CheckInDate", "CheckOutDate", "TotalPrice", "UnitPrice", "TaxesPrice", "NumberOfRoom", "NumberGuest", "ReasonCancel"
            };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    worksheet.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, i + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }

                var row = 2;
                foreach (var booking in bookings)
                {
                    worksheet.Cells[row, 1].Value = booking.BookingID;
                    worksheet.Cells[row, 2].Value = booking.CheckInDate;
                    worksheet.Cells[row, 3].Value = booking.CheckOutDate;
                    worksheet.Cells[row, 4].Value = booking.TotalPrice;
                    worksheet.Cells[row, 5].Value = booking.UnitPrice;
                    worksheet.Cells[row, 6].Value = booking.TaxesPrice;
                    worksheet.Cells[row, 7].Value = booking.NumberOfRoom;
                    worksheet.Cells[row, 8].Value = booking.NumberGuest;
                    worksheet.Cells[row, 9].Value = booking.ReasonCancle;

                    for (int col = 1; col <= headers.Length; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                return stream.ToArray();
            }
        }

    }
}
