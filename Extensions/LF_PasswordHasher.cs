using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using lonefire.Models;

namespace lonefire.Extensions
{
    //Override default Password Hasher for compatibility
    public class LF_PasswordHasher : IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(string password)
        {
            return CalcMd5(CalcMd5(password));
        }

        public string HashPassword(ApplicationUser user, string password)
        {
            return CalcMd5(CalcMd5(password));
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(CalcMd5(CalcMd5(providedPassword))))
                return PasswordVerificationResult.Success;
            else return PasswordVerificationResult.Failed;
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(CalcMd5(CalcMd5(providedPassword))))
                return PasswordVerificationResult.Success;
            else return PasswordVerificationResult.Failed;
        }

        #region Helper

        private string CalcMd5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            //Convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}
