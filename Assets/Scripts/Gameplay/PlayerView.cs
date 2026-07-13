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

        private Vector3 _targetPosition;

        public string PlayerId { get; private set; } = string.Empty;

        public void Initialize(
            string playerId,
            Vector3 initialPosition,
            bool isLocalPlayer)
        {
            PlayerId = playerId;
            transform.position = initialPosition;
            _targetPosition = initialPosition;

            if (playerNameText != null)
            {
                playerNameText.text = isLocalPlayer
                    ? $"{playerId} (Tú)"
                    : playerId;
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = isLocalPlayer
                    ? Color.green
                    : Color.red;
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
        }
    }
}