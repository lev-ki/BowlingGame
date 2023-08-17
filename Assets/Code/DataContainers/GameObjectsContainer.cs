using System.Collections.Generic;
using Code.Bowling;
using Code.GameObjects;
using UnityEngine;

namespace Code.DataContainers
{
    public class GameObjectsContainer : MonoBehaviour
    {
        #region singleton
        
        public static GameObjectsContainer Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate GameObjectsContainer found!");
            }
            Instance = this;
        }
        
        #endregion
        
        public List<Ball> balls = new();
        public Bottle mainPlayableBottle;
        
        public List<Transform> startingPinPositions;
        
        public Transform pinsParent;
        public Transform sandboxPinsParent;
        
        public BallLauncher ballLauncher;

        public PitFallDetector pitFallDetector;

        public Cleaner cleaner;

        public SpotlightFollow spotlightFollow;
        
        public Vector3 bottleHideout;
    }
}