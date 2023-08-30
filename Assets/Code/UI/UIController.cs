using System;
using Code.DataContainers;
using Code.GameObjects;
using Code.States.Cinematic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.Bowling;

namespace Code.Menu
{
    public class UIController : MonoBehaviour
    {
        public enum OptionType { Play, Sandbox, Settings, Credits, Quit, };
        private RaycastTarget currentlySelected;

        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject credits;

        [SerializeField] private Camera mainCamera;

        [SerializeField] private Image blackFadeImage;
        [SerializeField] private RectTransform chargeIndicator;

        [SerializeField] private GameObject popupBackground;

        public bool allowGameplayActions;

        public bool showReturnToLaneTimer;
        private bool returnToLaneTimerVisible = false;
        private Tween laneTextAlphaTween;

        private Sequence roundResultSequence;

        private void Start()
        {
            UIContainer.Instance.maxLevelText.text = ProgressionContainer.Instance.levels.Count.ToString();
            UIContainer.Instance.levelSelection.Initialize();
        }

        public void ShowRoundResult(string text)
        {
            if (roundResultSequence != null)
            {
                roundResultSequence.Kill();
            }
            UIContainer.Instance.roundResultText.text = text;
            roundResultSequence = DOTween.Sequence()
                .Append(DOTween.To(() => UIContainer.Instance.roundResult.alpha, (x) => UIContainer.Instance.roundResult.alpha = x, 1, 0.25f))
                .AppendInterval(2f)
                .Append(DOTween.To(() => UIContainer.Instance.roundResult.alpha, (x) => UIContainer.Instance.roundResult.alpha = x, 0, 1f));
        }

        private void ToggleReturToLaneTimer(bool newState)
        {
            if (newState == returnToLaneTimerVisible)
            {
                return;
            }

            if (laneTextAlphaTween != null)
            {
                laneTextAlphaTween.Kill();
            }
            returnToLaneTimerVisible = newState;
            laneTextAlphaTween = DOTween.To(()=> UIContainer.Instance.returnToLane.alpha, (x)=> UIContainer.Instance.returnToLane.alpha = x, newState ? 1 : 0, 0.25f);
        }

        private void Update()
        {
            ToggleReturToLaneTimer(showReturnToLaneTimer && InvisibleWall.TryShowTimer);

            if (InputManager.Instance.PauseInputActivated())
            {
                if (IsPausePanelOpened())
                {
                    ClosePausePanel();
                }
                else if (allowGameplayActions)
                {
                    OpenPausePanel();
                }
            }

            Vector2 movePos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                InputManager.Instance.cursorPosition, canvas.worldCamera,
                out movePos);

            chargeIndicator.position = canvas.transform.TransformPoint(movePos);

            Ray ray = mainCamera.ScreenPointToRay(InputManager.Instance.cursorPosition);

            if (!InputManager.Instance.block3DRaycast && Physics.Raycast(ray, out var hit, 100))
            {
                RaycastTarget target = hit.transform.gameObject.GetComponent<RaycastTarget>();

                if (target)
                {
                    if (target != currentlySelected)
                    {
                        if (currentlySelected)
                        {
                            currentlySelected.Toggle(false);
                        }
                        target.Toggle(true);
                        currentlySelected = target;
                    }
                    if (InputManager.Instance.PrimaryActionActivated())
                    {
                        switch (target.optionType)
                        {
                            case OptionType.Play:
                                if (ProgressionContainer.Instance.levelSelectionUnlocked)
                                {
                                    OpenLevelSelection();
                                }
                                else
                                {
                                    PlayLevelFromStart(0);
                                }
                                break;
                            case OptionType.Sandbox:
                                ProgressionContainer.Instance.selectedMode = ProgressionContainer.GameMode.Sandbox;
                                GameManager.Instance.InvokeEvent(EventId.MenuSandboxClicked);
                                break;
                            case OptionType.Settings:
                                ShowSettings();
                                break;
                            case OptionType.Credits:
                                ShowCredits();
                                break;
                            case OptionType.Quit:
                                Quit();
                                break;
                        }
                    }
                }
            }
        }

        public void OpenSummaryPanel()
        {
            popupBackground.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
            UIContainer.Instance.summaryPanel.SetActive(true);
            UIContainer.Instance.summaryScoreText.text = $"{(int)ProgressionContainer.Instance.TotalScore} points!";
        }

        public void CloseSummaryPanel()
        {
            popupBackground.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
            UIContainer.Instance.summaryPanel.SetActive(false);
        }

        private void OpenLevelSelection()
        {
            popupBackground.SetActive(true);
            UIContainer.Instance.levelSelection.gameObject.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseLevelSelection(int selectedLevel = -1)
        {
            popupBackground.SetActive(false);
            UIContainer.Instance.levelSelection.gameObject.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
            if (selectedLevel >= 0)
            {
                PlayLevelFromStart(selectedLevel);
            }
        }

        public void PlayLevelFromStart(int level = 0)
        {
            PlayLevel(level, 0);
        }

        private void PlayLevel(int level, int round)
        {
            if (level >= 0)
            {
                ProgressionContainer.Instance.CurrentLevelIndex = Mathf.Min(level, ProgressionContainer.Instance.levels.Count - 1);
            }

            ProgressionContainer.Instance.CurrentRoundIndex = Mathf.Clamp(round, 0, ProgressionContainer.Instance.CurrentLevel.rounds.Count - 1);

            ProgressionContainer.Instance.selectedMode = ProgressionContainer.GameMode.Levels;
            GameManager.Instance.InvokeEvent(EventId.MenuPlayClicked);
        }

        public void Quit()
        {
            popupBackground.SetActive(true);
            quitPanel.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void DenyQuit()
        {
            popupBackground.SetActive(false);
            quitPanel.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
        }

        public void ConfirmQuit()
        {
            popupBackground.SetActive(false);
            quitPanel.SetActive(true);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
        // do nothing here
#else
        Application.Quit();
#endif
        }

        public void ShowSettings()
        {
            popupBackground.SetActive(true);
            UIContainer.Instance.settingsPanel.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseSettings()
        {
            popupBackground.SetActive(false);
            UIContainer.Instance.settingsPanel.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
            // todo
        }

        public void ShowCredits()
        {
            popupBackground.SetActive(true);
            credits.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseCredits()
        {
            popupBackground.SetActive(false);
            credits.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
        }

        public void OpenTutorial()
        {
            popupBackground.SetActive(true);
            UIContainer.Instance.tutorialPanel.SetActive(true);
            GameManager.Instance.SetPause(true, "tutorial");
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseTutorial()
        {
            popupBackground.SetActive(false);
            UIContainer.Instance.tutorialPanel.SetActive(false);
            if (!UIContainer.Instance.ftuxCompleted)
            {
                UIContainer.Instance.ftuxCompleted = true;
            }
            GameManager.Instance.SetPause(false, "tutorial");
            InputManager.Instance.block3DRaycast = false;
        }
        
        public bool IsPausePanelOpened()
        {
            return UIContainer.Instance.pausePanel.activeSelf;
        }

        public void OpenPausePanel()
        {
            popupBackground.SetActive(true);
            UIContainer.Instance.pausePanel.SetActive(true);
            GameManager.Instance.SetPause(true, "uiPause");
            InputManager.Instance.block3DRaycast = true;
        }

        public void ClosePausePanel()
        {
            popupBackground.SetActive(false);
            UIContainer.Instance.pausePanel.SetActive(false);
            GameManager.Instance.SetPause(false, "uiPause");
            InputManager.Instance.block3DRaycast = false;
        }

        public void ToMainMenu()
        {
            CameraTransition.CustomActionToMenu();
            var goc = GameObjectsContainer.Instance;
            goc.mainPlayableBottle.transform.position = goc.bottleHideout;
            goc.mainPlayableBottle.fallThenNotify.enabled = false;
            foreach (Transform pin in goc.pinsParent)
            {
                Destroy(pin.gameObject);
            }

            for (var i = goc.balls.Count - 1; i >= 0; i--)
            {
                var ball = goc.balls[i];
                Destroy(ball.gameObject);
            }

            GameManager.Instance.InvokeEvent(EventId.PauseMenuSelected);
        }

        public void FadeToBlack(float duration, bool fadeOutAfter = true)
        {
            blackFadeImage.DOFade(1, duration);
            if (fadeOutAfter)
            {
                DOVirtual.DelayedCall(duration + 0.5f, ()=> { FadeFromBlack(duration); }, false);
            }
        }

        public void FadeFromBlack(float duration)
        {
            blackFadeImage.DOFade(0, duration);
        }
    }
}