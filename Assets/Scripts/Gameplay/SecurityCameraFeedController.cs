using System.Collections.Generic;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Moves the surveillance camera to the selected feed point.
    /// </summary>
    public class SecurityCameraFeedController : MonoBehaviour
    {
        [SerializeField]
        private Camera surveillanceCamera;

        [SerializeField]
        private List<SecurityCameraFeedPoint> feedPoints = new();

        private readonly Dictionary<string, SecurityCameraFeedPoint>
            _feedPointsById = new();

        private void Awake()
        {
            foreach (var feedPoint in feedPoints)
            {
                if (feedPoint == null ||
                    string.IsNullOrWhiteSpace(feedPoint.CameraId))
                {
                    continue;
                }

                _feedPointsById[feedPoint.CameraId] =
                    feedPoint;
            }

            if (surveillanceCamera != null)
            {
                surveillanceCamera.gameObject.SetActive(false);
            }
        }

        public void ShowFeed(string cameraId)
        {
            if (surveillanceCamera == null)
            {
                Debug.LogError(
                    "Surveillance Camera is not assigned."
                );

                return;
            }

            if (!_feedPointsById.TryGetValue(
                    cameraId,
                    out var feedPoint))
            {
                Debug.LogWarning(
                    $"Feed point '{cameraId}' was not found."
                );

                return;
            }

            var cameraTransform =
                surveillanceCamera.transform;

            cameraTransform.position = new Vector3(
                feedPoint.transform.position.x,
                feedPoint.transform.position.y,
                cameraTransform.position.z
            );

            surveillanceCamera.orthographicSize =
                feedPoint.OrthographicSize;

            surveillanceCamera.gameObject.SetActive(true);
        }

        public void HideFeed()
        {
            if (surveillanceCamera != null)
            {
                surveillanceCamera.gameObject.SetActive(false);
            }
        }
    }
}