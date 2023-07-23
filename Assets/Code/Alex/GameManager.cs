using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Bottle bottle;
    
    [SerializeField] private GameObject liquidLevelBar;

    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private Transform ballsContainer;
    
    [SerializeField] private GameObject sandboxPins;
    [SerializeField] private GameObject sandboxHints;

    [SerializeField] private Transform[] startingPinPositions;
    [SerializeField] private Transform pinContainer;
    [SerializeField] private GameObject pinPrefab;
    
    [SerializeField] private Cleaner cleanerPrefab;
    [SerializeField] private Transform cleanerContainer;

    public List<Level> levels;
    public int currentLevelIndex;

    public Level currentLevel;
    public Round currentRound;
    public GameData gameData;
    public MenuController menuController;

    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI CurrentRound;
    public TextMeshProUGUI MaxRounds;

    private Coroutine nextLevelCoroutine;
    
    private float gameSetupTimeout;

    public IEnumerator SetupGame(bool isSandbox, float delay = 0.1f)
    {
        if (gameSetupTimeout >= 0)
        {
            yield break;
        }
        gameSetupTimeout = 1.5f;
        gameData.isSandboxMode = isSandbox;
        bottle.rb.isKinematic = false;
        if (isSandbox)
        {
            var reset = bottle.GetComponent<Reset>();
            var position = new Vector3(6.5f, 0.5f, 7);
            bottle.rb.MovePosition(position);
            reset.SetDefaults(position, Quaternion.identity);
            reset.useKey = true;
            ballLauncher.useKey = true;
            sandboxPins.SetActive(true);
            sandboxHints.SetActive(true);
            gameData.gameStarted = true;
            yield break;
        }
        
        liquidLevelBar.gameObject.SetActive(true);

        currentRound = currentLevel.rounds[gameData.roundNumber];
        if (currentRound.resetPins)
        {
            DestroyPins();
        }
        if (gameData.roundNumber == 0)
        {
            DestroyPins();
        }
        DestroyBalls();
        DestroyCleaner();
        if (gameData.roundNumber == 0)
        {
            gameData.persistBottle = false;
        }
        yield return new WaitForSeconds(delay);
        
        gameData.gameStarted = true;

        var bottlePosition = currentLevel.shouldBeRandomBallPosition ? Random.Range(0, 9) : currentLevel.ballPosition;
        
        yield return SetupBowlingPins(bottlePosition);
        
        yield return new WaitForSeconds(2);
        
        gameData.persistBottle = true;
        gameData.liquidRanOut = false;
        gameData.hasBottleTouchedAWall = false;
        gameData.maxBottleAngle = 0;
        
        gameData.ballsPresent = 0;

        yield return new WaitForSeconds(currentRound.ballLaunchDelay);
        if (gameData.roundNumber == 0)
        {
            bottle.liquidLevel = 1;
            bottle.liquidLevelImage.fillAmount = 1;
        }
        for (int i = 0; i < currentRound.ballsNumber; i++)
        {
            if (gameData.isCleanerPresent)
            {
                yield break;
            }
            LaunchBall();
            float launchDelay = currentRound.ballLaunchDelay;
            if (i < currentRound.ballIntermediateLaunchDelays.Count - 1)
            {
                launchDelay = currentRound.ballIntermediateLaunchDelays[i];
            }
            yield return new WaitForSeconds(launchDelay);
        }
    }

    private IEnumerator SetupBowlingPins(int bottlePosition)
    {
        if (!currentRound.resetPins && gameData.roundNumber != 0)
        {
            yield break;
        }
        foreach (int pinPosition in currentLevel.pinPositions)
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 fallOffset = Vector3.up * (10 + Random.value);

            if (pinPosition == bottlePosition)
            {
                if (gameData.persistBottle)
                {
                    continue;
                }
                Vector3 transformPosition = startingPinPositions[pinPosition].position + fallOffset;
                bottle.rb.velocity = Vector3.zero;
                bottle.rb.angularVelocity = Vector3.zero;
                bottle.rb.MovePosition(transformPosition);
                bottle.rb.MoveRotation(Quaternion.identity);
                continue;
            }

            Instantiate(pinPrefab, startingPinPositions[pinPosition].position + fallOffset, Quaternion.identity, pinContainer);
            gameData.pinsPresent += 1;
        }
    }

    public void BottleFell()
    {
        if (gameData.isSandboxMode)
        {
            return;
        }
        gameData.hasBottleFell = true;
        gameData.persistBottle = false;
        if (currentRound.fallTriggersCleaner)
        {
            StartCleaner();
        }
    }
    
    public void LiquidRanOut()
    {
        if (gameData.isSandboxMode)
        {
            return;
        }
        gameData.liquidRanOut = true;
        gameData.persistBottle = false;
        if (currentRound.liquidRanOutTriggersCleaner)
        {
            StartCleaner();
        }
    }

    public void BottleTouchedAWall()
    {
        gameData.hasBottleTouchedAWall = true;
        if (currentRound.wallTouchIsFall)
        {
            BottleFell();
        }
    }

    public void BallCleaned()
    {
        if (gameData.isSandboxMode)
        {
            return;
        }
        gameData.ballsPresent -= 1;
        if (gameData.ballsPresent == 0 && !gameData.isCleanerPresent)
        {
            NextRound();
        }
    }

    public void BottleCleaned()
    {
        if (gameData.isSandboxMode)
        {
            return;
        }
        DestroyPins();
        DestroyBalls();
        DestroyCleaner();
        if (currentRound.restartRoundOnly)
        {
            gameData.restartsCount += 1;
            StartCoroutine(RestartRound());
        }
        else
        {
            gameData.restartsCount = 0;
            StartCoroutine(RestartLevel());
        }
    }

    public void LaunchBall()
    {
        ballLauncher.Launch();
        gameData.ballsPresent += 1;
    }

    private void NextRound()
    {
        if (gameData.isSandboxMode)
        {
            nextLevelCoroutine = StartCoroutine(SetupGame(gameData.isSandboxMode));
            return;
        }
        if (gameData.roundNumber == currentLevel.rounds.Count - 1)
        {
            WinLevel();
        }
        else
        {
            gameData.roundNumber += 1;
            CurrentRound.text = (gameData.roundNumber + 1).ToString();
            nextLevelCoroutine = StartCoroutine(SetupGame(gameData.isSandboxMode));
        }
    }

    private IEnumerator RestartLevel()
    {
        gameData.roundNumber = 0;
        CurrentRound.text = "1";
        yield return RestartRound();
    }
    
    
    private IEnumerator RestartRound()
    {
        DestroyPins();
        DestroyCleaner();
        DestroyBalls();
        yield return SetupGame(gameData.isSandboxMode);
    }


    #region cleanup

    public void Cleanup(Coroutine coroutineToStop)
    {
        if (nextLevelCoroutine != null) StopCoroutine(nextLevelCoroutine);
        if (coroutineToStop != null) StopCoroutine(coroutineToStop);
        if (gameData.isSandboxMode)
        {
            SandboxCleanup();
        }
        liquidLevelBar.gameObject.SetActive(false);
        DestroyPins();
        DestroyBalls();
        DestroyCleaner();
        gameData.gameStarted = false;
    }
    
    private void SandboxCleanup()
    {
        bottle.GetComponent<Reset>().useKey = false;
        ballLauncher.useKey = false;
        sandboxPins.SetActive(false);
        sandboxHints.SetActive(false);
    }

    private void DestroyPins()
    {
        gameData.pinsPresent = 0;
        foreach (Transform s in pinContainer)
        {
            Destroy(s.gameObject);
        }
    }

    private void DestroyCleaner()
    {
        foreach (Transform s in cleanerContainer)
        {
            Destroy(s.gameObject);
        }
    }
    
    private void DestroyBalls()
    {
        gameData.ballsPresent = 0;
        foreach (Transform s in ballsContainer)
        {
            Destroy(s.gameObject);
        }
    }

    #endregion
    
    private void WinLevel()
    {
        // calculate score
        float score = bottle.liquidLevel * 90;
        score = Mathf.Max(score, 0);

        bottle.rb.isKinematic = true;
        bottle.rb.MoveRotation(Quaternion.identity);
        
        menuController.GameplayToScore(score);
    }

    public void ScoringCompleted()
    {
        currentLevelIndex += 1;
        if (currentLevelIndex >= levels.Count)
        {
            menuController.WinGame();
            return;
        }
        else
        {
            currentLevel = levels[currentLevelIndex];
            gameData.roundNumber = 0;
            StartCoroutine(SetupGame(gameData.isSandboxMode, 5));
        }
        CurrentLevel.text = (currentLevelIndex + 1).ToString();
        CurrentRound.text = "1";
        MaxRounds.text = currentLevel.rounds.Count.ToString();
    }

    private void StartCleaner()
    {
        if (gameData.isCleanerPresent)
        {
            return;
        }
        DestroyBalls();
        Instantiate(cleanerPrefab, cleanerContainer);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        gameSetupTimeout -= Time.fixedDeltaTime;
    }
}