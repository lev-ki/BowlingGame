using Code.Bowling;
using Code.Controls;
using UnityEngine;

namespace Code.GameObjects
{
    public class Bottle : MonoBehaviour
    {
        #region otherComponents

        public Rigidbody rb;
        
        public FallThenNotify fallThenNotify;
        public SpillLiquid spillLiquid;
        public BottleBreakController bottleBreakController;
        public PopIntoExistence popIntoExistence;

        #endregion

        #region controls

        public PinKeyboardControls keyboardControls;
        public PinDragMovementControl dragMovementControl;

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
    }
}