using System.Collections;
using Code.DataContainers;
using Code.GameObjects;
using DG.Tweening;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "Round", menuName = "SO/GameStates/Round", order = 0)]
    public class Round : BaseState
    {
        private const string FallenTag = "FallenBall";
        private Coroutine ballsCoroutine;
        private Tween roundTimer;
        private int ballsPresent;
        private bool allBallsThrown;
        private bool allBallsFellCalled;

        public override void OnEnter()
        {
            base.OnEnter();
            GameObjectsContainer.Instance.pitFallDetector.gameObject.SetActive(true);
            GameObjectsContainer.Instance.invisibleWalls.SetActive(true);
            allBallsFellCalled = false;
            //DestroyBalls(0);
        }

        public override void OnExit()
        {
            base.OnExit();
            if ( ballsCoroutine != null)
            {
                GameManager.Instance.StopCoroutine(ballsCoroutine);
            }
            //DestroyBalls(2f); // 2 sec delay so that the ball doesn't disappear immediately after lose condition
            GameObjectsContainer.Instance.pitFallDetector.gameObject.SetActive(false);
            GameObjectsContainer.Instance.invisibleWalls.SetActive(false);
            GameObjectsContainer.Instance.mainPlayableBottle.dragMovementControl.Timer = 0;
            roundTimer.Kill();
        }

        public override void InvokeEvent(EventId eventId)
        {
            base.InvokeEvent(eventId);
            switch (eventId)
            {
                case EventId.ReadyToStartBalls:
                    OnStartBalls();
                    break;
                case EventId.BallFell:
                    OnBallFell();
                    break;
            }
        }

        private void OnBallFell()
        {
            ballsPresent -= 1;
            Debug.Log($"{ballsPresent} balls left");
            if (allBallsThrown && ballsPresent == 0 && !allBallsFellCalled)
            {
                allBallsFellCalled = true;
                roundTimer.Kill();
                GameManager.Instance.InvokeEvent(EventId.AllBallsFell);
            }
        }

        private void OnStartBalls()
        {
            foreach(Ball ball in GameObjectsContainer.Instance.balls)
            {
                ball.gameObject.tag = FallenTag;
            }

            ballsCoroutine = GameManager.Instance.StartCoroutine(BallsCoroutine());
            roundTimer = DOVirtual.DelayedCall(ProgressionContainer.Instance.CurrentRound.roundEndTimer, () =>
            {
                if (allBallsFellCalled)
                {
                    return;
                }
                allBallsFellCalled = true;
                GameManager.Instance.InvokeEvent(EventId.AllBallsFell);
            }, false);
        }

        private IEnumerator BallsCoroutine()
        {
            allBallsThrown = false;
            ballsPresent = 0;
            var round = ProgressionContainer.Instance.CurrentRound;
            yield return new WaitForSeconds(round.ballLaunchDelay);
            GameObjectsContainer.Instance.spotlightFollow.ClearTargets();
            foreach (var launch in round.launches)
            {
                yield return new WaitForSeconds(launch.preDelay);
                LaunchBall(launch);
                yield return new WaitForSeconds(launch.postDelay);
            }
            allBallsThrown = true;
        }
        
        private void LaunchBall(Progression.Round.BallLaunch launch)
        {
            GameObjectsContainer.Instance.ballLauncher.Launch(launch);
            ballsPresent += 1;
        }
    }
}