using System.Security.Cryptography;
using Backend.Dto;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dc;

        public UserRepository(DataContext dc)
        {
            this.dc = dc;
        }

        public async Task<User> Authenticate(string usernameOrEmail, string passwordText)
        {
            var user = await dc.Users!
                .FirstOrDefaultAsync(x => x.Username == usernameOrEmail || x.Email == usernameOrEmail);

            if (user == null || user.KeyPassword == null)
                return null!;

            if (!MatchPasswordHash(passwordText, user.Password, user.KeyPassword))
                return null!;

            return user;
        }


        private bool MatchPasswordHash(string passwordText, byte[]? password, byte[]? passwordKey)
        {
            using (var hmac = new HMACSHA512(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));

                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != password[i])
                        return false;
                }

                return true;
            }
        }
        public void Register(UserDto user1)
        {
            if (string.IsNullOrEmpty(user1.Password))
            {
                throw new ArgumentException("Lozinka nije unesena ili nije valjana.");
            }
            else
            {
                byte[] passwordHash, passwordKey;

                using (var hmac = new HMACSHA512())
                {
                    passwordKey = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user1.Password));
                }

                User user = new()
                {
                    Name = user1.Name,
                    Lastname = user1.Lastname,
                    Image = user1.Image,
                    Verified = user1.Verified,
                    Username = user1.Username,
                    Email = user1.Email,
                    Password = passwordHash,
                    KeyPassword = passwordKey
                };

                dc.Users!.Add(user);
            }
        }

        public async Task<bool> UserAlreadyExists(string userName, string email)
        {
            return await dc.Users!
                .AnyAsync(x => x.Username == userName || x.Email == email);
        }

        public async Task<bool> UserAlreadyExistsByUsername(string userName)
        {
            return await dc.Users!
                .AnyAsync(x => x.Username == userName);
        }

        public async Task<bool> UserAlreadyExistsByEmail(string email)
        {
            return await dc.Users!
                .AnyAsync(x => x.Email == email);
        }

    }
}
