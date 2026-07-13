using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }
}