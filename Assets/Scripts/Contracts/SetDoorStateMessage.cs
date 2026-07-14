using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class SetDoorStateMessage
    {
        public string type = "SetDoorStateCommand";
        public string matchId;
        public string doorId;
        public bool isClosed;
    }
}