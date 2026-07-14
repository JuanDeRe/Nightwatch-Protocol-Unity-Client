using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class DoorStateChangedEvent
    {
        public string type;
        public string matchId;
        public string doorId;
        public bool isClosed;
        public string changedByPlayerId;
    }
}