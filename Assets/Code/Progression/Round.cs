using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Progression
{
    [CreateAssetMenu(fileName = "Round", menuName = "Progression/Round", order = 0)]
    public class Round : ScriptableObject
    {
        public float ballLaunchDelay = 0.2f;
        
        public List<BallLaunch> launches;

        public bool resetPins;
        [Obsolete] public bool resetBottle;

        public float spillThreshold = 20;
        public float spillSpeed = 1;
        
        public float roundEndTimer = 30;

        [Serializable]
        public class BallLaunch
        {
            [Tooltip("Overrides Shooting Angle Horizontal")]
            public bool shouldTargetBottle;
            [Range(-1, 1),
             Tooltip("-1 means shooting to the left side of the lane (pin distance). 1 is for the right side")]
            public float shootingAngleHorizontal;
            [Range(-1, 1),
             Tooltip("-1 - left side of the lane. 1 - right side")]
            public float spawnPositionHorizontal;

            public float preDelay;
            public float postDelay;
            
            public float ballSpeed = 1;
        }
    }
}