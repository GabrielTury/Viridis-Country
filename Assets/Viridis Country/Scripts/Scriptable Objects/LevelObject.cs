using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObject : ScriptableObject
{

    public int goalWoodAmount;
    public int goalWaterAmount;
    public int goalStoneAmount;

}
