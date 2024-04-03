using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
    public static class GameEvents
    {
        public static event UnityAction<GameManager.GameResources, int> Construction_Placed;
        public static void OnConstructionPlaced(GameManager.GameResources resource, int amount) => Construction_Placed?.Invoke(resource, amount);

        public static event UnityAction Level_Start;
        public static void OnLevelStart() => Level_Start?.Invoke();

        public static event UnityAction Level_End;
        public static void OnLevelEnd() => Level_End?.Invoke();
    }
}
