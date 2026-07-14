using System.Collections.Generic;
using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Finds and updates the security doors in the scene.
    /// </summary>
    public class DoorRegistry : MonoBehaviour
    {
        [SerializeField]
        private List<DoorView> doors = new();

        private readonly Dictionary<string, DoorView> _doorsById = new();

        private void Awake()
        {
            foreach (var door in doors)
            {
                if (door == null ||
                    string.IsNullOrWhiteSpace(door.DoorId))
                {
                    continue;
                }

                _doorsById[door.DoorId] = door;
            }
        }

        public void SetDoorState(
            string doorId,
            bool isClosed)
        {
            if (!_doorsById.TryGetValue(
                    doorId,
                    out var door))
            {
                Debug.LogWarning(
                    $"Door '{doorId}' was not found in the scene."
                );

                return;
            }

            door.SetClosed(isClosed);
        }
        public bool TryGetDoorState(
            string doorId,
            out bool isClosed)
        {
            if (_doorsById.TryGetValue(
                    doorId,
                    out var door))
            {
                isClosed = door.IsClosed;
                return true;
            }

            isClosed = false;
            return false;
        }
    }
}