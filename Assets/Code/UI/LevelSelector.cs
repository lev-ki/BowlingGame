using Code.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button playButton;
        [SerializeField] private Image star1, star2, star3;

        private int level;

        public void InitializeSelector(int index)
        {
            level = index;
            levelText.text = $"Level {level + 1}";
            playButton.onClick.AddListener(SelectLevel);
            SetScore(0);
        }

        public void SetScore(float score)
        {
            float star1Brightness = Mathf.InverseLerp(0, 1 / 3.0f, score);
            float star2Brightness = Mathf.InverseLerp(1 / 3.0f, 2 / 3.0f, score);
            float star3Brightness = Mathf.InverseLerp(2 / 3.0f, 1, score);

            Color star1Color = star1.color;
            Color star2Color = star2.color;
            Color star3Color = star3.color;

            star1Color.a = star1Brightness;
            star2Color.a = star2Brightness;
            star3Color.a = star3Brightness;

            star1.color = star1Color;
            star2.color = star2Color;
            star3.color = star3Color;
        }

        private void SelectLevel()
        {
            UIContainer.Instance.UIController.CloseLevelSelection(level);
        }
    }
}