using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nightwatch.Client.DebugTools
{
    /// <summary>
    /// Converts mouse positions over the map into server coordinates.
    /// </summary>
    public class MapCoordinateProbe : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer mapRenderer;

        [SerializeField]
        private TMP_Text coordinateText;

        [SerializeField]
        private float serverMapWidth = 50f;

        [SerializeField]
        private float serverMapHeight = 60f;

        private void Update()
        {
            var mouse = Mouse.current;

            if (mouse == null || mapRenderer == null)
            {
                return;
            }

            var screenPosition = mouse.position.ReadValue();

            var worldPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    -Camera.main.transform.position.z
                )
            );

            var bounds = mapRenderer.bounds;

            if (!bounds.Contains(
                    new Vector3(
                        worldPosition.x,
                        worldPosition.y,
                        bounds.center.z
                    )))
            {
                if (coordinateText != null)
                {
                    coordinateText.text =
                        "Cursor fuera del mapa";
                }

                return;
            }

            var normalizedX = Mathf.InverseLerp(
                bounds.min.x,
                bounds.max.x,
                worldPosition.x
            );

            var normalizedY = Mathf.InverseLerp(
                bounds.min.y,
                bounds.max.y,
                worldPosition.y
            );

            var serverX = normalizedX * serverMapWidth;
            var serverY = normalizedY * serverMapHeight;

            if (coordinateText != null)
            {
                coordinateText.text =
                    $"Servidor: {serverX:F2}, {serverY:F2}";
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                Debug.Log(
                    $"Map coordinate: X={serverX:F2}, Y={serverY:F2}"
                );
            }
        }
    }
}