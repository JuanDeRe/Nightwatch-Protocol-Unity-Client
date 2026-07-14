using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Defines the world position and size of a security camera feed.
    /// </summary>
    public class SecurityCameraFeedPoint : MonoBehaviour
    {
        [SerializeField]
        private string cameraId;

        [SerializeField]
        private float orthographicSize = 3f;

        public string CameraId => cameraId;

        public float OrthographicSize => orthographicSize;
    }
}