namespace Nightwatch.Client.Session
{
    public static class SessionManager
    {
        public static string Token { get; private set; } = string.Empty;

        public static string UserId { get; private set; } = string.Empty;

        public static string Username { get; private set; } = string.Empty;

        public static string Role { get; private set; } = string.Empty;

        public static string MatchId { get; private set; } = string.Empty;

        public static string GameServiceId { get; private set; } = string.Empty;

        public static string GameRole { get; private set; } = string.Empty;

        public static string AnimatronicType { get; private set; } = string.Empty;

        public static bool IsAuthenticated =>
            !string.IsNullOrWhiteSpace(Token);

        public static void StartSession(
            string token,
            string userId,
            string username,
            string role)
        {
            Token = token;
            UserId = userId;
            Username = username;
            Role = role;
        }

        public static void ClearSession()
        {
            Token = string.Empty;
            UserId = string.Empty;
            Username = string.Empty;
            Role = string.Empty;
            MatchId = string.Empty;
            GameServiceId = string.Empty;
            GameRole = string.Empty;
            AnimatronicType = string.Empty;
        }
        public static void SetMatchAssignment(
            string matchId,
            string gameServiceId,
            string gameRole,
            string animatronicType)
        {
            MatchId = matchId;
            GameServiceId = gameServiceId;
            GameRole = gameRole;
            AnimatronicType = animatronicType;
        }
    }
}