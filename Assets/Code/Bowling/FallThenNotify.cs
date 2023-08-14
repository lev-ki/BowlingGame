using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Bowling
{
    public class FallThenNotify : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private LayerMask floorMask;

        private const RigidbodyConstraints AllowOnlyFall = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
        
        private void OnEnable()
        {
            rb.constraints = AllowOnlyFall;
        }

        private void OnDisable()
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (enabled && (1 << other.gameObject.layer & floorMask) != 0)
            {
                GameManager.Instance.InvokeEvent(EventId.StartingPinOrBottleHitGround);
                enabled = false;
            }
        }
    
    }
}