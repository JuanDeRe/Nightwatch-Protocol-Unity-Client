using UnityEngine;

namespace Nightwatch.Client.Configuration

{
    public static class ApiConfiguration
    {
        public const string AuthServiceUrl =
            "http://localhost:5001";

        public const string MatchmakingServiceUrl =
            "http://localhost:5004";

        public const string RealtimeGatewayUrl =
            "ws://localhost:5003/game-ws";
    }
}