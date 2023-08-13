using System.Collections;
using UnityEngine;

namespace Code.States.Cinematic
{
    [CreateAssetMenu(fileName = "PourLiquidCinematic", menuName = "SO/GameStates/Cinematic/PourLiquid", order = 0)]
    public class PourLiquid : BaseState
    {
        public override void OnEnter()
        {
            GameManager.Instance.StartCoroutine(PourLiquidCinematicCoroutine());
            base.OnEnter();
        }

        private IEnumerator PourLiquidCinematicCoroutine()
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
        }
    }
}