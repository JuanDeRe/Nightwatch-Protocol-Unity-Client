using System.Collections.Generic;
using Nightwatch.Client.Session;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Creates the correct visual prefab for each synchronized player.
    /// </summary>
    public class PlayerRegistry : MonoBehaviour
    {
        [Header("Player prefabs")]

        [SerializeField]
        private PlayerView guardPrefab;

        [SerializeField]
        private PlayerView foxyPrefab;

        [SerializeField]
        private PlayerView chicaPrefab;

        [SerializeField]
        private PlayerView bonniePrefab;

        [SerializeField]
        private PlayerView freddyPrefab;

        [SerializeField]
        private PlayerView fallbackPrefab;

        [Header("Scene configuration")]

        [SerializeField]
        private Transform playersContainer;

        [SerializeField]
        private SpriteRenderer mapRenderer;

        private readonly Dictionary<string, PlayerView> _players =
            new();

        public void RegisterPlayer(
            string playerId,
            string role,
            string animatronicType,
            float serverX,
            float serverY)
        {
            var worldPosition = ToWorldPosition(
                serverX,
                serverY
            );

            if (_players.TryGetValue(
                    playerId,
                    out var existingPlayer))
            {
                existingPlayer.SetAuthoritativePosition(
                    worldPosition
                );

                return;
            }

            var selectedPrefab = SelectPrefab(
                role,
                animatronicType
            );

            var playerView = Instantiate(
                selectedPrefab,
                worldPosition,
                Quaternion.identity,
                playersContainer
            );

            var isLocalPlayer =
                playerId == SessionManager.UserId;

            playerView.Initialize(
                playerId,
                role,
                animatronicType,
                worldPosition,
                isLocalPlayer
            );

            _players[playerId] = playerView;
        }

        public void UpdatePlayerPosition(
            string playerId,
            float serverX,
            float serverY)
        {
            var worldPosition = ToWorldPosition(
                serverX,
                serverY
            );

            if (!_players.TryGetValue(
                    playerId,
                    out var playerView))
            {
                Debug.LogWarning(
                    $"Player '{playerId}' has not been registered."
                );

                return;
            }

            playerView.SetAuthoritativePosition(worldPosition);
        }

        public bool ContainsPlayer(string playerId)
        {
            return _players.ContainsKey(playerId);
        }

        public PlayerView GetPlayer(string playerId)
        {
            return _players.TryGetValue(playerId, out var playerView)
                ? playerView
                : null;
        }


        private PlayerView SelectPrefab(
            string role,
            string animatronicType)
        {
            if (role == "Guard")
            {
                return guardPrefab != null
                    ? guardPrefab
                    : fallbackPrefab;
            }

            return animatronicType switch
            {
                "Foxy" => foxyPrefab,
                "Chica" => chicaPrefab,
                "Bonnie" => bonniePrefab,
                "Freddy" => freddyPrefab,
                _ => fallbackPrefab
            };
        }

        private Vector3 ToWorldPosition(float serverX,float serverY)
        {
            if (mapRenderer == null)
            {
                Debug.LogError("Map Renderer is not assigned.");
                return Vector3.zero;
            }

            const float serverMapWidth = 50f;
            const float serverMapHeight = 60f;

            var normalizedX = Mathf.Clamp01(
                serverX / serverMapWidth
            );

            var normalizedY = Mathf.Clamp01(
                serverY / serverMapHeight
            );

            var mapBounds = mapRenderer.bounds;

            var worldX = Mathf.Lerp(
                mapBounds.min.x,
                mapBounds.max.x,
                normalizedX
            );

            var worldY = Mathf.Lerp(
                mapBounds.min.y,
                mapBounds.max.y,
                normalizedY
            );

            return new Vector3(
                worldX,
                worldY,
                0f
            );
        }
    }
}