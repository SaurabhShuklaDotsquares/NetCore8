using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Code.LIBS
{
    public class PasswordEncryption
    {
        public static bool IsPasswordMatch(string userPassword, string enteredPassword, string saltKey)
        {
            //if (enteredPassword == "Dots@123")
            //{
            //    return true;
            //}
            if (!String.IsNullOrEmpty(userPassword) && !String.IsNullOrEmpty(enteredPassword) && !String.IsNullOrEmpty(saltKey))
            {
                string savedPassword = GenerateEncryptedPassword(enteredPassword, saltKey, "MD5");

                return userPassword.Equals(savedPassword);
            }

            return false;
        }

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public static string CreateSaltKey(int size = 32)
        {
            //generate a cryptographic random number
            using (var provider = new RNGCryptoServiceProvider())
            {
                var buff = new byte[size];
                provider.GetBytes(buff);

                // Return a Base64 string representation of the random number
                return Convert.ToBase64String(buff);
            }
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public static string GenerateEncryptedPassword(string password, string saltkey, string passwordFormat = "MD5")
        {
            return CreateHash(Encoding.UTF8.GetBytes(String.Concat(saltkey, password)), passwordFormat);
        }

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Data hash</returns>
        private static string CreateHash(byte[] data, string hashAlgorithm = "MD5")
        {
            if (String.IsNullOrEmpty(hashAlgorithm))
            {
                hashAlgorithm = "MD5";
            }

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
            if (algorithm == null)
            {
                throw new ArgumentException("Unrecognized hash name");
            }

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        //public static string DecryptPassword(string hashedPassword, string salt)
        //{
        //    // Convert the hashed password and salt back to byte arrays
        //    byte[] hashedPasswordBytes = Encoding.UTF8.GetBytes(hashedPassword);
        //    byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

        //    // Concatenate the salt and hashed password bytes
        //    byte[] saltedPasswordBytes = new byte[hashedPasswordBytes.Length + saltBytes.Length];
        //    Array.Copy(saltBytes, 0, saltedPasswordBytes, 0, saltBytes.Length);
        //    Array.Copy(hashedPasswordBytes, 0, saltedPasswordBytes, saltBytes.Length, hashedPasswordBytes.Length);

        //    // Compute the MD5 hash of the concatenated value
        //    using (MD5 md5 = MD5.Create())
        //    {
        //        byte[] decryptedBytes = md5.ComputeHash(saltedPasswordBytes);
        //        return BitConverter.ToString(decryptedBytes).Replace("-", "").ToLower();
        //    }
        //}

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$"                        // non-alphanumeric
         //"!@$?_-"                    
         };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        public static string GenerateRandomOTP()
        {
            Random rnd = new Random();
            string n = rnd.Next(1000, 9999).ToString();
            return n;
        }
    }
}
