using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class JoinQueueResponse
    {
        public string status;
        public string message;
        public string matchId;
        public string gameServiceId;
        public string role;
        public string animatronicType;
    }
}