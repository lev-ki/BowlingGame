using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControl", menuName = "Progression/PlayerControl", order = 0)]
public class Round : ScriptableObject
{
    public int ballsNumber = 1;
    public float ballSpeedMultiplier = 1;
    public float ballLaunchDelay = 0.2f;
    public List<float> ballIntermediateLaunchDelays;
    public bool restartRoundOnly;
    
    public bool resetPins;
    public bool resetBottle;
    
    public bool fallTriggersCleaner = true;
    public bool wallTouchIsFall;
    
    public float spillThreshold = 20;
    public float spillSpeed = 1;
    public float spillMaxThreshold = 80;
    public bool liquidRanOutTriggersCleaner = true;
}