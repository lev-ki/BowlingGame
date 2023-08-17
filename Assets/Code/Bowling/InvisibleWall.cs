using System;
using UnityEngine;

namespace Code.Bowling
{
    public class InvisibleWall : MonoBehaviour
    {
        [SerializeField] LayerMask bottleLayerMask;

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & bottleLayerMask) != 0)
            {
                GameManager.Instance.InvokeEvent(EventId.BottleFell);
            }
        }
    }
}