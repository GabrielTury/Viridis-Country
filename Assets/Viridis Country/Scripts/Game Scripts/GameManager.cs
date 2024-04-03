using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    #region Public
        public enum GameResources
        {
            None,
            Wood,
            Stone,
            Water
        }
    #endregion

    [SerializeField] //Temp
    private LevelObject levelVariables;

    public int actionsMade {  get; private set; }

    #region Resource Amounts
    [Header("Altere esses valores no level scriptable object!! \nEstao aqui somente para visualizacao")]
    public int zeroAmount;
    public int waterAmount;
    public int woodAmount;
    public int stoneAmount;

    public int objectiveWater { get; private set; }
    public int objectiveWood { get; private set; }
    public int objectiveStone { get; private set; }
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        objectiveStone = levelVariables.goalStoneAmount;
        objectiveWater = levelVariables.goalWaterAmount;
        objectiveWood = levelVariables.goalWoodAmount;

    }

    private void GetGatheredResources(GameResources resource, int amount)
    {
        switch(resource)
        {
            case GameResources.None:

                break;

            case GameResources.Water:

                waterAmount += amount;
                break;

            case GameResources.Wood:

                woodAmount += amount;
                break;

            case GameResources.Stone:

                stoneAmount += amount;
                break;
        }

        //aumenta as ações em 1
        actionsMade++;

        if(stoneAmount == objectiveStone && waterAmount == objectiveWater && woodAmount == objectiveWood)
        {
            GameEvents.OnLevelEnd();
        }
    }
    private void LevelEnd()
    {
        if(actionsMade <= levelVariables.threeStarsAmount)
        {
            Debug.Log("***");
        }
        else if( actionsMade <= levelVariables.twoStarsAmount)
        {
            Debug.Log("**");
        }
        else if(actionsMade >= levelVariables.oneStarAmount)
        {
            Debug.Log("*");
        }
        Debug.Log("Terminou o Level com: " + actionsMade + " construcoes");

       
    }

    private void OnEnable()
    {
        GameEvents.Construction_Placed += GetGatheredResources;
        GameEvents.Level_End += LevelEnd;
    }

    private void OnDisable()
    {
        GameEvents.Construction_Placed -= GetGatheredResources;
        GameEvents.Level_End -= LevelEnd;
    }


}

