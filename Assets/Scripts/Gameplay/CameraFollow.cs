using Nightwatch.Client.Session;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Follows the local player without rotating the camera.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private PlayerRegistry playerRegistry;

        [SerializeField]
        private float followSpeed = 8f;

        private Transform _target;

        private void LateUpdate()
        {
            if (_target == null)
            {
                var localPlayer = playerRegistry.GetPlayer(
                    SessionManager.UserId
                );

                if (localPlayer == null)
                {
                    return;
                }

                _target = localPlayer.CameraTarget;
            }

            var desiredPosition = new Vector3(
                _target.position.x,
                _target.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime
            );
        }
    }
}