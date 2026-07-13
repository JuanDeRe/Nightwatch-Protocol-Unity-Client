using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class PlayerMovedEvent
    {
        public string type;
        public string matchId;
        public string playerId;
        public float x;
        public float y;
        public string roomId;
    }
}