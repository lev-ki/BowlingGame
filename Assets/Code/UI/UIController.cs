using Code.DataContainers;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Code.Menu
{
    public class UIController : MonoBehaviour
    {
        public enum OptionType { Play, Sandbox, Settings, Credits, Quit, };
        private RaycastTarget currentlySelected;

        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject credits;

        [SerializeField] private Camera mainCamera;

        [SerializeField] private Image blackFadeImage;

        private bool allowGameplayActions = false;
        public bool AllowGameplayActions
        {
            get => allowGameplayActions;
            set
            {
                allowGameplayActions = value;
            }
        }

        private void Update()
        {
            if (InputManager.Instance.PauseInputActivated())
            {
                if (allowGameplayActions)
                {
                    OpenPausePanel();
                }
            }
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
            quitPanel.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void DenyQuit()
        {
            quitPanel.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
        }

        public void ConfirmQuit()
        {
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
            // todo
        }

        public void CloseSettings()
        {
            // todo
        }

        public void ShowCredits()
        {
            credits.SetActive(true);
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseCredits()
        {
            credits.SetActive(false);
            InputManager.Instance.block3DRaycast = false;
        }

        public void OpenTutorial()
        {
            UIContainer.Instance.tutorialPanel.SetActive(true);
            GameManager.Instance.SetPause(true, "tutorial");
            InputManager.Instance.block3DRaycast = true;
        }

        public void CloseTutorial()
        {
            UIContainer.Instance.tutorialPanel.SetActive(false);
            if (!UIContainer.Instance.ftuxCompleted)
            {
                UIContainer.Instance.ftuxCompleted = true;
            }
            GameManager.Instance.SetPause(false, "tutorial");
            InputManager.Instance.block3DRaycast = false;
        }

        public void OpenPausePanel()
        {
            UIContainer.Instance.pausePanel.SetActive(true);
            GameManager.Instance.SetPause(true, "uiPause");
            InputManager.Instance.block3DRaycast = true;
        }

        public void ClosePausePanel()
        {
            UIContainer.Instance.pausePanel.SetActive(false);
            GameManager.Instance.SetPause(false, "uiPause");
            InputManager.Instance.block3DRaycast = false;
        }

        public void ToMainMenu()
        {
            // TODO(anyone): make this work
            //GameManager.Instance.InvokeEvent(EventId.PauseMenuSelected);
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