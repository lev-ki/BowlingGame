using System;
using System.Collections.Generic;
using Code.DataContainers;
using UnityEngine;

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
            
            /* TODO(Lev): StartCoroutine and handle transition
             * Different transitions are different scriptable objects (SO) in unity so it's
             * generally possible to add smth like Animation field and assign different
             * values for each SO. It's up to you.
             * Also, do not forget to call
             * GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
             * In parent game states (root for most cases) it'll be a mapping to catch this and actually
             * transition to the next state
             */
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
            Score
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
            {new(CameraLocation.Score, CameraLocation.Game), CustomActionFromMenuToGame},
        };
        
        private static void CustomActionFromMenuToGame()
        {
            // TODO(Lev): redo! this is only for testing purpose
            // we might want to wait some time after firing CinematicFinished event before returning camera control to bottle
            // so it won't be camera jumping to bottle hideout. Another way is to move this set to ResetPins
            var coc = CinematicObjectsContainer.Instance;
            var bottleTransform = GameObjectsContainer.Instance.mainPlayableBottle.transform;
            coc.customCameraController.currentTarget = bottleTransform;
            coc.customCameraController.secondaryTarget = bottleTransform;
            coc.customCameraController.defaultOrbitTarget = bottleTransform;
            coc.customCameraController.cameraOffset = new Vector3(0, 5, -12);
            coc.customCameraController.currentCameraMode = CustomCameraController.CameraMode.Orbit;
            coc.customCameraController.futureCameraMode = CustomCameraController.CameraMode.Orbit;
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
        }
        
        private static void CustomActionFromGameToScore()
        {
            // TODO(Lev): redo! this is only for testing purpose
            // we might want to wait some time after firing CinematicFinished event before returning camera control to bottle
            // so it won't be camera jumping to bottle hideout. Another way is to move this set to ResetPins
            var coc = CinematicObjectsContainer.Instance;
            coc.cameraAnimator.SetTrigger("ToScore");
            coc.customCameraController.currentTarget = coc.cameraMenuTarget;
            coc.customCameraController.secondaryTarget = coc.cameraMenuTarget;
            coc.customCameraController.defaultOrbitTarget = coc.cameraMenuTarget;
            coc.customCameraController.cameraOffset = Vector3.zero;// new Vector3(0, 5, -12);
            coc.customCameraController.currentCameraMode = CustomCameraController.CameraMode.Fixed;
            coc.customCameraController.futureCameraMode = CustomCameraController.CameraMode.Fixed;
            coc.customCameraController.targetCamera.transform.position = coc.customCameraController.transform.position;
            GameManager.Instance.InvokeEvent(EventId.CinematicFinished);
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