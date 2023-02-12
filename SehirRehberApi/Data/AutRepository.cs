using Microsoft.EntityFrameworkCore;
using SehirRehberApi.Data;
using SehirRehberApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberApi.Data
{
    public class AutRepository : IAutRepository
    {

        DataContext _datacontext;
        public AutRepository(DataContext datacontext)
        {
            _datacontext = datacontext;
        }
        public User Login(string userName, string password)
        {
            var user =  _datacontext.Users.FirstOrDefault(u => u.Username == userName);
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;

            }
            return user;




        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;

                    }
                }
                return true;
            }

        }

        public async Task<User> Register(User user, string password)
        {
          
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _datacontext.Users.AddAsync(user);
            await _datacontext.SaveChangesAsync();
            return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> UserExsists(string userName)
        {
          if(await _datacontext.Users.AnyAsync(u=>u.Username==userName))
            {
                return true;

            }
            return false;
        }
    }
}
