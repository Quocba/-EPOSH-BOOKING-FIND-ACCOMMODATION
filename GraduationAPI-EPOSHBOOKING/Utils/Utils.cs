using GraduationAPI_EPOSHBOOKING.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace GraduationAPI_EPOSHBOOKING.Ultils
{
    public class Utils
    {
        public static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

       
        public static String sendMail(String toEmail)
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 999999);
            // Tạo mã OTP ngẫu nhiên
            string otp = otpNumber.ToString();

            // Cấu hình thông tin SMTP
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Thay đổi nếu cần
            string smtpUsername = "eposhhotel@gmail.com";
            string smtpPassword = "yqgorijrzzvpmwqa";

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
                    mailMessage.Subject = "EPOSH-BOOKING OTP CODE";
                    mailMessage.Body = $"Your OTP code is: {otp}";

                    // Gửi email
                    client.Send(mailMessage);
                }

            }
            return otp;
        }
        public static byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }

        public static string ConvertIFormFileToBase64(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
            return null;
        }
        public static string GenerateRandomString()
        {
            int length = 32;
            const string prefix = "EPOSH";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] stringChars = new char[length - prefix.Length]; // Đặt độ dài chuỗi ngẫu nhiên để giảm đi độ dài của prefix

            for (int i = 0; i < length - prefix.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return prefix + new string(stringChars);
        }

    }

}