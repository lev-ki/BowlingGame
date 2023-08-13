using System;
using Code.States;
using UnityEngine;

namespace Code
{
    public class GameManager : MonoBehaviour
    {
        #region singleton
        
        public static GameManager Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate GameManager found!");
            }
            Instance = this;
        }

        #endregion

        private void Start()
        {
            rootGameState.OnEnter();
        }

        [SerializeField]
        private BaseState rootGameState;
        
        public void InvokeEvent(EventId eventId)
        {
            Debug.Log(eventId);
            rootGameState.InvokeEvent(eventId);
        }
    }
    
    [Serializable]
    public enum EventId
    {
        #region transition 
        // menu events
        MenuPlayClicked,
        MenuSandboxClicked,
        
        // game events
        PinsAndBottleSet,
        BottleDrained,
        BottleFell,
        BottleBroken,
        AllBallsFell,
        AllRoundsFinished,
        
        // pause events
        PauseMenuSelected,
        
        // scoring events
        ScoringNextLevelSelected,
        ScoringMenuSelected,
        
        // cinematic events
        CinematicFinished,

        #endregion

        #region other

        StartingPinOrBottleHitGround,
        ReadyToStartBalls,
        BallFell

        #endregion

    }
}