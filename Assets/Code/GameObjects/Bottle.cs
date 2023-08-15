using Code.Bowling;
using Code.Controls;
using UnityEngine;

namespace Code.GameObjects
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField] private GameObject intactBottle;
        [SerializeField] private GameObject brokenBottlePrefab;
        [SerializeField] private float bottleBreakPiecesScatterForce = 5f;

        private GameObject brokenBottleInstance;
        #region otherComponents

        public Rigidbody rb;
        
        public FallThenNotify fallThenNotify;
        public SpillLiquid spillLiquid;
        public BottleBreakController bottleBreakController;

        #endregion

        #region controls

        [SerializeField] private PinKeyboardControls keyboardControls;
        [SerializeField] private PinDragMovementControl dragMovementControl;

        public bool IsBroken { get; private set; }
        
        public void EnableControls()
        {
            keyboardControls.enabled = true;
            dragMovementControl.enabled = true;
        }
        
        public void DisableControls()
        {
            keyboardControls.enabled = false;
            dragMovementControl.enabled = false;
        }
        
        #endregion

        public void DestroyPieces(float delay)
        {
            if (brokenBottleInstance)
            {
                Destroy(brokenBottleInstance, delay);
                brokenBottleInstance = null;
            }
        }

        public void ResetBottle()
        {
            rb.isKinematic = false;
            DestroyPieces(0);
            intactBottle.SetActive(true);
            IsBroken = false;
        }

        public void OnBreakBottle(Vector3 breakImpulse)
        {
            if (IsBroken)
            {
                return;
            }
            rb.isKinematic = true;
            Destroy(brokenBottleInstance);
            brokenBottleInstance = Instantiate(brokenBottlePrefab, transform.position, transform.rotation, transform.parent);
            for (int i = 0; i < brokenBottleInstance.transform.childCount; i++)
            {
                Transform child = brokenBottleInstance.transform.GetChild(i);
                Rigidbody childRB = child.GetComponent<Rigidbody>();
                if (childRB)
                {
                    childRB.AddForce((Random.insideUnitSphere + Vector3.up * Random.Range(0, 1))* bottleBreakPiecesScatterForce);
                    childRB.AddForce(-breakImpulse * 0.1f, ForceMode.Impulse);
                }
            }
            intactBottle.SetActive(false);
            GameManager.Instance.InvokeEvent(EventId.BottleBroken);
            IsBroken = true;
            spillLiquid.LiquidLevel = 0;
        }
    }
}