using System;
using UnityEngine.Serialization;

[Serializable]
public struct GameData
{
    public bool gameStarted;
    public int ballsPresent;
    
    public int pinsPresent;
    
    public bool liquidRanOut;
    public bool hasBottleFell;
    public bool hasBottleTouchedAWall;
    public float maxBottleAngle;
    
    /**
     * managed by cleaner
     */
    public bool isCleanerPresent;
        
    public int roundNumber;
    
    public int restartsCount;
    
    public bool isSandboxMode;
    
    public bool persistBottle;
}