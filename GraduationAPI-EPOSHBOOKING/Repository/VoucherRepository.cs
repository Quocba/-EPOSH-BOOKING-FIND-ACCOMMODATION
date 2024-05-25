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
            }
            catch (Exception ex)
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

        public ResponseMessage GetVoucherById(int voucherId)
        {
            try
            {
                var voucher = db.voucher.FirstOrDefault(c => c.VoucherID == voucherId);
                if (voucher != null)
                {
                    return new ResponseMessage
                    {
                        Success = true,
                        Message = "Successfully",
                        Data = voucher,
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
            }
            catch (Exception ex)
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


        public ResponseMessage GetVouchersByAccountId(int accountId)
        {
            var myVoucher = db.myVoucher.Include(voucher => voucher.Voucher)
                .Where(account => account.AccountID == accountId)
                .Select(mv => mv.Voucher)
                .ToList();
            if (myVoucher.Any())
            {
                return new ResponseMessage {Success = true, Data = myVoucher, Message = "Successfully", StatusCode = (int)HttpStatusCode.OK};
            }

                return new ResponseMessage {Success = false, Data = myVoucher, Message = "Data not found", StatusCode = (int)HttpStatusCode.NotFound};
        }

        public ResponseMessage ReceiveVoucher(int accountID, int voucherID)
        {
            try
            {
                var account = db.accounts.FirstOrDefault(account => account.AccountID == accountID);
                var voucher = db.voucher.FirstOrDefault(voucher => voucher.VoucherID == voucherID);
                if (account != null && voucher != null)
                {
                    var checkAlready = db.myVoucher.FirstOrDefault(x => x.AccountID == accountID && x.VoucherID == voucherID);
                    var voucherData = new
                    {
                        VoucherID = voucher.VoucherID,
                        VoucherName = voucher.VoucherName,
                        Code = voucher.Code,
                        QuantityUsed = voucher.QuantityUseed,
                        Discount = voucher.Discount,
                        Description = voucher.Description
                    };

                    if (checkAlready != null)
                    {
                        return new ResponseMessage { Success = true, Data = voucherData, Message = "You have already received this voucher", StatusCode = (int)HttpStatusCode.OK };

                    }
                    else
                    {
                        MyVoucher addMyVoucher = new MyVoucher
                        {
                            Account = account,
                            Voucher = voucher,
                            AccountID = account.AccountID,
                            VoucherID = voucher.VoucherID,
                            IsVoucher = true

                        };
                        db.myVoucher.Add(addMyVoucher);
                        db.SaveChanges();
                        
                        return new ResponseMessage
                        {
                            Success = true,
                            Data = voucherData,
                            Message = "Received Voucher successfully" ,
                            StatusCode = (int)HttpStatusCode.OK
                        };
                    }
                    
                }
                else
                {
                    return new ResponseMessage { Success = false, Data = null, Message = "Voucher receipt failed",StatusCode = (int)HttpStatusCode.NotFound};
                }
            }
            catch (Exception ex)
            {
                return new ResponseMessage {Success = false, Message = ex.Message, StatusCode = (int)HttpStatusCode.InternalServerError};
            }
            
        }
    }
}

