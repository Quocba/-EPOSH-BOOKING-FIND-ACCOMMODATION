﻿using GraduationAPI_EPOSHBOOKING.Model;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IBookingRepository
    {
        public ResponseMessage GetBookingByAccount(int AccountID);

        public ResponseMessage CancleBooking(int bookingID, String Reason);
        public ResponseMessage CreateBooking(int accountID, int voucherID,int RoomID,Booking? booking);
    }
}
