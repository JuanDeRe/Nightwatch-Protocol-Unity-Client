using System.Collections.Generic;
using Nightwatch.Client.Session;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Creates and updates visual players by player ID.
    /// </summary>
    public class PlayerRegistry : MonoBehaviour
    {
        [SerializeField]
        private PlayerView playerPrefab;

        [SerializeField]
        private Transform playersContainer;

        [SerializeField]
        private float worldScale = 0.1f;

        private readonly Dictionary<string, PlayerView> _players = new();

        public void UpdatePlayer(
            string playerId,
            float serverX,
            float serverY)
        {
            var worldPosition = new Vector3(
                serverX * worldScale,
                serverY * worldScale,
                0f
            );

            if (!_players.TryGetValue(playerId, out var playerView))
            {
                playerView = Instantiate(
                    playerPrefab,
                    worldPosition,
                    Quaternion.identity,
                    playersContainer
                );

                var isLocalPlayer =
                    playerId == SessionManager.UserId;

                playerView.Initialize(
                    playerId,
                    worldPosition,
                    isLocalPlayer
                );

                _players[playerId] = playerView;
            }

            playerView.SetAuthoritativePosition(worldPosition);
        }

        public bool ContainsPlayer(string playerId)
        {
            return _players.ContainsKey(playerId);
        }
    }
}