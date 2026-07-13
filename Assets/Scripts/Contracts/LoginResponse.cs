using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class LoginResponse
    {
        public string token;
        public string userId;
        public string username;
        public string role;
    }
}