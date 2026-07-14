using System;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Nightwatch.Client.Configuration;
using Nightwatch.Client.Contracts;
using Nightwatch.Client.Session;
using UnityEngine;

namespace Nightwatch.Client.Networking
{
    /// <summary>
    /// Manages the WebSocket connection with RealtimeGateway.
    /// </summary>
    public class RealtimeClient : MonoBehaviour
    {
        private WebSocket _webSocket;

        public bool IsConnected =>
            _webSocket != null &&
            _webSocket.State == WebSocketState.Open;

        public event Action OnConnected;
        public event Action<string> OnConnectionError;
        public event Action<PlayerMovedEvent> OnPlayerMoved;
        public event Action<MatchSnapshotEvent> OnMatchSnapshot;
        public event Action<DoorStateChangedEvent> OnDoorStateChanged;
        public event Action<ActiveCameraChangedEvent> OnActiveCameraChanged;

        public async Task ConnectAsync()
        {
            if (!SessionManager.IsAuthenticated)
            {
                OnConnectionError?.Invoke("No existe una sesión autenticada.");
                return;
            }

            var encodedToken = Uri.EscapeDataString(SessionManager.Token);

            var encodedMatchId = Uri.EscapeDataString(SessionManager.MatchId);

            var url =
                $"{ApiConfiguration.RealtimeGatewayUrl}" +
                $"?access_token={encodedToken}" +
                $"&match_id={encodedMatchId}";

            _webSocket = new WebSocket(url);

            _webSocket.OnOpen += () =>
            {
                Debug.Log("WebSocket connected.");
                OnConnected?.Invoke();
            };

            _webSocket.OnError += error =>
            {
                Debug.LogError($"WebSocket error: {error}");
                OnConnectionError?.Invoke(error);
            };

            _webSocket.OnClose += closeCode =>
            {
                Debug.Log($"WebSocket closed: {closeCode}");
            };

            _webSocket.OnMessage += bytes =>
            {
                var json = Encoding.UTF8.GetString(bytes);
                HandleMessage(json);
            };

            try
            {
                await _webSocket.Connect();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                OnConnectionError?.Invoke(
                    "No fue posible conectar con el servidor."
                );
            }
        }

        public async Task SendMoveAsync(
            float directionX,
            float directionY,
            float deltaTime)
        {
            if (!IsConnected)
            {
                return;
            }

            var command = new MoveCommandMessage
            {
                matchId = SessionManager.MatchId,
                directionX = directionX,
                directionY = directionY,
                deltaTime = deltaTime
            };

            var json = JsonUtility.ToJson(command);

            Debug.Log($"Sending MoveCommand: {json}");

            await _webSocket.SendText(json);
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket == null)
            {
                return;
            }

            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.Close();
            }

            _webSocket = null;
        }

        public async Task SendDoorStateAsync(string doorId,bool isClosed)
        {
            if (!IsConnected)
            {
                return;
            }

            var message = new SetDoorStateMessage
            {
                matchId = SessionManager.MatchId,
                doorId = doorId,
                isClosed = isClosed
            };

            var json = JsonUtility.ToJson(message);

            await _webSocket.SendText(json);
        }

        private void HandleMessage(string json)
        {
            var baseMessage = JsonUtility.FromJson<WebSocketMessage>(json);

            if (baseMessage == null ||
                string.IsNullOrWhiteSpace(baseMessage.type))
            {
                Debug.LogWarning($"Invalid WebSocket message: {json}");
                return;
            }

            switch (baseMessage.type)
            {
                case "PlayerMoved":
                {
                    var movementEvent =
                        JsonUtility.FromJson<PlayerMovedEvent>(json);

                    OnPlayerMoved?.Invoke(movementEvent);
                    break;
                }

                case "Error":
                {
                    var error = JsonUtility.FromJson<ErrorResponse>(json);

                    Debug.LogWarning(
                        $"Gateway rejected command: {error?.message}"
                    );

                    OnConnectionError?.Invoke(
                        error?.message ?? "El servidor rechazó la acción."
                    );

                    break;
                }
                case "MatchSnapshot":
                {
                    var snapshot =
                        JsonUtility.FromJson<MatchSnapshotEvent>(json);

                    OnMatchSnapshot?.Invoke(snapshot);
                    break;
                }
                case "DoorStateChanged":
                {
                    var doorEvent =
                        JsonUtility.FromJson<DoorStateChangedEvent>(json);

                    OnDoorStateChanged?.Invoke(doorEvent);
                    break;
                }
                case "ActiveCameraChanged":
                {
                    var cameraEvent =
                        JsonUtility.FromJson<ActiveCameraChangedEvent>(
                            json
                        );

                    OnActiveCameraChanged?.Invoke(cameraEvent);
                    break;
                }

                default:
                    Debug.LogWarning(
                        $"Unsupported WebSocket message: {baseMessage.type}"
                    );
                    break;
            }
        }
        public async Task SendChangeCameraAsync(string cameraId)
        {
            if (!IsConnected)
            {
                return;
            }

            var message = new ChangeCameraMessage
            {
                matchId = SessionManager.MatchId,
                cameraId = cameraId
            };

            var json = JsonUtility.ToJson(message);

            await _webSocket.SendText(json);
        }
        public Task CloseCameraMonitorAsync()
        {
            return SendChangeCameraAsync(null);
        }

        private async void OnDestroy()
        {
            await DisconnectAsync();
        }
    }
}