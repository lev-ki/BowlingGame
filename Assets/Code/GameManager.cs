using System;
using System.Collections.Generic;
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
            pauseReasons = new List<string>();
            rootGameState.OnEnter();
        }

        [SerializeField]
        private BaseState rootGameState;
        
        public void InvokeEvent(EventId eventId)
        {
            Debug.Log(eventId);
            rootGameState.InvokeEvent(eventId);
        }


        private List<string> pauseReasons;
        public bool IsPaused { get; private set; }

        public void SetPause(bool newPausedValue, string reason = "generic")
        {
            // try pausing
            if (newPausedValue)
            {
                if (pauseReasons.Contains(reason))
                {
                    return; // already paused with this reason
                }
                pauseReasons.Add(reason);
                if (!IsPaused)
                {
                    Time.timeScale = 0;
                    IsPaused = true;
                }
            }
            // try unpausing
            else
            {
                if (pauseReasons.Contains(reason))
                {
                    pauseReasons.Remove(reason);
                    if (pauseReasons.Count == 0)
                    {
                        Time.timeScale = 1;
                        IsPaused = false;
                    }
                }
            }
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
        BallFell,

        #endregion
        
        StartNewRound
    }
}