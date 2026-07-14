using UnityEngine;

namespace Nightwatch.Client.Gameplay
{
    /// <summary>
    /// Displays the current visual state of a security door.
    /// </summary>
    public class DoorView : MonoBehaviour
    {
        [SerializeField]
        private string doorId;

        [SerializeField]
        private Animator animator;

        public string DoorId => doorId;

        public bool IsClosed { get; private set; }

        public void SetClosed(bool isClosed)
        {
            if (IsClosed == isClosed)
            {
                return;
            }

            IsClosed = isClosed;

            if (animator != null)
            {
                animator.SetBool("IsClosed", isClosed);
            }
        }
    }
}