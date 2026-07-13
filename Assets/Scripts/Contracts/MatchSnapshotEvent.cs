using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class MatchSnapshotEvent
    {
        public string type;
        public string matchId;
        public string status;
        public int durationSeconds;
        public string startedAtUtc;
        public MatchSnapshotPlayer[] players;
    }
}