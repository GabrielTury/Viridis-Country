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

    public int constructionsPlaced {  get; private set; }

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

        //Temp para achar as construções
        constructionsPlaced = 0;
        foreach(GameObject obj in FindObjectsOfType<GameObject>())
        {
            if(obj.layer == 6)
            {
                constructionsPlaced++;
            }
        }

        if(stoneAmount == objectiveStone && waterAmount == objectiveWater && woodAmount == objectiveWood)
        {
            GameEvents.OnLevelEnd();
        }
    }
    private void LevelEnd()
    {
        if(constructionsPlaced <= levelVariables.threeStarsAmount)
        {
            Debug.Log("***");
        }
        else if( constructionsPlaced <= levelVariables.twoStarsAmount)
        {
            Debug.Log("**");
        }
        else if(constructionsPlaced >= levelVariables.oneStarAmount)
        {
            Debug.Log("*");
        }
        Debug.Log("Terminou o Level com: " + constructionsPlaced + " construcoes");

       
    }

    private void OnEnable()
    {
        GameEvents.Resource_Gathered += GetGatheredResources;
        GameEvents.Level_End += LevelEnd;
    }

    private void OnDisable()
    {
        GameEvents.Resource_Gathered -= GetGatheredResources;
        GameEvents.Level_End -= LevelEnd;
    }


}

