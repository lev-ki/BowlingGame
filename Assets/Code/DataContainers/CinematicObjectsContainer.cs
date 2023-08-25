using Code.Controls;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.DataContainers
{
    public class CinematicObjectsContainer : MonoBehaviour
    {
        #region singleton
        
        public static CinematicObjectsContainer Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate CinematicObjectsContainer found!");
            }
            Instance = this;
        }

        #endregion

        #region camera

        [Header("Camera")]
        public CustomCameraController customCameraController;
        public Animator cameraAnimator;
        public Transform cameraFollowTarget;
        public Transform cameraMenuPosition;
        public Transform cameraGameplayPosition;
        public Transform cameraScorePosition;

        #endregion

        #region pourLiquid

        [Header("Scoring Cinematic")]
        public Transform pourLiquidBottle;
        public Transform bottleRestingPosition;
        public Transform bottlePouringPosition;
        public ParticleSystem pourLiquidParticleSystemPrefab;
        public Transform particleSystemParent;

        public Renderer liquidSource;
        public Renderer liquidDestination;

        #endregion

        #region light

        [Header("Light")]
        public Light pinPositionsHighlight;

        #endregion

        [Header("Other")]
        public AudioSource poppingSoundSource;
    }
}