using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public int currentLevel;
    public int currentStars;

    public PlayerData (int level, int stars)
    {
        currentLevel = level;
        currentStars = stars;
    }

}
