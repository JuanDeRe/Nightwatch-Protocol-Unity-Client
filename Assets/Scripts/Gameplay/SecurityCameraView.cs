using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Displays whether a security camera is being observed.
    /// </summary>
    public class SecurityCameraView : MonoBehaviour
    {
        [SerializeField]
        private string cameraId;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Sprite inactiveSprite;

        [SerializeField]
        private Sprite activeSprite;

        public string CameraId => cameraId;

        public bool IsObserved { get; private set; }

        private void Awake()
        {
            SetObserved(false);
        }

        public void SetObserved(bool isObserved)
        {
            IsObserved = isObserved;

            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.sprite = isObserved
                ? activeSprite
                : inactiveSprite;
        }
    }
}