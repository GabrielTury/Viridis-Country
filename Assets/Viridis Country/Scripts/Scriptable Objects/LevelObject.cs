using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject
{
    [Space(2), Header("Objetivos")]
    public int goalWoodAmount;
    public int goalPlankAmount;
    public int goalStoneAmount;
    public int goalProcessedStoneAmount;
    public int goalConstructionMaterialsAmount;
    public int goalWaterAmount;
    public int goalMilkAmount;
    public int goalFermentedMilkAmount;
    public int goalCheeseAmount;
    public int goalSkinAmount;
    public int goalLeatherAmount;
    public int goalWoolAmount;
    public int goalClothAmount;
    public int goalClothesAmount;
    public int goalWheatAmount;
    public int goalFlourAmount;
    public int goalBreadAmount;
    public int goalGoldAmount;

    [Space(2), Header("Estrelas")]
    public int threeStarsAmount;
    public int twoStarsAmount;
    public int oneStarAmount;

    [Space(2), Header("Tentativas")]
    public int maxTries;

}
