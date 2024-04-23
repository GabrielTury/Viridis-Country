using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public SessionManager Instance { get; private set; }

    private SaveManager saveManager;

    public int currentLevel { get; private set; }
    public int currentStars { get; private set; }

    private PlayerData playerLoadedData;
    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
    }

    private void Start()
    {
        saveManager = GetComponent<SaveManager>();

        playerLoadedData = saveManager.LoadGame();

        if( playerLoadedData != null )
        {
        currentLevel = playerLoadedData.currentLevel;
        currentStars = playerLoadedData.currentStars;
        }
        else
        {
            PlayerData newPlayerData = new PlayerData(0,0); //default data
            saveManager.SaveGame(newPlayerData);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars);
        saveManager.SaveGame(newPlayerData);
    }

    private void OnEnable()
    {
        GameEvents.Level_End += LevelEndSession;
    }

    private void OnDisable()
    {
        GameEvents.Level_End -= LevelEndSession;
    }
    [ContextMenu("TESTESAVE")]
    public void Teste()
    {
        currentLevel = 2;
        currentStars = 5;

        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars);
        saveManager.SaveGame(newPlayerData);
    }

    private void LevelEndSession()
    {
        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars);
        saveManager.SaveGame(newPlayerData);
    }
}
