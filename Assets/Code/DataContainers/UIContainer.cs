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

        public GameObject gameName3d;
        public GameObject tutorialGeneral3d;
        public GameObject tutorialSandbox3d;
        public GameObject mainMenu3d;
        
        public LiquidLevelImage liquidLevelImage;
        
        #endregion

        #region logic

        public UIController UIController;

        #endregion

        #region scorePanel
        
        public GameObject scorePanel;
        public Image star1, star2, star3;
        public AudioSource neonAudio;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI scorePanelButtonText;
        public TextMeshProUGUI scorePanelCurrentLevelText;

        #endregion
        
        #region hud
        
        public GameObject hud;
        
        public TextMeshProUGUI currentLevelText;
        public TextMeshProUGUI currentRoundText;
        public TextMeshProUGUI maxRoundText;

        public GameObject tutorialPanel;

        public GameObject pausePanel;

        public bool ftuxCompleted = false;

        #endregion
    }
}