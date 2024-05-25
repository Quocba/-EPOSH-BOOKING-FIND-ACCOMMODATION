using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;

namespace GraduationAPI_EPOSHBOOKING.IRepository
{
    public interface IVoucherRepository
    {
        public ResponseMessage GetAllVouchers();
        public ResponseMessage GetVoucherById(int voucherId);
        public ResponseMessage GetVouchersByAccountId(int accountId);

    }
}
