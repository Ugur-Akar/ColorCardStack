using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Settings
    public bool spawnLevel = true;
    public bool isDominoEffect = true;
    public int tutorialLevels;

    // Connections
    public GameObject[] levels;
    public UIManager ui;
    public LevelData levelData;
    public GameObject[] boundGos;
    
    // State variables
    int currentLevel;
    int score;
    int starCount = 3;

    bool canFail = true;
    bool hasFailed = false;

    public int stacksLeft = 0;
    public int movesLeft = 0;
    float cameraSize = 5;

    List<string> doneStacks;

    private void Awake()
    {
        EventManagerInit();

        InitStates();
        InitConnections();
        if(isDominoEffect)
            EventManager.CardConfigEvent();

        doneStacks = new List<string>();
    }

    void InitStates()
    {
        currentLevel = PlayerPrefs.GetInt("Level", 0);
        LoadLevel();
        Camera.main.orthographicSize = cameraSize;
        BuildBounds();
    }

    void LoadLevel()
    {
        if (spawnLevel)
        {
            int prefabIndex = GetPrefabIndex(currentLevel, tutorialLevels, levels.Length);
            GameObject levelGO = Instantiate(levels[prefabIndex], Vector3.zero, Quaternion.identity);
            movesLeft = levelData.startMoves[prefabIndex] + levelData.otherMoves[prefabIndex] * 2;
            cameraSize = levelData.cameraSize[prefabIndex];
            RenderSettings.skybox = levelData.materials[prefabIndex];
            ui.InitMoveUI(levelData.startMoves[prefabIndex], levelData.otherMoves[prefabIndex]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 50; i++)
            {
                Debug.Log("Prefab index for level " + i + ":" + GetPrefabIndex(i, tutorialLevels, levels.Length));
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            OnFinishLevel();
        }

    }

    int GetPrefabIndex(int levelIndex, int nInitialLevels, int nLevels)
    {

        int nRepeatingLevels = nLevels - nInitialLevels;
        int prefabIndex = levelIndex;
        if (levelIndex >= nInitialLevels)
        {
            prefabIndex = ((levelIndex - nInitialLevels) % nRepeatingLevels) + nInitialLevels;
        }
        return prefabIndex;

    }

    void InitConnections()
    {
        ui.OnLevelStart += OnLevelStart;
        ui.OnNextLevel += OnNextLevel;
        ui.OnLevelRestart += OnLevelRestart;

        UpdateLevelUI();
        
    }

    void OnLevelFailed()
    {
        if (canFail)
        {
            hasFailed = true;
            EventManager.DisableSwipingEvent();
            EventManager.CleanEvents();
            ui.FailLevel();
            Debug.Log("LEVEL FAILED");
        }        
        
    }

    void OnFinishLevel()
    {
        if (!hasFailed)
        {
            canFail = false;
            EventManager.CleanEvents();
            ui.SetScore(movesLeft);
            ui.FinishLevel();
            PlayerPrefs.SetInt("Level", currentLevel + 1);
        }
        
    }

    void OnLevelStart()
    {
        Debug.Log("LEVEL STARTED");
        EventManager.OnLevelStartedEvent();
    }

    void OnLevelRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("showStart", 0);
    }

    #region EventManager
    void EventManagerInit()
    {
        EventManager.StackIsComplete += StackIsComplete;
        EventManager.StackIsWrong += StackIsWrong;
        EventManager.StackIsIncomplete += StackIsIncomplete;
        EventManager.AnchorEnabled += AnchorEnabled;
        EventManager.CardSwiped += CardSwiped;
    }

    void StackIsComplete(string cardTag)
    {
        Debug.Log("Stack is right.");

        if (!doneStacks.Contains(cardTag))
        {
            doneStacks.Add(cardTag);
            stacksLeft--;
            if (stacksLeft <= 0)
            {
                OnFinishLevel();
            }
        }

    }

    void StackIsWrong()
    {
        Debug.Log("Stack is wrong.");
    }

    void StackIsIncomplete()
    {
        Debug.Log("Stack is incomplete.");
    }

    void AnchorEnabled()
    {
        stacksLeft++;
    }

    void CardSwiped()
    {
        movesLeft--;
        UpdateMoveUI();

        if(movesLeft <= 0 && stacksLeft >= 1)
        {
            OnLevelFailed();
        }
        
    }
    #endregion

    void UpdateLevelUI()
    {
        ui.UpdateLevelUI(currentLevel);
    }

    void UpdateMoveUI()
    {
        ui.UpdateMoveUI(movesLeft);
    }

    public void OnSensitivityChanged(string value = "")
    {
        //Debug.Log(value);
        float sensitivity;
        if (IsDigitsOnly(value))
        {
            sensitivity = float.Parse(value);
            PlayerPrefs.SetFloat(nameof(sensitivity), sensitivity);
        }
        else
        {
            sensitivity = PlayerPrefs.GetFloat(nameof(sensitivity), 0.5f);
        }

        EventManager.OnSensitivityChangedEvent(sensitivity);
    }

    bool IsDigitsOnly(string str)
    {
        bool hasDot = false;
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
            {
                if((c == ',' || c =='.') && !hasDot)
                {
                    hasDot = true;
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    public void GetText(TMP_InputField tmp)
    {
        string val = tmp.text;
        OnSensitivityChanged(val);
    }

    void BuildBounds()
    {
        //0 horizontal
        //1 vertical

        Instantiate(boundGos[0], new Vector3(0, -cameraSize, 0), Quaternion.identity);
        Instantiate(boundGos[0], new Vector3(0, cameraSize, 0), Quaternion.identity);
        float width = Screen.width;
        float height = Screen.height;
        Debug.Log("Coef = " + width / height);  
        Instantiate(boundGos[1], new Vector3(-cameraSize * width / height, 0, 0), Quaternion.identity);
        Instantiate(boundGos[1], new Vector3(cameraSize * width / height, 0, 0), Quaternion.identity);
    }

}