using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class MoveCommandMessage
    {
        public string type = "MoveCommand";
        public string matchId;
        public float directionX;
        public float directionY;
        public float deltaTime;
    }
}