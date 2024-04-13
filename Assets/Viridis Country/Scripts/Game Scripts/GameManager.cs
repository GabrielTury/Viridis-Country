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
    #region Game Resources
        public enum GameResources
        {
            None,
            Wood,
            Plank,
            Stone,
            ProcessedStone,
            ConstructionMaterials,
            Water,
            Milk,
            FermentedMilk,
            Cheese,
            Skin,
            Leather,
            Wool,
            Cloth,
            Clothes,
            Wheat,
            Flour,
            Bread,
            Gold
        }
    #endregion

    [SerializeField] //Temp
    private LevelObject levelVariables;

    public int actionsMade {  get; private set; }

    #region Resource Amounts
    [Header("Altere esses valores no level scriptable object!! \nEstao aqui somente para visualizacao")]

    public int zeroAmount;
    public int woodAmount;
    public int plankAmount;
    public int stoneAmount;
    public int processedStoneAmount;
    public int constructionMaterialsAmount;
    public int waterAmount;
    public int milkAmount;
    public int fermentedMilkAmount;
    public int cheeseAmount;
    public int skinAmount;
    public int leatherAmount;
    public int woolAmount;
    public int clothAmount;
    public int clothesAmount;
    public int wheatAmount;
    public int flourAmount;
    public int breadAmount;
    public int goldAmount;

    public int objectiveWater { get; private set; }
    public int objectiveWood { get; private set; }
    public int objectiveStone { get; private set; }
    public int objectivePlank { get; private set; }
    public int objectiveProcessedStone { get; private set; }
    public int objectiveConstructionMaterials { get; private set; }
    public int objectiveMilk { get; private set; }
    public int objectiveFermentedMilk { get; private set; }
    public int objectiveCheese { get; private set; }
    public int objectiveSkin { get; private set; }
    public int objectiveLeather { get; private set; }
    public int objectiveWool { get; private set; }
    public int objectiveCloth { get; private set; }
    public int objectiveClothes { get; private set; }
    public int objectiveWheat { get; private set; }
    public int objectiveFlour { get; private set; }
    public int objectiveBread { get; private set; }
    public int objectiveGold { get; private set; }

    int[] objectiveArray = new int[18];
    int[] amountArray = new int[18];

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
        objectivePlank = levelVariables.goalPlankAmount;
        objectiveProcessedStone = levelVariables.goalProcessedStoneAmount;
        objectiveConstructionMaterials = levelVariables.goalConstructionMaterialsAmount;
        objectiveMilk = levelVariables.goalMilkAmount;
        objectiveFermentedMilk = levelVariables.goalFermentedMilkAmount;
        objectiveCheese = levelVariables.goalCheeseAmount;
        objectiveSkin = levelVariables.goalSkinAmount;
        objectiveLeather = levelVariables.goalLeatherAmount;
        objectiveWool = levelVariables.goalWoolAmount;
        objectiveCloth = levelVariables.goalClothAmount;
        objectiveClothes = levelVariables.goalClothesAmount;
        objectiveWheat = levelVariables.goalWheatAmount;
        objectiveFlour = levelVariables.goalFlourAmount;
        objectiveBread = levelVariables.goalBreadAmount;
        objectiveGold = levelVariables.goalGoldAmount;

        for(int i = 0; i < objectiveArray.Length; i++)
        {
            objectiveArray[i] = GetObjectiveOnInt(i);
            amountArray[i] = GetAmountOnInt(i);
        }

    }

    private void GetGatheredResources(GameResources resource, int amount)
    {
        switch(resource)
        {
            case GameResources.None:
                // Não faz nada para o recurso "None"
                break;

            case GameResources.Water:
                waterAmount += amount;
                break;

            case GameResources.Wood:
                woodAmount += amount;
                break;

            case GameResources.Plank:
                plankAmount += amount;
                break;

            case GameResources.Stone:
                stoneAmount += amount;
                break;

            case GameResources.ProcessedStone:
                processedStoneAmount += amount;
                break;

            case GameResources.ConstructionMaterials:
                constructionMaterialsAmount += amount;
                break;

            case GameResources.Milk:
                milkAmount += amount;
                break;

            case GameResources.FermentedMilk:
                fermentedMilkAmount += amount;
                break;

            case GameResources.Cheese:
                cheeseAmount += amount;
                break;

            case GameResources.Skin:
                skinAmount += amount;
                break;

            case GameResources.Leather:
                leatherAmount += amount;
                break;

            case GameResources.Wool:
                woolAmount += amount;
                break;

            case GameResources.Cloth:
                clothAmount += amount;
                break;

            case GameResources.Clothes:
                clothesAmount += amount;
                break;

            case GameResources.Wheat:
                wheatAmount += amount;
                break;

            case GameResources.Flour:
                flourAmount += amount;
                break;

            case GameResources.Bread:
                breadAmount += amount;
                break;

            case GameResources.Gold:
                goldAmount += amount;
                break;
        }

        //aumenta as ações em 1
        actionsMade++;
        Debug.Log("Acoes: "+actionsMade);

        bool objectivesMet = true;
        for(int i = 0; i < amountArray.Length; i++)
        {
            amountArray[i] = GetAmountOnInt(i);
            if (amountArray[i] != objectiveArray[i])
            {
                objectivesMet = false;
                return;
            }
        }

        if(objectivesMet)
        {
            GameEvents.OnLevelEnd();
        }
    }
    #region Get for Arrays
    private int GetAmountOnInt(int id)
    {
        switch (id)
        {
            case 0:
                return stoneAmount;
            case 1:
                return waterAmount;
            case 2:
                return woodAmount;
            case 3:
                return plankAmount;
            case 4:
                return processedStoneAmount;
            case 5:
                return constructionMaterialsAmount;
            case 6:
                return milkAmount;
            case 7:
                return fermentedMilkAmount;
            case 8:
                return cheeseAmount;
            case 9:
                return skinAmount;
            case 10:
                return leatherAmount;
            case 11:
                return woolAmount;
            case 12:
                return clothAmount;
            case 13:
                return clothesAmount;
            case 14:
                return wheatAmount;
            case 15:
                return flourAmount;
            case 16:
                return breadAmount;
            case 17:
                return goldAmount;
            default:
                // Se o recurso não estiver definido, retorna 0
                return 0;
        }
    }
    private int GetObjectiveOnInt(int id) //Define tambem a ordem dos recursos caso seja necessário
    {
        switch (id)
        {
            case 0:
                return objectiveStone;

            case 1:
                return objectiveWater;

            case 2:
                return objectiveWood;

            case 3:
                return objectivePlank;

            case 4:
                return objectiveProcessedStone;

            case 5:
                return objectiveConstructionMaterials;

            case 6:
                return objectiveMilk;

            case 7:
                return objectiveFermentedMilk;

            case 8:
                return objectiveCheese;

            case 9:
                return objectiveSkin;

            case 10:
                return objectiveLeather;

            case 11:
                return objectiveWool;

            case 12:
                return objectiveCloth;

            case 13:
                return objectiveClothes;

            case 14:
                return objectiveWheat;

            case 15:
                return objectiveFlour;

            case 16:
                return objectiveBread;

            case 17:
                return objectiveGold;

            default:
                return 0;
        }
    }
    #endregion
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
        Debug.Log("Terminou o Level com: " + actionsMade + " acoes");

       
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

