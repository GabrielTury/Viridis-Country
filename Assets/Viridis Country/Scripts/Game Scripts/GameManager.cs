using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    public int zeroAmount;
    public int waterAmount;
    public int woodAmount;
    public int stoneAmount;

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
    }

    private void OnEnable()
    {
        GameEvents.Resource_Gathered += GetGatheredResources;
    }

    private void OnDisable()
    {
        GameEvents.Resource_Gathered -= GetGatheredResources;
    }


}

