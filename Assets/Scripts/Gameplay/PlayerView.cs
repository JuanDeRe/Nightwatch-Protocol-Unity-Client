using TMPro;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Represents a synchronized player in the scene.
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerNameText;

        [SerializeField]
        private float interpolationSpeed = 12f;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Transform cameraTarget;

        public Transform CameraTarget => cameraTarget != null ? cameraTarget : transform;

        private Vector3 _previousPosition;

        private bool _hasPowerUp;

        private Vector3 _targetPosition;

        public string PlayerId { get; private set; } = string.Empty;

        public string Role { get; private set; } = string.Empty;

        public string AnimatronicType { get; private set; } =
            string.Empty;

        public void Initialize(
            string playerId,
            string role,
            string animatronicType,
            Vector3 initialPosition,
            bool isLocalPlayer)
        {
            PlayerId = playerId;
            Role = role;
            AnimatronicType = animatronicType;
            _previousPosition = initialPosition;    
            transform.position = initialPosition;
            _targetPosition = initialPosition;

            if (playerNameText != null)
            {
                playerNameText.gameObject.SetActive(!isLocalPlayer);

                if (!isLocalPlayer)
                {
                    playerNameText.text = playerId;
                }
            }
        

            // El personaje local se distingue ligeramente.
            var spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        public void SetAuthoritativePosition(Vector3 position)
        {
            _targetPosition = position;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                _targetPosition,
                interpolationSpeed * Time.deltaTime
            );

            if (animator == null || Role != "Animatronic")
            {
                return;
            }

            var distanceMoved = Vector3.Distance(
                transform.position,
                _previousPosition
            );

            var isMoving = distanceMoved > 0.0001f;

            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("HasPowerUp", _hasPowerUp);

            _previousPosition = transform.position;
        }
        public void SetPowerUpState(bool hasPowerUp)
        {
            _hasPowerUp = hasPowerUp;
        }
        public void SetGuardLeftControl(bool active)
        {
            if (animator == null || Role != "Guard")
            {
                return;
            }

            animator.SetBool("UsingLeftControl", active);

            if (active)
            {
                animator.SetBool("UsingRightControl", false);
            }
        }

        public void SetGuardRightControl(bool active)
        {
            if (animator == null || Role != "Guard")
            {
                return;
            }

            animator.SetBool("UsingRightControl", active);

            if (active)
            {
                animator.SetBool("UsingLeftControl", false);
            }
        }
    }
}