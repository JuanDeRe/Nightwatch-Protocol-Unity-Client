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
    /// Sends queue and lobby requests to MatchmakingService.
    /// </summary>
    public class MatchmakingApiClient
    {
        public IEnumerator JoinQueue(
            string playerId,
            Action<JoinQueueResponse> onSuccess,
            Action<string> onError)
        {
            var body = new JoinQueueRequest
            {
                playerId = playerId
            };

            yield return SendPost(
                "/matchmaking/join",
                JsonUtility.ToJson(body),
                onSuccess,
                onError
            );
        }

        public IEnumerator JoinRoomByCode(
            string playerId,
            string inviteCode,
            Action<JoinQueueResponse> onSuccess,
            Action<string> onError)
        {
            var body = new JoinRoomByCodeRequest
            {
                playerId = playerId,
                inviteCode = inviteCode
            };

            yield return SendPost(
                "/matchmaking/rooms/join-by-code",
                JsonUtility.ToJson(body),
                onSuccess,
                onError
            );
        }

        public IEnumerator GetStatus(
            string playerId,
            Action<MatchmakingStatusResponse> onSuccess,
            Action<string> onError)
        {
            var url =
                $"{ApiConfiguration.MatchmakingServiceUrl}" +
                $"/matchmaking/status/{playerId}";

            using var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(
                    $"No fue posible consultar matchmaking: {request.error}"
                );

                yield break;
            }

            var response =
                JsonUtility.FromJson<MatchmakingStatusResponse>(
                    request.downloadHandler.text
                );

            onSuccess?.Invoke(response);
        }

        private IEnumerator SendPost(
            string path,
            string json,
            Action<JoinQueueResponse> onSuccess,
            Action<string> onError)
        {
            var body = Encoding.UTF8.GetBytes(json);
            var url = ApiConfiguration.MatchmakingServiceUrl + path;

            using var request = new UnityWebRequest(
                url,
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
                onError?.Invoke(
                    $"Error de matchmaking: {request.downloadHandler.text}"
                );

                yield break;
            }

            var response = JsonUtility.FromJson<JoinQueueResponse>(
                request.downloadHandler.text
            );

            onSuccess?.Invoke(response);
        }
    }
}