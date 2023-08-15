using Code.DataContainers;
using UnityEngine;

namespace Code.Menu
{
    public class MenuController : MonoBehaviour
    {
        public enum OptionType { Play, Sandbox, Settings, Credits, Quit, };
        private RaycastTarget currentlySelected;
        
        [SerializeField] private GameObject raycastBlocker;
        [SerializeField] private GameObject quitPanel;
        [SerializeField] private GameObject credits;

        private bool Block3dRaycast
        {
            get => raycastBlocker.activeSelf;
            set => raycastBlocker.SetActive(value);
        }
        
        [SerializeField] private Camera mainCamera;

        private bool allowGameplayActions = false;
        public bool AllowGameplayActions => allowGameplayActions;
        
        private void Update()
        {
            if (InputManager.Instance.PauseInputActivated())
            {
                if (allowGameplayActions)
                {
                    //TogglePause(!isPaused);
                }
            }
            Ray ray = mainCamera.ScreenPointToRay(InputManager.Instance.cursorPosition);
            if (Block3dRaycast)
            {
                return;
            }
            if (Physics.Raycast(ray, out var hit, 100))
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
            Block3dRaycast = true;
        }

        public void DenyQuit()
        {
            quitPanel.SetActive(false);
            Block3dRaycast = false;
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
            Block3dRaycast = true;
        }
        
        public void CloseCredits()
        {
            credits.SetActive(false);
            Block3dRaycast = false;
        }

        public void OpenTutorial()
        {
            UIContainer.Instance.tutorialPanel.SetActive(true);
        }

        public void CloseTutorial()
        {
            UIContainer.Instance.tutorialPanel.SetActive(false);
            if ( !UIContainer.Instance.ftuxCompleted )
            {
                UIContainer.Instance.ftuxCompleted = true;
            }
        }

        public void OpenPausePanel()
        {
            UIContainer.Instance.pausePanel.SetActive(true);
        }

        public void ClosePausePanel()
        {
            UIContainer.Instance.pausePanel.SetActive(false);
        }
    }
}