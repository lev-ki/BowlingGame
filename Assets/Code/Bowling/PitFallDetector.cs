using System;
using UnityEngine;

namespace Code.Bowling
{
    public class PitFallDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask ballMask;
        [SerializeField] private LayerMask pinMask;
        [SerializeField] private LayerMask bottleMask;
        
        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & ballMask) != 0)
            {
                GameManager.Instance.InvokeEvent(EventId.BallFell);
                // ball despawn logic
            }
            if ((1 << other.gameObject.layer & pinMask) != 0)
            {
                // pin despawn logic
            }
            if ((1 << other.gameObject.layer & bottleMask) != 0)
            {
                GameManager.Instance.InvokeEvent(EventId.BottleFell);
            }
        }
    }
}