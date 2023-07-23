using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Progression/Level", order = 0)]
public class Level : ScriptableObject
{
    public List<Round> rounds;
    
    public bool shouldBeRandomBallPosition = true;
    public int ballPosition = 5;
    
    /**
     * pin positions to be set
     * includes the ball position
     */
    public List<int> pinPositions;
}