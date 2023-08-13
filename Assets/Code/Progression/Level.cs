using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Level", menuName = "Progression/Level", order = 0)]
public class Level : ScriptableObject
{
    public List<Round> rounds;
    
    public bool shouldBeRandomBallPosition = true;
    public int bottlePosition = 5;
    
    /**
     * pin positions to be set
     * includes the bottle position
     */
    public List<int> pinPositions;
}