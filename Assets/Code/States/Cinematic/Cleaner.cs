using System.Collections;
using UnityEngine;

namespace Code.States.Cinematic
{
    [CreateAssetMenu(fileName = "CleanerCinematic", menuName = "SO/GameStates/Cinematic/Cleaner", order = 0)]
    public class Cleaner : BaseState
    {
        public override void OnEnter()
        {
            // TODO (Lev)
            GameManager.Instance.StartCoroutine(CinematicCoroutine());
            base.OnEnter();
        }
        
        private IEnumerator CinematicCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
        }
    }
}