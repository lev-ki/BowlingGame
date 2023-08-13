﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.DataContainers
{
    public class ProgressionContainer : MonoBehaviour
    {
        #region singleton
        
        public static ProgressionContainer Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate ProgressionContainer found!");
            }
            Instance = this;
        }
        
        #endregion
        
        public List<Level> levels;
        [SerializeField] private int currentLevelIndex;
        [SerializeField] private int currentRoundIndex;
        
        public Level CurrentLevel => levels[CurrentLevelIndex];
        public Round CurrentRound => CurrentLevel.rounds[CurrentRoundIndex];

        public int CurrentLevelIndex
        {
            get => currentLevelIndex;
            set
            {
                UIContainer.Instance.currentLevelText.text = (value + 1).ToString();
                UIContainer.Instance.scorePanelCurrentLevelText.text = (value + 1).ToString();
                currentLevelIndex = value;
                UIContainer.Instance.maxRoundText.text = levels[value].rounds.Count.ToString();
            }
        }

        public int CurrentRoundIndex
        {
            get => currentRoundIndex;
            set
            {
                UIContainer.Instance.currentRoundText.text = (value + 1).ToString();
                currentRoundIndex = value;
            }
        }

        public float currentScore;
        
        [Serializable]
        public struct RuntimeBottleRoundStartOptions
        {
            public bool refillBottle;
            public bool resetBottle;
        }
        public RuntimeBottleRoundStartOptions runtimeBottleRoundStartOptions; 
        
        public enum GameMode {Levels, Sandbox}
        
        public GameMode selectedMode;
    }
}