using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public int currentEnergy;
    public string timerStart;
    public string timerEnd;

    public Dictionary<string, int> levels;

    public PlayerData (int energy, DateTime tStart, DateTime tEnd, Dictionary<string, int> allLevels)
    {
        currentEnergy = energy;
        timerStart = tStart.ToString();
        timerEnd = tEnd.ToString();
        levels = allLevels;

    }

}
