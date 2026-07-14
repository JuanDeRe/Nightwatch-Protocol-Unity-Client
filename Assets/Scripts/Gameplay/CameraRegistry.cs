using System.Collections.Generic;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Updates the active security camera indicator.
    /// </summary>
    public class SecurityCameraRegistry : MonoBehaviour
    {
        [SerializeField]
        private List<SecurityCameraView> cameras = new();

        private readonly Dictionary<string, SecurityCameraView>
            _camerasById = new();

        private void Awake()
        {
            foreach (var securityCamera in cameras)
            {
                if (securityCamera == null ||
                    string.IsNullOrWhiteSpace(
                        securityCamera.CameraId))
                {
                    continue;
                }

                _camerasById[securityCamera.CameraId] =
                    securityCamera;
            }
        }

        public void SetActiveCamera(string activeCameraId)
        {
            foreach (var pair in _camerasById)
            {
                var isActive =
                    pair.Key == activeCameraId;

                pair.Value.SetObserved(isActive);
            }
        }
    }
}