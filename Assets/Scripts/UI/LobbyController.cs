using Nightwatch.Client.Session;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Nightwatch.Client.Contracts;
using Nightwatch.Client.Networking;
using UnityEngine.UI;
using System;


namespace Nightwatch.Client.UI
{
    /// <summary>
    /// Controls matchmaking actions from the lobby screen.
    /// </summary>
    public class LobbyController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text welcomeText;

        [SerializeField]
        private TMP_Text sessionInfoText;

        [SerializeField]
        private TMP_InputField inviteCodeInput;

        [SerializeField]
        private TMP_Text matchmakingStatusText;

        [SerializeField]
        private TMP_Text countdownText;

        [SerializeField]
        private Button findMatchButton;

        [SerializeField]
        private Button joinByCodeButton;

        private readonly MatchmakingApiClient _matchmakingClient = new();

        private Coroutine _statusPollingCoroutine;

        private DateTime? _countdownEndsAtUtc;

        private void Start()
        {
            if (!SessionManager.IsAuthenticated)
            {
                SceneManager.LoadScene("LoginScene");
                return;
            }

            welcomeText.text =
                $"Bienvenido, {SessionManager.Username}";

            sessionInfoText.text =
                $"ID: {SessionManager.UserId}";

            countdownText.text = string.Empty;

            matchmakingStatusText.text = string.Empty;
            matchmakingStatusText.color = Color.white;
        }

        public void FindMatch()
        {
            SetButtonsEnabled(false);
            ShowStatus("Buscando partida...", Color.white);

            StartCoroutine(
                _matchmakingClient.JoinQueue(
                    SessionManager.UserId,
                    HandleJoinSuccess,
                    HandleError
                )
            );
        }

        public void JoinByCode()
        {
            var code = inviteCodeInput.text
                .Trim()
                .ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(code))
            {
                ShowStatus(
                    "Ingresa un código de invitación.",
                    Color.red
                );

                return;
            }

            SetButtonsEnabled(false);
            ShowStatus("Uniéndose a la sala...", Color.white);

            StartCoroutine(
                _matchmakingClient.JoinRoomByCode(
                    SessionManager.UserId,
                    code,
                    HandleJoinSuccess,
                    HandleError
                )
            );
        }

        public void Logout()
        {
            if (_statusPollingCoroutine != null)
            {
                StopCoroutine(_statusPollingCoroutine);
            }

            SessionManager.ClearSession();
            SceneManager.LoadScene("LoginScene");
        }

        private void HandleJoinSuccess(JoinQueueResponse response)
        {
            if (response.status == "Assigned")
            {
                HandleMatchAssigned(
                    response.matchId,
                    response.gameServiceId,
                    response.role,
                    response.animatronicType
                );

                return;
            }

            ShowStatus(
                response.message ?? "Esperando jugadores...",
                Color.white
            );

            if (_statusPollingCoroutine == null)
            {
                _statusPollingCoroutine =
                    StartCoroutine(PollMatchmakingStatus());
            }
        }

        private IEnumerator PollMatchmakingStatus()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                MatchmakingStatusResponse response = null;
                string error = null;

                yield return _matchmakingClient.GetStatus(
                    SessionManager.UserId,
                    result => response = result,
                    message => error = message
                );

                if (!string.IsNullOrWhiteSpace(error))
                {
                    ShowStatus(error, Color.red);
                    continue;
                }

                if (response == null)
                {
                    ShowStatus(
                        "El servicio devolvió una respuesta inválida.",
                        Color.red
                    );

                    continue;
                }

                // Primero comprobamos si la partida ya fue asignada.
                if (response.status == "Assigned")
                {
                    HandleMatchAssigned(
                        response.matchId,
                        response.gameServiceId,
                        response.role,
                        response.animatronicType
                    );

                    yield break;
                }

                // Si todavía no está asignado, mostramos la sala.
                matchmakingStatusText.text =
                    $"Jugadores encontrados: " +
                    $"{response.currentPlayers}/{response.maxPlayers}";

                if (!string.IsNullOrWhiteSpace(response.inviteCode))
                {
                    matchmakingStatusText.text +=
                        $"\nCódigo: {response.inviteCode}";
                }

                if (!string.IsNullOrWhiteSpace(response.countdownEndsAtUtc) &&
                    DateTime.TryParse(
                        response.countdownEndsAtUtc,
                        null,
                        System.Globalization.DateTimeStyles.RoundtripKind,
                        out var countdownEnd))
                {
                    _countdownEndsAtUtc = countdownEnd.ToUniversalTime();
                }
                else
                {
                    _countdownEndsAtUtc = null;
                    countdownText.text = "Buscando jugadores...";
                }
            }
        }


       private void HandleMatchAssigned(
            string matchId,
            string gameServiceId,
            string role,
            string animatronicType)
        {
            SessionManager.SetMatchAssignment(
                matchId,
                gameServiceId,
                role,
                animatronicType
            );
            _countdownEndsAtUtc = null;
            countdownText.text = string.Empty;

            var assignmentText = role == "Guard"
                ? "Uniéndose a la partida como vigilante."
                : $"Uniéndose como {animatronicType}.";

            ShowStatus(assignmentText, Color.green);
            StartCoroutine(LoadGameScene());
            _statusPollingCoroutine = null;
        }

        private void HandleError(string message)
        {
            SetButtonsEnabled(true);
            ShowStatus(message, Color.red);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            findMatchButton.interactable = enabled;
            joinByCodeButton.interactable = enabled;
        }

        private void ShowStatus(string message, Color color)
        {
            matchmakingStatusText.text = message;
            matchmakingStatusText.color = color;
        }
        private void Update()
        {
            if (_countdownEndsAtUtc is null)
            {
                return;
            }

            var remaining = _countdownEndsAtUtc.Value - DateTime.UtcNow;

            if (remaining.TotalSeconds > 0)
            {
                countdownText.text =
                    $"La partida inicia en {Mathf.CeilToInt((float)remaining.TotalSeconds)} s";
            }
            else
            {
                countdownText.text = "Uniéndose a la partida...";
            }
        }
        private IEnumerator LoadGameScene()
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("GameScene");
        }
    }
}