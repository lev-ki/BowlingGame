using DG.Tweening;
using UnityEngine;

namespace Code.Bowling
{
    public class BallStuck : MonoBehaviour
    {
        [SerializeField] private LayerMask ballMask;
        private const string FallenTag = "FallenBall";
        
        private void OnTriggerEnter(Collider other)
        {
            DOVirtual.DelayedCall(10f, () =>
            {
                if (other == null || other.CompareTag(FallenTag) || (1 << other.gameObject.layer & ballMask) == 0)
                {
                    return;
                }
                GameManager.Instance.InvokeEvent(EventId.BallFell);
                other.gameObject.tag = FallenTag;
            });
        }
    }
}