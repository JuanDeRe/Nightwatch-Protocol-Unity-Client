using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class ErrorResponse
    {
        public string type;
        public string message;
    }
}