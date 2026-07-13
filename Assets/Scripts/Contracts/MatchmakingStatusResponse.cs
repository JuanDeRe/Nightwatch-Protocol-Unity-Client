using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class MatchmakingStatusResponse
    {
        public string playerId;
        public string status;
        public string matchId;
        public string gameServiceId;
        public string role;
        public string animatronicType;
        public string inviteCode;
        public int currentPlayers;
        public int maxPlayers;
        public string countdownEndsAtUtc;
    }
}