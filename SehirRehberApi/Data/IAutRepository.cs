using SehirRehberApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberApi.Data
{
    public interface IAutRepository
    {
        Task<User> Register(User user, string password);
        User Login(string userName, string password);

        Task<bool> UserExsists(string userName);

    }
}
