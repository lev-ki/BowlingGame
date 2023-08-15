using Code.DataContainers;
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

        private void DestroyBalls(float delay)
        {
            for (int i = GameObjectsContainer.Instance.balls.Count - 1; i >= 0; i--)
            {
                Destroy(GameObjectsContainer.Instance.balls[i].gameObject, delay);
                GameObjectsContainer.Instance.balls.RemoveAt(i);
            }
        }

        private IEnumerator CinematicCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            Debug.Log("Cleaner Cinematic here");
            DestroyBalls(0.2f);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
        }
    }
}