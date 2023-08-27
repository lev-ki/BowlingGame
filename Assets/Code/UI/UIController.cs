using System;
using Code.DataContainers;
using Code.GameObjects;
using Code.States.Cinematic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

        private void Start()
        {
            UIContainer.Instance.maxLevelText.text = ProgressionContainer.Instance.levels.Count.ToString();
        }

        private void Update()
        {
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
                                ProgressionContainer.Instance.selectedMode = ProgressionContainer.GameMode.Levels;
                                GameManager.Instance.InvokeEvent(EventId.MenuPlayClicked);
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
                DOVirtual.DelayedCall(duration + 0.5f, ()=> { FadeFromBlack(duration); });
            }
        }

        public void FadeFromBlack(float duration)
        {
            blackFadeImage.DOFade(0, duration);
        }
    }
}