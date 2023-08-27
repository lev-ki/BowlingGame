using System.Collections.Generic;
using UnityEngine;

namespace Code.Progression
{
    [CreateAssetMenu(fileName = "Level", menuName = "Progression/Level", order = 0)]
    public class Level : ScriptableObject
    {
        public List<Round> rounds;
    
        public bool shouldBeRandomBottlePosition = true;
        public int bottlePosition = 5;
        public bool failRestartsLevel;
    
        /**
         * pin positions to be set
         * includes the bottle position
         */
        public List<int> pinPositions;
    }
}