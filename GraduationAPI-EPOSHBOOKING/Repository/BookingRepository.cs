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


        public ResponseMessage GetAllBooking()
        {
            var listBooking = db.booking.Include(room => room.Room)
                .Include(hotel => hotel)
                .Include(account => account.Account)
                .Include(voucher => voucher.Voucher)
                .ToList();
            if (listBooking != null)
            {
                return new ResponseMessage { Success = true, Data = listBooking, Message = "Successfully", StatusCode= (int)HttpStatusCode.OK };
            }
            return new ResponseMessage { Success = false,Data = listBooking, Message = "No Data", StatusCode=(int)HttpStatusCode.NotFound };
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
                var voucher = db.voucher.FirstOrDefault(voucher => voucher.VoucherID == voucherID && voucher.QuantityUsed > 0);

                double unitPrice = room.Price;
                var specialPrice = room.SpecialPrice
                       .FirstOrDefault(sp => sp.StartDate <= booking.CheckInDate && booking.CheckOutDate <= sp.EndDate);
                if (specialPrice != null)
                {
                    unitPrice = specialPrice.Price;
                };
                if (voucher != null && voucher.QuantityUsed > 0)
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
                        Status = "Wait For Check-In"
                    };
                    voucher.QuantityUsed = voucher.QuantityUsed - 1;
                    room.Quantity = room.Quantity - booking.NumberOfRoom;
                    db.voucher.Update(voucher);
                    db.booking.Add(createBookingWithVoucher);
                    db.room.Update(room);
                    if (voucher.QuantityUsed == 0)
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
                    Ultils.Utils.sendMail(createBookingWithVoucher.Account.Email);
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
                    Ultils.Utils.sendMail(createBooking.Account.Email);
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
        
       

    }
}
