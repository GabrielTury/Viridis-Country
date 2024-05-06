using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public int currentLevel;
    public int currentStars;
    public int currentEnergy;
    public string timerStart;
    public string timerEnd;

    public PlayerData (int level, int stars, int energy, DateTime tStart, DateTime tEnd)
    {
        currentLevel = level;
        currentStars = stars;
        currentEnergy = energy;
        timerStart = tStart.ToString();
        timerEnd = tEnd.ToString();

        
    }

}
