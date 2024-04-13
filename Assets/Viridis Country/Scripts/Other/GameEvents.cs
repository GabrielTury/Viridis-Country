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

        public static event UnityAction Construction_Placed;
        public static void OnConstructionPlaced() => Construction_Placed?.Invoke();

        public static event UnityAction Level_Start;
        public static void OnLevelStart() => Level_Start?.Invoke();

        public static event UnityAction Level_End;
        public static void OnLevelEnd() => Level_End?.Invoke();
    }
}
