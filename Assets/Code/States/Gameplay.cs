using Code.DataContainers;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "Gameplay", menuName = "SO/GameStates/Gameplay", order = 0)]
    public class Gameplay : BaseState
    {
        public override void OnEnter()
        {
            RefillBottle();
            ShowGameplayUI();
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            HideGameplayUI();
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
                    if (ProgressionContainer.Instance.CurrentLevel.rounds.Count - 1 == ProgressionContainer.Instance.CurrentRoundIndex)
                    {
                        ProgressionContainer.Instance.currentScore = GameObjectsContainer.Instance.mainPlayableBottle.spillLiquid.LiquidLevel;
                        // TODO(Alex): level increment in scoring
                        GameManager.Instance.InvokeEvent(EventId.AllRoundsFinished);
                        // return to prevent new round from starting
                        return;
                    }
                    // in case that's not the last round
                    ProgressionContainer.Instance.CurrentRoundIndex += 1;
                    
                    ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle |= ProgressionContainer.Instance.CurrentRound.resetBottle;
                    break;
                case EventId.BottleDrained:
                    RefillBottle();
                    break;
                case EventId.BottleFell:
                    RefillBottle();
                    break;
            }
            base.InvokeEvent(eventId);
        }
    }
}