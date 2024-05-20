using GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public SessionManager Instance { get; private set; }

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
    public int currentLevel { get; private set; }
    public int currentStars { get; private set; }

    

    private PlayerData playerLoadedData;

    [Header("Ads Area")]

    [SerializeField]
    private string mainMenuScene;

    private bool isAdsInitialized = false;

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

        rechargeTime = new TimeSpan(daysToRecharge, hoursToRecharge, minutesToRecharge, secondsToRecharge);

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        saveManager = GetComponent<SaveManager>();

        playerLoadedData = saveManager.LoadGame();

        if( playerLoadedData != null )
        {
            currentLevel = playerLoadedData.currentLevel;
            currentStars = playerLoadedData.currentStars;
            energyAmount = playerLoadedData.currentEnergy;
            timerStart = DateTime.Parse(playerLoadedData.timerStart);

            OffScreenRecharge();
            
        }
        else
        {
            Debug.Log("Used Default save Values");
            energyAmount = maxEnergy;
            PlayerData newPlayerData = new PlayerData(0, 0, energyAmount, timerStart, timerEnd); //default data
            saveManager.SaveGame(newPlayerData);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("App Quit");

        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars, energyAmount, timerStart, timerEnd);
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
    /*[ContextMenu("TESTESAVE")]
    public void Teste()
    {
        currentLevel = 2;
        currentStars = 5;

        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars, energyAmount, timerStart, timerEnd);
        saveManager.SaveGame(newPlayerData);
    }*/
    private void LevelStarted()
    {
        if(energyAmount > 0)
            energyAmount--;
        
        
        CheckEnergy();
    }
    private void LevelEndSession()
    {            
        PlayerData newPlayerData = new PlayerData(currentLevel, currentStars, energyAmount, timerStart, timerEnd);
        saveManager.SaveGame(newPlayerData);

        
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
    }

    private void SceneLoad(Scene sceneLoaded, LoadSceneMode loadSceneMode)
    {
        if(sceneLoaded.name == mainMenuScene && energyAmount < maxEnergy)
        {
            if (isAdsInitialized)
                AdEvents.OnEnergyDepleted();
            else
                StartCoroutine(WaitAndCheckAds());
            
        }

        if (sceneLoaded.name == mainMenuScene)
            Debug.Log("Menu Principal");
    }

    private IEnumerator WaitAndCheckAds()
    {
        yield return new WaitForSeconds(5);

        SceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void SceneUnload(Scene unloadedScene)
    {

    }

    private void AdReward()
    {
        AddEnergy(1);
    }

    private void AdSystemInitialized()
    {
        isAdsInitialized = true;
    }
}
