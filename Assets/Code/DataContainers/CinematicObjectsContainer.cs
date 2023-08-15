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
        
        public CustomCameraController customCameraController;
        public Animator cameraAnimator;
        public Transform cameraFollowTarget;
        public Transform cameraMenuPosition;
        public Transform cameraGameplayPosition;
        public Transform cameraScorePosition;

        #endregion

        #region pourLiquid

        public Animator pourLiquidAnimator;
        public Renderer jugWater;
        
        #endregion

    }
}