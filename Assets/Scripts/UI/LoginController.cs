using Nightwatch.Client.Contracts;
using Nightwatch.Client.Networking;
using Nightwatch.Client.Session;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nightwatch.Client.UI
{
    /// <summary>
    /// Handles login form validation and authentication.
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField usernameInput;

        [SerializeField]
        private TMP_InputField passwordInput;

        [SerializeField]
        private Button loginButton;

        [SerializeField]
        private TMP_Text statusText;

        private readonly AuthApiClient _authApiClient = new();

        private void Start()
        {
            statusText.text = string.Empty;
            statusText.color = Color.white;
        }

        public void Login()
        {
            var username = usernameInput.text.Trim();
            var password = passwordInput.text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowError("Ingresa usuario y contraseña.");
                return;
            }

            loginButton.interactable = false;
            statusText.color = Color.white;
            statusText.text = "Iniciando sesión...";

            StartCoroutine(
                _authApiClient.Login(
                    username,
                    password,
                    HandleLoginSuccess,
                    HandleLoginError
                )
            );
        }

        private void HandleLoginSuccess(LoginResponse response)
        {
            SessionManager.StartSession(
                response.token,
                response.userId,
                response.username,
                response.role
            );

            statusText.color = Color.white;
            statusText.text = "Sesión iniciada.";

            SceneManager.LoadScene("LobbyScene");
        }

        private void HandleLoginError(string message)
        {
            loginButton.interactable = true;
            ShowError(message);
        }

        private void ShowError(string message)
        {
            statusText.text = message;
            statusText.color = Color.red;
        }
    
    }
}