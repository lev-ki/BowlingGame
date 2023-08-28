using Code.Menu;
using Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.DataContainers
{
    public class UIContainer : MonoBehaviour
    {
        #region singleton
        
        public static UIContainer Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate UIContainer found!");
            }
            Instance = this;
        }
        
        #endregion

        #region visuals

        [Header("Visuals")]
        public GameObject gameName3d;
        public GameObject tutorialGeneral3d;
        public GameObject tutorialSandbox3d;
        public GameObject mainMenu3d;
        
        public LiquidLevelImage liquidLevelImage;

        #endregion

        #region logic

        [Header("Logic")]
        public UIController UIController;

        #endregion

        #region scorePanel

        [Header("Score Panel")]
        public GameObject scorePanel;
        public Image star1, star2, star3;
        public AudioSource neonAudio;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI scorePanelButtonText;
        public TextMeshProUGUI scorePanelCurrentLevelText;
        public GameObject summaryPanel;
        public TextMeshProUGUI summaryScoreText;

        #endregion

        #region hud

        [Header("HUD")]
        public GameObject hud;
        
        public TextMeshProUGUI currentLevelText;
        public TextMeshProUGUI currentRoundText;
        public TextMeshProUGUI maxRoundText;
        public TextMeshProUGUI maxLevelText;

        public GameObject tutorialPanel;

        public GameObject pausePanel;

        public bool ftuxCompleted = false;

        #endregion

        #region settings

        [Header("Settings")]
        public Settings settings;
        public GameObject settingsPanel;
        public TextMeshProUGUI resolutionText;
        public TextMeshProUGUI fullscreenText;

        #endregion

        #region returnToLane

        [Header("Return To Lane")]
        public TextMeshProUGUI returnTime;
        public GameObject returnToLane;

        #endregion

        #region levelSelection

        [Header("Level Selection")]
        public LevelSelection levelSelection;

        #endregion
    }
}