using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
    public static class GameEvents
    {
        public static event UnityAction<GameManager.GameResources, int> Resource_Gathered;
        public static void OnResourceGathered(GameManager.GameResources resource, int amount) => Resource_Gathered?.Invoke(resource, amount);
    }
}
