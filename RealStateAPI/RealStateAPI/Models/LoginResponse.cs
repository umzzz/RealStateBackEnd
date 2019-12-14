using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class LoginResponse
    {
        public LoginResponse(string token, string userName)
        {
            Token = token;
            UserName = userName;
        }

        public string Token { get; private set; }

        public string UserName { get; private set; }

    }
}
