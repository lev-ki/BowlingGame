﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public enum OptionType { Start, Sandbox, Settings, Credits, Quit, };
    [SerializeField] CustomCameraController mainCamera;
    RaycastTarget currentlySelected;

    public Animator CameraAnimator;
    public AudioSource musicSource;
    public AudioClip menuMusic, gameplayMusic;

    public GameObject RaycastBlocker;
    public GameObject MenuParent;
    public GameObject QuitPanel;
    public GameObject PauseScreen;
    public GameObject CreditsGO;
    public GameObject Tutorial;
    public GameObject ScoresPanel;
    public Image Star1, Star2, Star3;
    public AudioSource NeonAudio;
    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI Scores;
    public TextMeshProUGUI NextLevelText;

    public Transform BottleHideout;
    public Transform MenuTarget;

    public PourLiquidToScore pourLiquidToScore;

    public bool inGameplay = false;
    public bool isPaused = false;
    public bool block3dRaycast = false;

    private Coroutine gameManagerCoroutine;
    private Coroutine cameraToMenuCoroutine;

    private bool lastLevel = false;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inGameplay)
            {
                TogglePause(!isPaused);
            }
        }
        if (block3dRaycast)
        {
            return;
        }
        if (Physics.Raycast(ray, out hit, 100))
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
                if (Input.GetMouseButtonDown(0))
                {
                    switch (target.OptionType)
                    {
                        case OptionType.Start:
                            StartGame();
                            break;
                        case OptionType.Sandbox:
                            StartSandbox();
                            break;
                        case OptionType.Settings:
                            Settings();
                            break;
                        case OptionType.Credits:
                            Credits();
                            break;
                        case OptionType.Quit:
                            Quit();
                            break;
                    }
                }
            }
        }
    }

    public void OpenTutorial()
    {
        Time.timeScale = 0.0001f;
        Tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        Time.timeScale = 1;
        Tutorial.SetActive(false);
    }

    public void UnblockRaycast()
    {
        block3dRaycast = false;
    }

    public void TogglePause(bool state)
    {
        isPaused = state;
        Time.timeScale = isPaused ? 0.0001f : 1;
        PauseScreen.SetActive(isPaused);
    }

    public void Quit()
    {
        RaycastBlocker.SetActive(true);
        QuitPanel.SetActive(true);
        block3dRaycast = true;
    }

    public void DenyQuit()
    {
        RaycastBlocker.SetActive(false);
        QuitPanel.SetActive(false);
        block3dRaycast = false;
    }

    public void ConfirmQuit()
    {
        QuitPanel.SetActive(true);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
        // do nothing here
#else
        Application.Quit();
#endif
    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        inGameplay = true;
        cameraToMenuCoroutine = StartCoroutine(TranslateCameraToGame());
        gameManagerCoroutine = StartCoroutine(GameManager.Instance.SetupGame(false, 2));
        musicSource.clip = gameplayMusic;
        musicSource.Play();
    }

    [ContextMenu("StartSandbox")]
    public void StartSandbox()
    {
        inGameplay = true;
        cameraToMenuCoroutine = StartCoroutine(TranslateCameraToGame());
        gameManagerCoroutine = StartCoroutine(GameManager.Instance.SetupGame(true, 2));
        musicSource.clip = gameplayMusic;
        musicSource.Play();
    }

    public void Settings()
    {
        // todo
    }

    public void Credits()
    {
        RaycastBlocker.SetActive(true);
        CreditsGO.SetActive(true);
        block3dRaycast = true;
    }

    [ContextMenu("ShowMainMenu")]
    public void ShowMainMenu()
    {
        inGameplay = false;
        isPaused = false;
        PauseScreen.SetActive(false);
        StartCoroutine(TranslateCameraToMenu());
        //musicSource.clip = menuMusic;
        musicSource.Stop();
    }

    public void GameplayToScore(float score)
    {
        StartCoroutine(TranslateCameraToScore(score));
    }

    public void ScoreToGameplay()
    {
        StartCoroutine(TranslateCameraScoreToGame());
    }

    private IEnumerator TranslateCameraToScore(float score)
    {
        pourLiquidToScore.Setup(score);
        CameraAnimator.SetTrigger("ToScore");
        //MenuParent.SetActive(false);
        //yield return new WaitForSeconds(5f);
        mainCamera.currentTarget = MenuTarget;
        mainCamera.secondaryTarget = MenuTarget;
        mainCamera.defaultOrbitTarget = MenuTarget;
        mainCamera.cameraOffset = Vector3.zero;// new Vector3(0, 5, -12);
        mainCamera.currentCameraMode = CustomCameraController.CameraMode.Fixed;
        mainCamera.futureCameraMode = CustomCameraController.CameraMode.Fixed;
        mainCamera.targetCamera.transform.position = mainCamera.transform.position;
        GameManager.Instance.bottle.dragControl.timer = -1;
        yield return new WaitForSeconds(2f);
        pourLiquidToScore.ShowScore(score);
    }

    private IEnumerator TranslateCameraScoreToGame()
    {
        CameraAnimator.SetTrigger("ScoreToGameplay");
        MenuParent.SetActive(false);
        yield return new WaitForSeconds(5f);
        mainCamera.currentTarget = GameManager.Instance.bottle.transform;
        mainCamera.secondaryTarget = GameManager.Instance.bottle.transform;
        mainCamera.defaultOrbitTarget = GameManager.Instance.bottle.transform;
        mainCamera.cameraOffset = new Vector3(0, 5, -12);
        mainCamera.currentCameraMode = CustomCameraController.CameraMode.Orbit;
        mainCamera.futureCameraMode = CustomCameraController.CameraMode.Orbit;
        GameManager.Instance.bottle.dragControl.timer = 2f;
    }

    private IEnumerator TranslateCameraToMenu()
    {
        CameraAnimator.SetTrigger("ToMenu");
        //MenuParent.SetActive(true);
        mainCamera.currentTarget = MenuTarget;
        mainCamera.secondaryTarget = MenuTarget;
        mainCamera.defaultOrbitTarget = MenuTarget;
        mainCamera.cameraOffset = Vector3.zero;// new Vector3(0, 5, -12);
        mainCamera.currentCameraMode = CustomCameraController.CameraMode.Fixed;
        mainCamera.futureCameraMode = CustomCameraController.CameraMode.Fixed;
        mainCamera.targetCamera.transform.position = mainCamera.transform.position;
        GameManager.Instance.bottle.dragControl.timer = -1;
        GameManager.Instance.Cleanup(gameManagerCoroutine);
        if (cameraToMenuCoroutine != null) StopCoroutine(cameraToMenuCoroutine);
        GameManager.Instance.bottle.transform.position = BottleHideout.position;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator TranslateCameraToGame()
    {
        CameraAnimator.SetTrigger("ToGameplay");
        MenuParent.SetActive(false);
        yield return new WaitForSeconds(5f);
        mainCamera.currentTarget = GameManager.Instance.bottle.transform;
        mainCamera.secondaryTarget = GameManager.Instance.bottle.transform;
        mainCamera.defaultOrbitTarget = GameManager.Instance.bottle.transform;
        mainCamera.cameraOffset = new Vector3(0, 5, -12);
        mainCamera.currentCameraMode = CustomCameraController.CameraMode.Orbit;
        mainCamera.futureCameraMode = CustomCameraController.CameraMode.Orbit;
        GameManager.Instance.bottle.dragControl.timer = 2f;
        OpenTutorial();
    }

    public void WinGame()
    {
        Debug.Log("You won");
    }

    public void OpenScoresPanel(float score)
    {
        ScoresPanel.SetActive(true);
        StartCoroutine(ScoresPanelAnimation(score));
    }

    private IEnumerator ScoresPanelAnimation(float score)
    {
        float scaledScore = score * 10 / 9;

        float star1Brightness = Mathf.InverseLerp(0, 30, score);
        float star2Brightness = Mathf.InverseLerp(30, 60, score);
        float star3Brightness = Mathf.InverseLerp(60, 90, score);
        float timePassed = 0;

        CurrentLevel.text = (GameManager.Instance.currentLevelIndex + 1).ToString();
        if (GameManager.Instance.currentLevelIndex == GameManager.Instance.levels.Count - 1)
        {
            lastLevel = true;
            NextLevelText.text = "To Menu";
        }
        Scores.text = ((int)scaledScore).ToString();
        NeonAudio.Play();
        Color star1Color = Star1.color;
        Color star2Color = Star2.color;
        Color star3Color = Star3.color;

        while (timePassed < 2f)
        {
            float randomTime = Random.Range(0.05f, 0.2f);
            float alpha = Random.Range(0f, 1f);
            star1Color.a = alpha * star1Brightness;
            star2Color.a = alpha * star2Brightness;
            star3Color.a = alpha * star3Brightness;
            Star1.color = star1Color;
            Star2.color = star2Color;
            Star3.color = star3Color;
            yield return new WaitForSeconds(randomTime);
            timePassed += randomTime;
        }
        star1Color.a = star1Brightness;
        star2Color.a = star2Brightness;
        star3Color.a = star3Brightness;
        Star1.color = star1Color;
        Star2.color = star2Color;
        Star3.color = star3Color;
    }

    public void CloseScoresPanel()
    {
        ScoresPanel.SetActive(false);
        if (lastLevel)
        {
            ShowMainMenu();
            return;
        }
        GameManager.Instance.ScoringCompleted();
    }
}