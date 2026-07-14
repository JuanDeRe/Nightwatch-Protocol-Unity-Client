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

        [SerializeField]
        private DoorRegistry doorRegistry;

        [SerializeField]
        private GameObject guardControlsPanel;

        [SerializeField]
        private SecurityCameraRegistry securityCameraRegistry;

        [SerializeField]
        private GameObject guardCameraPanel;

        [SerializeField]
        private GameObject openCameraButton;

        [SerializeField]
        private TMP_Text gameClockText;

        [SerializeField]
        private TMP_Text batteryText;

        [SerializeField]
        private SecurityCameraFeedController securityCameraFeedController;

        private DateTime? _matchStartedAtUtc;
        private int _matchDurationSeconds;

        private float _nextMovementSendTime;

        private bool _hasReceivedMatchSnapshot;

        private float _nextClockUpdateTime;

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
            var isGuard = SessionManager.GameRole == "Guard";

            if (batteryText != null)
            {
                batteryText.gameObject.SetActive(isGuard);

                if (isGuard)
                {
                    batteryText.text = "Batería: 100%";
                }
            }

            guardCameraPanel.SetActive(false);
            openCameraButton.SetActive(isGuard);
            guardControlsPanel.SetActive(isGuard);

            realtimeClient.OnConnected += HandleConnected;
            realtimeClient.OnConnectionError += HandleConnectionError;
            realtimeClient.OnPlayerMoved += HandlePlayerMoved;
            realtimeClient.OnMatchSnapshot += HandleMatchSnapshot;
            realtimeClient.OnDoorStateChanged += HandleDoorStateChanged;
            realtimeClient.OnActiveCameraChanged += HandleActiveCameraChanged;
            

            await realtimeClient.ConnectAsync();
        }

        public void ToggleLeftDoor()
        {
            ToggleDoor("left_door");
        }

        public void ToggleRightDoor()
        {
            ToggleDoor("right_door");
        }

        private void ToggleDoor(string doorId)
        {
            if (SessionManager.GameRole != "Guard")
            {
                return;
            }

            if (!doorRegistry.TryGetDoorState(
                    doorId,
                    out var isClosed))
            {
                return;
            }

            _ = realtimeClient.SendDoorStateAsync(
                doorId,
                !isClosed
            );
        }
        public void OpenCameraMonitor()
        {
            if (SessionManager.GameRole != "Guard")
            {
                return;
            }

            guardCameraPanel.SetActive(true);
            openCameraButton.SetActive(false);

            // Selección inicial provisional.
            _ = realtimeClient.SendChangeCameraAsync("cam_1a");
        }

        public async void CloseCameraMonitor()
        {
            if (SessionManager.GameRole != "Guard")
            {
                return;
            }

            await realtimeClient.CloseCameraMonitorAsync();

            securityCameraFeedController.HideFeed();

            guardCameraPanel.SetActive(false);
            openCameraButton.SetActive(true);
        }

        public void SelectCamera1A()
        {
            SelectSecurityCamera("cam_1a");
        }

        public void SelectCamera1B()
        {
            SelectSecurityCamera("cam_1b");
        }

        public void SelectCamera1C()
        {
            SelectSecurityCamera("cam_1c");
        }

        public void SelectCamera2A()
        {
            SelectSecurityCamera("cam_2a");
        }

        public void SelectCamera2B()
        {
            SelectSecurityCamera("cam_2b");
        }

        public void SelectCamera3()
        {
            SelectSecurityCamera("cam_3");
        }

        public void SelectCamera4A()
        {
            SelectSecurityCamera("cam_4a");
        }

        public void SelectCamera4B()
        {
            SelectSecurityCamera("cam_4b");
        }

        public void SelectCamera5()
        {
            SelectSecurityCamera("cam_5");
        }

        public void SelectCamera6()
        {
            SelectSecurityCamera("cam_6");
        }

        public void SelectCamera7()
        {
            SelectSecurityCamera("cam_7");
        }

        private void SelectSecurityCamera(string cameraId)
        {
            if (SessionManager.GameRole != "Guard")
            {
                return;
            }

            _ = realtimeClient.SendChangeCameraAsync(cameraId);
        }

        private void HandleDoorStateChanged(
            DoorStateChangedEvent doorEvent)
        {
            doorRegistry.SetDoorState(
                doorEvent.doorId,
                doorEvent.isClosed
            );
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

            if (!Mathf.Approximately(directionX, 0f) ||
                !Mathf.Approximately(directionY, 0f))
            {
                Debug.Log(
                    $"Movement input: X={directionX}, Y={directionY}, " +
                    $"Connected={realtimeClient.IsConnected}, " +
                    $"Role={SessionManager.GameRole}"
                );
            }

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
            if (Time.unscaledTime >= _nextClockUpdateTime)
            {
                _nextClockUpdateTime =
                    Time.unscaledTime + 1f;

                UpdateGameClock();
            }

            _nextMovementSendTime =
                Time.time + movementSendInterval;

            _ = realtimeClient.SendMoveAsync(
                directionX,
                directionY,
                movementSendInterval
            );
        }

        private void HandleActiveCameraChanged(
            ActiveCameraChangedEvent cameraEvent)
        {
            securityCameraRegistry.SetActiveCamera(
                cameraEvent.cameraId
            );

            // Solo el guardia cambia su monitor.
            if (SessionManager.GameRole != "Guard")
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(
                    cameraEvent.cameraId))
            {
                securityCameraFeedController.HideFeed();
                return;
            }

            securityCameraFeedController.ShowFeed(
                cameraEvent.cameraId
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
            playerRegistry.UpdatePlayerPosition(
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
                playerRegistry.RegisterPlayer(
                    player.playerId,
                    player.role,
                    player.animatronicType,
                    player.x,
                    player.y
                );
            }
            if (snapshot.doors != null)
            {
                foreach (var door in snapshot.doors)
                {
                    doorRegistry.SetDoorState(
                        door.doorId,
                        door.isClosed
                    );
                }
            }

            securityCameraRegistry.SetActiveCamera(snapshot.activeCameraId);

            if (SessionManager.GameRole == "Guard")
            {
                if (string.IsNullOrWhiteSpace(
                        snapshot.activeCameraId))
                {
                    securityCameraFeedController.HideFeed();
                }
                else
                {
                    securityCameraFeedController.ShowFeed(
                        snapshot.activeCameraId
                    );
                }
            }

            _matchDurationSeconds = snapshot.durationSeconds;

            if (DateTime.TryParse(
                    snapshot.startedAtUtc,
                    null,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out var startedAt))
            {
                _matchStartedAtUtc = startedAt.ToUniversalTime();
                _hasReceivedMatchSnapshot = true;
            }
            else
            {
                Debug.LogWarning(
                    $"Invalid match start date: {snapshot.startedAtUtc}"
                );

                _matchStartedAtUtc = null;
                _hasReceivedMatchSnapshot = false;
            }

            matchInfoText.text =
                $"Partida: {snapshot.matchId}\n" +
                $"Estado: {snapshot.status}\n" +
                $"Jugadores: {snapshot.players.Length}\n" +
                $"Tu rol: {SessionManager.GameRole}\n" +
                $"Animatrónico: " +
                $"{SessionManager.AnimatronicType}";
        }
        private void UpdateGameClock()
        {
            if (gameClockText == null)
            {
                return;
            }

            if (!_hasReceivedMatchSnapshot ||
                !_matchStartedAtUtc.HasValue ||
                _matchDurationSeconds <= 0)
            {
                gameClockText.text = "12 AM";
                return;
            }

            var elapsedTime =
                DateTime.UtcNow - _matchStartedAtUtc.Value;

            var elapsedSeconds = Math.Max(
                0d,
                elapsedTime.TotalSeconds
            );

            var normalizedProgress = Mathf.Clamp01(
                (float)(
                    elapsedSeconds /
                    _matchDurationSeconds
                )
            );

            var gameHourIndex = Mathf.Min(
                6,
                Mathf.FloorToInt(normalizedProgress * 6f)
            );

            gameClockText.text = gameHourIndex == 0
                ? "12 AM"
                : $"{gameHourIndex} AM";
        }

        private void OnDestroy()
        {
            realtimeClient.OnConnected -= HandleConnected;
            realtimeClient.OnConnectionError -= HandleConnectionError;
            realtimeClient.OnPlayerMoved -= HandlePlayerMoved;
            realtimeClient.OnMatchSnapshot -= HandleMatchSnapshot;
            realtimeClient.OnDoorStateChanged -= HandleDoorStateChanged;
            realtimeClient.OnActiveCameraChanged -= HandleActiveCameraChanged;
        }
    }
}