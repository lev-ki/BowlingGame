using System;
using System.Collections.Generic;
using Code.DataContainers;
using UnityEngine;
using DG.Tweening;

namespace Code.States.Cinematic
{
    [CreateAssetMenu(fileName = "CameraTransitionCinematic", menuName = "SO/GameStates/Cinematic/CameraTransitionCinematic", order = 0)]
    public class CameraTransition : BaseState
    {
        [SerializeField] private CameraLocation src;
        [SerializeField] private CameraLocation dst;
        
        public override void OnEnter()
        {
            base.OnEnter();
            _mapping[src].SrcBefore();
            _mapping[dst].DstBefore();

            var pair = new Tuple<CameraLocation, CameraLocation>(src, dst);
            if (_customActions.ContainsKey(pair))
            {
                _customActions[pair].Invoke();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            _mapping[src].SrcAfter();
            _mapping[dst].DstAfter();
        }


        [Serializable]
        public enum CameraLocation
        {
            Menu,
            Game,
            Score,
            Any
        }
        
        private static Dictionary<CameraLocation, Location> _mapping = new ()
        {
            {CameraLocation.Menu, new MenuLocation()},
            {CameraLocation.Game, new GameLocation()},
            {CameraLocation.Score, new ScoreLocation()}
        };

        #region CustomPairActions
        
        /**
         * Action for particular pair transitions are handled here. Example below.
         */
        private static Dictionary<Tuple<CameraLocation, CameraLocation>, Action> _customActions = new ()
        {
            {new(CameraLocation.Menu, CameraLocation.Game), CustomActionFromMenuToGame},
            {new(CameraLocation.Game, CameraLocation.Score), CustomActionFromGameToScore},
            {new(CameraLocation.Score, CameraLocation.Game), CustomActionFromScoreToGame},
            {new(CameraLocation.Any, CameraLocation.Menu), CustomActionFromScoreToGame},
        };
        
        private static void CustomActionFromMenuToGame()
        {
            float transitionDuration = 1f;
            float postTransitionDelay = 3f;
            var coc = CinematicObjectsContainer.Instance;
            
            coc.cameraFollowTarget.position = coc.cameraMenuPosition.position;
            coc.cameraFollowTarget.rotation = coc.cameraMenuPosition.rotation;
            coc.cameraFollowTarget.DOMove(coc.cameraGameplayPosition.position, transitionDuration);
            coc.cameraFollowTarget.DORotateQuaternion(coc.cameraGameplayPosition.rotation, transitionDuration);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(transitionDuration);
            sequence.AppendCallback(() => { GameManager.Instance.InvokeEvent(EventId.CinematicFinished); });
            sequence.AppendInterval(postTransitionDelay);
        }
        
        private static void CustomActionFromGameToScore()
        {
            float transitionDuration = 1f;
            var coc = CinematicObjectsContainer.Instance;
            DOVirtual.DelayedCall(3f, () =>
            {
                coc.cameraFollowTarget.position = coc.cameraGameplayPosition.position;
                coc.cameraFollowTarget.rotation = coc.cameraGameplayPosition.rotation;
                coc.customCameraController.SetFixedTarget(coc.cameraFollowTarget, Vector3.zero);
                coc.cameraFollowTarget.DOMove(coc.cameraScorePosition.position, transitionDuration);
                coc.cameraFollowTarget.DORotateQuaternion(coc.cameraScorePosition.rotation, transitionDuration);

                coc.liquidSource.material.SetFloat(Shader.PropertyToID("_Fill"), ProgressionContainer.Instance.currentScore);
                coc.liquidDestination.material.SetFloat(Shader.PropertyToID("_Fill"), 0);

                DOVirtual.DelayedCall(transitionDuration, () => { GameManager.Instance.InvokeEvent(EventId.CinematicFinished); });
            });
        }

        private static void CustomActionFromScoreToGame()
        {
            float transitionDuration = 1f;
            float postTransitionDelay = 3f;
            var coc = CinematicObjectsContainer.Instance;
            var bottleTransform = GameObjectsContainer.Instance.mainPlayableBottle.transform;

            coc.cameraFollowTarget.position = coc.cameraScorePosition.position;
            coc.cameraFollowTarget.rotation = coc.cameraScorePosition.rotation;
            coc.cameraFollowTarget.DOMove(coc.cameraGameplayPosition.position, transitionDuration);
            coc.cameraFollowTarget.DORotateQuaternion(coc.cameraGameplayPosition.rotation, transitionDuration);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(transitionDuration);
            sequence.AppendCallback(() => { GameManager.Instance.InvokeEvent(EventId.CinematicFinished); });
            sequence.AppendInterval(postTransitionDelay);
            sequence.AppendCallback(() => { coc.customCameraController.SetOrbitTarget(bottleTransform, new Vector3(0, 5, -12)); });
        }
        
        public static void CustomActionToMenu()
        {
            float transitionDuration = 1f;
            var coc = CinematicObjectsContainer.Instance;
            coc.cameraFollowTarget.position = coc.customCameraController.cameraParent.position;
            coc.cameraFollowTarget.rotation = coc.customCameraController.cameraParent.rotation;
            coc.customCameraController.SetFixedTarget(coc.cameraFollowTarget, Vector3.zero);
            coc.cameraFollowTarget.DOMove(coc.cameraMenuPosition.position, transitionDuration);
            coc.cameraFollowTarget.DORotateQuaternion(coc.cameraMenuPosition.rotation, transitionDuration);

            DOVirtual.DelayedCall(transitionDuration, () => { GameManager.Instance.InvokeEvent(EventId.CinematicFinished); });
        }

        #endregion
    }

    #region StartEndHandlers
    internal abstract class Location
    {
        /**
         * What should we do before starting transition from location
         */
        public virtual void SrcBefore(){}
        
        /**
         * What should we do after starting transition from location
         */
        public virtual void SrcAfter(){}
        
        /**
         * What should we do before finishing transition from location
         */
        public virtual void DstBefore(){}
        
        /**
         * What should we do after finishing transition from location
         */
        public virtual void DstAfter(){}
    }

    internal class MenuLocation : Location
    {
        public override void SrcAfter()
        {
            UIContainer.Instance.mainMenu3d.SetActive(false);
        }
        
        public override void DstBefore()
        {
            UIContainer.Instance.mainMenu3d.SetActive(true);
        }
    }

    internal class GameLocation : Location
    {
        private void DestroyBalls(float delay)
        {
            GameObjectsContainer.Instance.spotlightFollow.ClearTargets();
            for (int i = GameObjectsContainer.Instance.balls.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(GameObjectsContainer.Instance.balls[i].gameObject, delay);
                GameObjectsContainer.Instance.balls.RemoveAt(i);
            }
        }

        public override void SrcBefore()
        {
            base.SrcBefore();
            DestroyBalls(2f);
        }
        public override void SrcAfter()
        {
            UIContainer.Instance.tutorialGeneral3d.SetActive(false);
            UIContainer.Instance.tutorialSandbox3d.SetActive(false);
        }

        public override void DstBefore()
        {
            UIContainer.Instance.tutorialGeneral3d.SetActive(true);
            if (ProgressionContainer.Instance.selectedMode == ProgressionContainer.GameMode.Sandbox)
            {
                UIContainer.Instance.tutorialSandbox3d.SetActive(true);
            }
        }
    }

    internal class ScoreLocation : Location
    {
    }
    
    #endregion
}