using System;
using Nightwatch.Client.Contracts;
using Nightwatch.Client.Networking;
using Nightwatch.Client.Session;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Nightwatch.Client.Gameplay;

namespace Nightwatch.Client.UI
{
    /// <summary>
    /// Controls the initial realtime gameplay scene.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private RealtimeClient realtimeClient;

        [SerializeField]
        private TMP_Text connectionStatusText;

        [SerializeField]
        private TMP_Text matchInfoText;

        [SerializeField]
        private float movementSendInterval = 0.1f;

        [SerializeField]
        private PlayerRegistry playerRegistry;

        private float _nextMovementSendTime;

        private async void Start()
        {
            if (!SessionManager.IsAuthenticated ||
                string.IsNullOrWhiteSpace(SessionManager.MatchId))
            {
                SceneManager.LoadScene("LobbyScene");
                return;
            }
            matchInfoText.text =
                $"Partida: {SessionManager.MatchId}\n" +
                $"Rol: {SessionManager.GameRole}\n" +
                $"Animatrónico: {SessionManager.AnimatronicType}";

            connectionStatusText.color = Color.white;
            connectionStatusText.text = "Conectando...";

            realtimeClient.OnConnected += HandleConnected;
            realtimeClient.OnConnectionError += HandleConnectionError;
            realtimeClient.OnPlayerMoved += HandlePlayerMoved;
            realtimeClient.OnMatchSnapshot += HandleMatchSnapshot;

            await realtimeClient.ConnectAsync();
        }

        private void Update()
        {
            // Smoothly approaches the last authoritative position.

            if (!realtimeClient.IsConnected)
            {
                return;
            }

            // The guard does not use free movement.
            if (SessionManager.GameRole == "Guard")
            {
                return;
            }

            if (Time.time < _nextMovementSendTime)
            {
                return;
            }

            var keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            var directionX = 0f;
            var directionY = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                directionX = -1f;
            }
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                directionX = 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                directionY = -1f;
            }
            else if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                directionY = 1f;
            }

            if (Mathf.Approximately(directionX, 0f) &&
                Mathf.Approximately(directionY, 0f))
            {
                return;
            }

            _nextMovementSendTime =
                Time.time + movementSendInterval;

            _ = realtimeClient.SendMoveAsync(
                directionX,
                directionY,
                movementSendInterval
            );
        }

        public async void LeaveGame()
        {
            await realtimeClient.DisconnectAsync();
            SceneManager.LoadScene("LobbyScene");
        }

        private void HandleConnected()
        {
            connectionStatusText.color = Color.green;
            connectionStatusText.text = "Conectado al servidor.";
        }

        private void HandleConnectionError(string message)
        {
            connectionStatusText.color = Color.red;
            connectionStatusText.text = message;
        }

        private void HandlePlayerMoved(PlayerMovedEvent movementEvent)
        {
            playerRegistry.UpdatePlayer(
                movementEvent.playerId,
                movementEvent.x,
                movementEvent.y
            );

            if (movementEvent.playerId != SessionManager.UserId)
            {
                return;
            }

            matchInfoText.text =
                $"Partida: {SessionManager.MatchId}\n" +
                $"Rol: {SessionManager.GameRole}\n" +
                $"Animatrónico: {SessionManager.AnimatronicType}\n" +
                $"Sala: {movementEvent.roomId}\n" +
                $"Posición: {movementEvent.x:F2}, " +
                $"{movementEvent.y:F2}";
        }

        private void HandleMatchSnapshot(MatchSnapshotEvent snapshot)
        {
            if (snapshot.players == null)
            {
                return;
            }

            foreach (var player in snapshot.players)
            {
                playerRegistry.UpdatePlayer(
                    player.playerId,
                    player.x,
                    player.y
                );
            }

            matchInfoText.text =
                $"Partida: {snapshot.matchId}\n" +
                $"Estado: {snapshot.status}\n" +
                $"Jugadores: {snapshot.players.Length}\n" +
                $"Tu rol: {SessionManager.GameRole}\n" +
                $"Animatrónico: " +
                $"{SessionManager.AnimatronicType}";
        }

        private void OnDestroy()
        {
            realtimeClient.OnConnected -= HandleConnected;
            realtimeClient.OnConnectionError -= HandleConnectionError;
            realtimeClient.OnPlayerMoved -= HandlePlayerMoved;
            realtimeClient.OnMatchSnapshot -= HandleMatchSnapshot;
        }
    }
}