using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private SaveManager saveManager;

    private DateTime timerStart;

    private DateTime timerEnd;

    private Coroutine timerCoroutine;
    [SerializeField]
    private int daysToRecharge;
    [SerializeField]
    private int hoursToRecharge;
    [SerializeField]
    private int minutesToRecharge;
    [SerializeField]
    private int secondsToRecharge;

    private TimeSpan rechargeTime;

    [SerializeField]
    private int maxEnergy;
    public int energyAmount { get; private set; }

    [SerializeField]
    private TextMeshProUGUI energyCounterText;
    

    private PlayerData playerLoadedData;

    [Header("Ads Area")]

    [SerializeField]
    private string mainMenuScene;

    private bool isAdsInitialized = false;

    public Dictionary<string, int> playerLevels { get; private set; } 
           

    [SerializeField]
    private int playableLevels;
    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        rechargeTime = new TimeSpan(daysToRecharge, hoursToRecharge, minutesToRecharge, secondsToRecharge);

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        saveManager = GetComponent<SaveManager>();

        playerLoadedData = saveManager.LoadGame();

        if( playerLoadedData != null )
        {
            energyAmount = playerLoadedData.currentEnergy;
            playerLevels = playerLoadedData.levels;
            timerStart = DateTime.Parse(playerLoadedData.timerStart);

            OffScreenRecharge();
            
        }
        else
        {
            Debug.Log("Used Default save Values");
            energyAmount = maxEnergy;
            playerLevels = new Dictionary<string, int>();
            for(int i = 0; i < playableLevels; i++)
            {
                playerLevels.Add("level " + i, 0);
            }

            PlayerData newPlayerData = new PlayerData(energyAmount, timerStart, timerEnd, playerLevels); //default data
            saveManager.SaveGame(newPlayerData);
        }

        Debug.Log("Start");
        energyCounterText.gameObject.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("App Quit");
        timerStart = DateTime.Now;

        PlayerData newPlayerData = new PlayerData(energyAmount, timerStart, timerEnd, playerLevels);
        saveManager.SaveGame(newPlayerData);
    }

    private void OnEnable()
    {
        GameEvents.Level_Start += LevelStarted;
        GameEvents.Level_End += LevelEndSession;

        SceneManager.sceneLoaded += SceneLoad;
        SceneManager.sceneUnloaded += SceneUnload;

        AdEvents.Ad_Completed += AdReward;
        AdEvents.Ads_Initialized += AdSystemInitialized;
    }

    private void OnDisable()
    {
        GameEvents.Level_Start -= LevelStarted;
        GameEvents.Level_End -= LevelEndSession;

        SceneManager.sceneLoaded -= SceneLoad;
        SceneManager.sceneUnloaded -= SceneUnload;

        AdEvents.Ad_Completed -= AdReward;
        AdEvents.Ads_Initialized -= AdSystemInitialized;
    }

    private void LevelStarted()
    {
        if(energyAmount > 0)
            energyAmount--;
        
        
        CheckEnergy();
    }
    private void LevelEndSession()
    {            
        PlayerData newPlayerData = new PlayerData(energyAmount, timerStart, timerEnd, playerLevels);
        saveManager.SaveGame(newPlayerData);

        int totalStars = 0;
        foreach(var a in playerLevels)
        {
            totalStars += a.Value;
        }
        if(totalStars >= 30)
        {
            AdEvents.OnAchievementCompleted("Achievement05", 100);
        }
        else if(totalStars >= 10)
        {
            AdEvents.OnAchievementCompleted("Achievement04", 100);
        }
        
    }

    private void CheckEnergy()
    {
        if (energyAmount < maxEnergy && timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }
    }

    private IEnumerator TimerCoroutine()
    {
        timerStart = DateTime.Now;

        timerEnd = timerStart.Add(rechargeTime);
        Debug.Log(timerEnd);
        double timeLeft = rechargeTime.TotalSeconds;

        yield return new WaitForSeconds(Convert.ToSingle((timerEnd - timerStart).TotalSeconds));

        energyAmount++;
        timerCoroutine = null;
        
        CheckEnergy();
    }

    private void OffScreenRecharge()
    {
        TimeSpan secondsPassed = DateTime.Now - timerStart;

        if(energyAmount < maxEnergy)
        {
            energyAmount += (int)(secondsPassed.TotalSeconds / rechargeTime.TotalSeconds);
            //Debug.Log("Quantidade de energia a ser adicionada: " +  (int)(secondsPassed.TotalSeconds / rechargeTime.TotalSeconds));

            if(energyAmount > maxEnergy)
                energyAmount = maxEnergy;
        }

        CheckEnergy();
    }

    public void AddEnergy(int amount)
    {
        energyAmount += amount;
        UpdateEnergyCounterUI();
    }

    private void SceneLoad(Scene sceneLoaded, LoadSceneMode loadSceneMode)
    {
        if(sceneLoaded.name == mainMenuScene && energyAmount < maxEnergy)
        {
            if (isAdsInitialized)
                AdEvents.OnEnergyDepleted();
            else
            {
                StartCoroutine(WaitAndCheckAds());
                Debug.Log(isAdsInitialized);
            }
            
        }

        if (sceneLoaded.name == mainMenuScene)
        {
            foreach(TextMeshProUGUI a in FindObjectsOfType<TextMeshProUGUI>())
            {
                if (a.gameObject.CompareTag("energyCounter"))
                {
                    energyCounterText = a;
                }
            }

            if (energyCounterText != null)
            {
                energyCounterText.gameObject.SetActive(true);
                UpdateEnergyCounterUI();
                Debug.Log("Scene Load");
            }
        }
        else
        {
            if(energyCounterText != null)
                energyCounterText.gameObject.SetActive(false);
        }
            
    }

    private IEnumerator WaitAndCheckAds()
    {
        yield return new WaitForSeconds(2);

        SceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void SceneUnload(Scene unloadedScene)
    {
        StopCoroutine(WaitAndCheckAds());
    }

    private void AdReward()
    {
        AddEnergy(1);
    }

    private void AdSystemInitialized()
    {
        isAdsInitialized = true;
    }

    private void UpdateEnergyCounterUI()
    {
        if (energyCounterText != null)
            energyCounterText.text = energyAmount.ToString() + "/" + maxEnergy.ToString();
    }

    public void SetStarsAmount(string key, int newAmount)
    {
        Debug.Log("Set Stars Amount Called with: " + newAmount);
        playerLevels[key] = newAmount;
        Debug.Log("value for that key: " + playerLevels[key]);
    }

    public int GetStarsAmount(string levelName)
    {
        return playerLevels[levelName];
    }

    [ContextMenu("OverWriteLevelsDictionary")]
    public void OverWriteDictionary()
    {
        playerLevels = new Dictionary<string, int>();
        for (int i = 0; i < playableLevels; i++)
        {
            playerLevels.Add("level " + (i+1), 0);
        }
    }
}
