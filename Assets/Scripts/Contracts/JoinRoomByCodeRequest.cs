using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class JoinRoomByCodeRequest
    {
        public string playerId;
        public string inviteCode;
    }
}