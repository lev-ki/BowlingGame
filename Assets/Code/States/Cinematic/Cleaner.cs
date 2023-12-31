﻿using Code.DataContainers;
using System.Collections;
using UnityEngine;

namespace Code.States.Cinematic
{
    [CreateAssetMenu(fileName = "CleanerCinematic", menuName = "SO/GameStates/Cinematic/Cleaner", order = 0)]
    public class Cleaner : BaseState
    {
        private Coroutine cinematicCoroutine;
        
        public override void OnEnter()
        {
            // TODO (Lev)
            cinematicCoroutine = GameManager.Instance.StartCoroutine(CinematicCoroutine());
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            GameManager.Instance.StopCoroutine(cinematicCoroutine);
        }

        private void DestroyBalls(float delay)
        {
            GameObjectsContainer.Instance.spotlightFollow.ClearTargets();
            for (int i = GameObjectsContainer.Instance.balls.Count - 1; i >= 0; i--)
            {
                Destroy(GameObjectsContainer.Instance.balls[i].gameObject, delay);
                GameObjectsContainer.Instance.balls.RemoveAt(i);
            }
        }

        private IEnumerator CinematicCoroutine()
        {
            yield return new WaitForSeconds(3f);
            GameObjectsContainer.Instance.cleaner.StartCleaning();
            yield return new WaitForSeconds(2.5f);
            UIContainer.Instance.UIController.FadeToBlack(0.75f, true);
            GameObjectsContainer.Instance.mainPlayableBottle.bottleBreakController.DestroyPieces(0.76f);
            yield return new WaitForSeconds(0.3f);
            DestroyBalls(0.2f);
            yield return new WaitForSeconds(0.2f);
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
        }
    }
}