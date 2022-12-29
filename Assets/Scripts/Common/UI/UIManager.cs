using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    const float DEFAULT_START_DELAY = 0.2f;

    public  Action OnLevelStart, 
                    OnNextLevel, 
                    OnLevelRestart, 
                    OnGamePaused, 
                    OnGameResumed, 
                    OnInGameRestart;

    [Header("Settings")]
    public bool defaultPauseOperations = true;
    public int scoreCoef = 200;

    [Header("Screens")]
    public GameObject startCanvas;
    public GameObject ingameCanvas;
    public GameObject finishCanvas;
    public GameObject failCanvas;
    public GameObject pauseMenu;
    [Header("Start")]
    public TextMeshProUGUI startLevelText;
    [Header("In Game")]
    public LevelBarDisplay levelBarDisplay;
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI inGameLevelText;
    public GameObject[] stars;
    public LevelData levelData;
    public DOTweenAnimation handleTween;
    [Header("Finish Screen")]
    //public ScoreTextManager scoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endGameLevelText;
    public GameObject[] endGameStars;
    public GameObject retryButton;
    // State variables
    float timeScale;
    bool movesInitialized = false;
    bool firstStarLost = false;
    int maxAmountOfMoves = 0;
    float starValue = 0;
    float startMoveValue = 0.1f;
    float otherMoveValue = 0.1f;
    float progress = 3;
    int startMoves, otherMoves;
    int totalScore;
    void Start()
    {
        InitState();   
    }
    
    void InitState()
    {
        timeScale = Time.timeScale;
        starValue = 1f / stars.Length;

        InitStates();

        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    #region Handler Functions

    public void StartLevelButton()
    {
        OnLevelStart?.Invoke();
        
    }

    public void NextLevelButton()
    {
        PlayerPrefs.SetInt("displayStart", 0);
        OnNextLevel?.Invoke();

    }

    public void RestartLevelButton()
    {
        EventManager.CleanEvents();
        PlayerPrefs.SetInt("displayStart", 0);
        OnLevelRestart?.Invoke();
    }

    public void OnPauseButtonPressed()
    {
        pauseMenu.SetActive(true);
        Camera.main.GetComponent<Collider>().enabled = true;
        if (defaultPauseOperations)
        {
            timeScale = Time.timeScale; // Restore the current time scale to use in Resume button
            Time.timeScale = 0;
        }
        OnGamePaused?.Invoke();
    }

    public void OnResumeButtonPressed()
    {
        pauseMenu.SetActive(false);
        Camera.main.GetComponent<Collider>().enabled = false;
        if (defaultPauseOperations)
        {
            Time.timeScale = timeScale;
        }
        OnGameResumed?.Invoke();
    }

    public void OnInGameRestartPressed()
    {
        //EventManager.EnableSwipingEvent();
        if (defaultPauseOperations)
        {
            Time.timeScale = timeScale;
            EventManager.CleanEvents();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        OnInGameRestart?.Invoke();
    }

    #endregion

    public void StartLevel()
    {
        startCanvas.SetActive(false);
        ingameCanvas.SetActive(true);
    }

    public void SetInGameScore(int score)
    {
        inGameScoreText.text = "" + score;
    }

    public void SetInGameScoreAsText(string scoreText)
    {
        inGameScoreText.text = scoreText;
    }


    public void DisplayScore(int score, int oldScore=0)
    {
        //scoreText.DisplayScore(score, oldScore);
    }

    public void SetLevel(int level)
    {
        levelBarDisplay.SetMoves(level);
    }

    public void UpdateProgess(int progress)
    {
        levelBarDisplay.DisplayProgress(progress);
    }

    public void FinishLevel()
    {
        ingameCanvas.SetActive(false);
        finishCanvas.SetActive(true);
        if (firstStarLost)
        {
            retryButton.SetActive(true);
        }
        movesInitialized = false;
    }

    public void FailLevel()
    {
        ingameCanvas.SetActive(false);
        failCanvas.SetActive(true);
    }
    void InitStates()
    {
        ingameCanvas.SetActive(false);
        finishCanvas.SetActive(false);
        failCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

  
    public void OnRestartButtonPressed()
    {
        int index = PlayerPrefs.GetInt("Level", 0);
        index--;
        PlayerPrefs.SetInt("Level", index);
        EventManager.CleanEvents();
        OnLevelRestart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLevelUI(int currentLevel)
    {
        int level = currentLevel + 1;
        startLevelText.text = level.ToString();
        inGameLevelText.text = level.ToString();
        endGameLevelText.text = level.ToString();
    }

    public void InitMoveUI(int startMoves, int otherMoves)
    {
        if (!movesInitialized)
        {
            movesInitialized = true;
            maxAmountOfMoves = startMoves + 2 * otherMoves;
            startMoveValue = 1f / startMoves;
            otherMoveValue = 1f / otherMoves;
            this.startMoves = startMoves;
            this.otherMoves = otherMoves;
            UpdateMoveUI(maxAmountOfMoves);
        }
    }

    public void UpdateMoveUI(int movesLeft)
    {
        if(movesLeft >= maxAmountOfMoves)
        {
            levelBarDisplay.DisplayProgress(3);
            levelBarDisplay.SetMoves(maxAmountOfMoves);
        }

        else if(movesLeft < maxAmountOfMoves)
        {
            if(movesLeft + startMoves >= maxAmountOfMoves)
            {
                progress -= startMoveValue;
            }
            else if(movesLeft + otherMoves >= maxAmountOfMoves - startMoves)
            {
                if (!firstStarLost)
                {
                    firstStarLost = true;
                    //stars[2].SetActive(false);
                    stars[2].transform.parent.GetComponent<DOTweenAnimation>().DORestart();
                    endGameStars[2].SetActive(false);
                    progress -= otherMoveValue;
                }
                else
                {
                    progress -= otherMoveValue;
                }

            }
            else
            {
                stars[1].transform.parent.GetComponent<DOTweenAnimation>().DORestart();
                endGameStars[1].SetActive(false);
                progress -= otherMoveValue;
            }

            handleTween.DORestart();
            levelBarDisplay.DisplayProgress(progress);
            levelBarDisplay.SetMoves(movesLeft);
        }

    }

    public void OnStarTweenFinished(int starIndex)
    {
        stars[starIndex].SetActive(false);
    }

    public void SetScore(int movesLeft)
    {
        int score = movesLeft * scoreCoef;
        totalScore += score;
        PlayerPrefs.SetInt("TotalScore", totalScore);
        scoreText.text = totalScore.ToString();
    }
}
