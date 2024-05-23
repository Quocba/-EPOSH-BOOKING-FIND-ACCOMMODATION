using GraduationAPI_EPOSHBOOKING.DataAccess;
using GraduationAPI_EPOSHBOOKING.IRepository;
using GraduationAPI_EPOSHBOOKING.Model;
using GraduationAPI_EPOSHBOOKING.Ultils;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GraduationAPI_EPOSHBOOKING.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DBContext _context;

        public AccountRepository(DBContext context)
        {
            _context = context;
        }

        public ResponseMessage Register(string email, string password, string fullName, string phone)
        {
            if (_context.accounts.Any(a => a.Email == email))
            {
                return new ResponseMessage
                {
                    Success = false,
                    Message = "Email is already registered",
                    StatusCode = 400
                };
            }

            string hashedPassword = Utils.HashPassword(password);
            var account = new Account
            {
                Email = email,
                Password = hashedPassword,
                IsActive = true,
                Phone = phone,
                Role = _context.roles.FirstOrDefault(r => r.Name == "Customer") 
            };
            var profile = new Profile
            {
                fullName = fullName
            };

            account.Profile = profile;
            _context.accounts.Add(account);
            _context.SaveChanges();

            return new ResponseMessage
            {
                Success = true,
                Message = "Registration Successfully",
                StatusCode = 201,
                Data = new { account.AccountID, account.Email, profile.fullName }
            };
        }
    }
}
