using System.Collections;
using Code.DataContainers;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "PlayerControl", menuName = "SO/GameStates/PlayerControl", order = 0)]
    public class PlayerControl : BaseState
    {
        public override void OnEnter()
        {
            EnablePlayerControls();
            base.OnEnter();
            GameManager.Instance.InvokeEvent(EventId.ReadyToStartBalls);
        }

        public override void OnExit()
        {
            base.OnExit();
            DisablePlayerControls();
        }

        #region controls

        private void EnablePlayerControls()
        {
            GameObjectsContainer.Instance.mainPlayableBottle.EnableControls();
            // TODO: think about enabling/disabling rotation
        }

        private void DisablePlayerControls()
        {
            GameObjectsContainer.Instance.mainPlayableBottle.DisableControls();
        }

        #endregion
    }
}