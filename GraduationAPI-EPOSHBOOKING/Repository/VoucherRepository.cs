using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly DBContext db;

        public VoucherRepository(DBContext _db)
        {
            this.db = _db;
        }

        public ResponseMessage GetAllVouchers()
        {
            try
            {
                var vouchers = db.voucher.ToList();

                if (vouchers != null && vouchers.Any())
                {
                    return new ResponseMessage
                    {
                        Success = true,
                        Message = "Successfully",
                        Data = vouchers,
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        Success = false,
                        Message = "Not Found",
                        Data = null,
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }
            }catch (Exception ex)
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Internal Server Error",
                    Data = null,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
          
        }
    }
}
