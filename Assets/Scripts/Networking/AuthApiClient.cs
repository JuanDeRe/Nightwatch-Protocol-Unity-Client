using System;
using System.Collections;
using System.Text;
using Nightwatch.Client.Configuration;
using Nightwatch.Client.Contracts;
using UnityEngine;
using UnityEngine.Networking;

namespace Nightwatch.Client.Networking
{
    /// <summary>
    /// Sends authentication requests to AuthService.
    /// </summary>
    public class AuthApiClient
    {
        public IEnumerator Login(
            string username,
            string password,
            Action<LoginResponse> onSuccess,
            Action<string> onError)
        {
            var requestBody = new LoginRequest
            {
                username = username,
                password = password
            };

            var json = JsonUtility.ToJson(requestBody);
            var body = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest(
                $"{ApiConfiguration.AuthServiceUrl}/auth/login",
                UnityWebRequest.kHttpVerbPOST
            );

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var message = request.responseCode == 401
                    ? "Usuario o contraseña incorrectos."
                    : $"No fue posible conectar con AuthService: {request.error}";

                onError?.Invoke(message);
                yield break;
            }

            var response = JsonUtility.FromJson<LoginResponse>(
                request.downloadHandler.text
            );

            if (response == null ||
                string.IsNullOrWhiteSpace(response.token))
            {
                onError?.Invoke(
                    "AuthService devolvió una respuesta inválida."
                );

                yield break;
            }

            onSuccess?.Invoke(response);
            
        }
    }
}