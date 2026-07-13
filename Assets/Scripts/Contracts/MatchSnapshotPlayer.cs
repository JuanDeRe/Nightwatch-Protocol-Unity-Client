using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class MatchSnapshotPlayer
    {
        public string playerId;
        public string role;
        public string animatronicType;
        public float x;
        public float y;
        public string roomId;
        public bool isConnected;
    }
}