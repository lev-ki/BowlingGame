using System.Collections;
using Code.DataContainers;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "ScoreScreen", menuName = "SO/GameStates/ScoreScreen", order = 0)]
    public class ScoreScreen : BaseState
    {
        private Coroutine scorePanelCoroutine;
        
        public override void OnEnter()
        {
            UIContainer.Instance.scorePanel.SetActive(true);
            scorePanelCoroutine = GameManager.Instance.StartCoroutine(
                ScoresPanelAnimation(ProgressionContainer.Instance.currentScore));
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            GameManager.Instance.StopCoroutine(scorePanelCoroutine);
            UIContainer.Instance.scorePanel.SetActive(false);
        }
        
        public void ScorePanelButtonCallback()
        {
            if (ProgressionContainer.Instance.CurrentLevelIndex == ProgressionContainer.Instance.levels.Count - 1)
            {
                GameManager.Instance.InvokeEvent(EventId.ScoringMenuSelected);
            }
            else
            {
                GameManager.Instance.InvokeEvent(EventId.ScoringNextLevelSelected);
                ProgressionContainer.Instance.CurrentLevelIndex += 1;
                ProgressionContainer.Instance.CurrentRoundIndex = 0;
            }
        }

        private IEnumerator ScoresPanelAnimation(float score)
        {
            var ui = UIContainer.Instance;
            float scaledScore = score * 100;
            float star1Brightness = Mathf.InverseLerp(0, 1/3.0f, score);
            float star2Brightness = Mathf.InverseLerp(1/3.0f, 2/3.0f, score);
            float star3Brightness = Mathf.InverseLerp(2/3.0f, 1, score);
            float timePassed = 0;

            ui.currentLevelText.text = (ProgressionContainer.Instance.CurrentLevelIndex + 1).ToString();
            if (ProgressionContainer.Instance.CurrentLevelIndex == ProgressionContainer.Instance.levels.Count - 1)
            {
                ui.scorePanelButtonText.text = "To Menu";
            }
            ui.scoreText.text = ((int)scaledScore).ToString();
            ui.neonAudio.Play();
            Color star1Color = ui.star1.color;
            Color star2Color = ui.star2.color;
            Color star3Color = ui.star3.color;

            while (timePassed < 2f)
            {
                float randomTime = Random.Range(0.05f, 0.2f);
                float alpha = Random.Range(0f, 1f);
                star1Color.a = alpha * star1Brightness;
                star2Color.a = alpha * star2Brightness;
                star3Color.a = alpha * star3Brightness;
                ui.star1.color = star1Color;
                ui.star2.color = star2Color;
                ui.star3.color = star3Color;
                yield return new WaitForSeconds(randomTime);
                timePassed += randomTime;
            }
            star1Color.a = star1Brightness;
            star2Color.a = star2Brightness;
            star3Color.a = star3Brightness;
            ui.star1.color = star1Color;
            ui.star2.color = star2Color;
            ui.star3.color = star3Color;
        }
    }
}