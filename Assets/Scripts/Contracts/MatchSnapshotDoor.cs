using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class MatchSnapshotDoor
    {
        public string doorId;
        public bool isClosed;
    }
}