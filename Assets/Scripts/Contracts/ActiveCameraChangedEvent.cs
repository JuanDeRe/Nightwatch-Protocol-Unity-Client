using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class ActiveCameraChangedEvent
    {
        public string type;
        public string matchId;
        public string cameraId;
        public string changedByPlayerId;
    }
}
