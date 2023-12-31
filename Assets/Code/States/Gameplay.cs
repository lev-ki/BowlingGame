﻿using Code.DataContainers;
using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "Gameplay", menuName = "SO/GameStates/Gameplay", order = 0)]
    public class Gameplay : BaseState
    {
        private Tween winCallback;
        
        public override void OnEnter()
        {
            // here might go some save/load logic
            // this sets ui values in some images
            ProgressionContainer.Instance.CurrentLevelIndex = ProgressionContainer.Instance.CurrentLevelIndex;
            
            UIContainer.Instance.UIController.allowGameplayActions = true;
            if ( !UIContainer.Instance.ftuxCompleted )
            {
                UIContainer.Instance.UIController.OpenTutorial();
            }
            RefillBottle();
            ShowGameplayUI();
            CinematicObjectsContainer.Instance.musicSource.Stop();
            CinematicObjectsContainer.Instance.musicSource.clip = CinematicObjectsContainer.Instance.gameplayMusic;
            CinematicObjectsContainer.Instance.musicSource.Play();
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            HideGameplayUI();
            UIContainer.Instance.UIController.allowGameplayActions = false;
        }

        private void ShowGameplayUI()
        {
            UIContainer.Instance.liquidLevelImage.Show();
            UIContainer.Instance.hud.SetActive(true);
        }

        private void HideGameplayUI()
        {
            UIContainer.Instance.liquidLevelImage.Hide();
            UIContainer.Instance.hud.SetActive(false);
        }

        private void RefillBottle()
        {
            ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle = true;
            ProgressionContainer.Instance.runtimeBottleRoundStartOptions.refillBottle = true;
        }

        public override void InvokeEvent(EventId eventId)
        {
            switch (eventId)
            {
                case EventId.AllBallsFell:
                    winCallback = DOVirtual.DelayedCall(2, () =>
                    {
                        ProgressionContainer.Instance.roundScores[ProgressionContainer.Instance.CurrentRoundIndex] = GameObjectsContainer.Instance.mainPlayableBottle.spillLiquid.LiquidLevel;
                        if (ProgressionContainer.Instance.CurrentLevel.rounds.Count - 1 == ProgressionContainer.Instance.CurrentRoundIndex)
                        {
                            ProgressionContainer.Instance.currentScore = ProgressionContainer.Instance.roundScores.Aggregate(0.0f, (sum, pair) => sum + pair.Value) / ProgressionContainer.Instance.CurrentLevel.rounds.Count;
                            ProgressionContainer.Instance.roundScores = new();
                            GameManager.Instance.InvokeEvent(EventId.AllRoundsFinished);
                            return;
                        }
                        // in case that's not the last round
                        UIContainer.Instance.UIController.ShowRoundResult($"Stage {ProgressionContainer.Instance.CurrentRoundIndex + 1} completed!");
                        GameObjectsContainer.Instance.mainPlayableBottle.spillLiquid.LiquidLevel = 1;
                        ProgressionContainer.Instance.CurrentRoundIndex += 1;

                        ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle |= ProgressionContainer.Instance.CurrentRound.resetBottle;

                        GameManager.Instance.InvokeEvent(EventId.StartNewRound);
                    }, false);
                    // return to prevent new round from starting
                    return;
                case EventId.BottleDrained:
                    Fail();
                    break;
                case EventId.BottleFell:
                    Fail();
                    break;
                case EventId.BottleBroken:
                    Fail();
                    break;
            }
            base.InvokeEvent(eventId);
        }

        private void Fail()
        {
            winCallback.Kill();
            if (ProgressionContainer.Instance.CurrentLevel.failRestartsLevel)
            {
                ProgressionContainer.Instance.CurrentRoundIndex = 0;
            }
            RefillBottle();
        }
    }
}