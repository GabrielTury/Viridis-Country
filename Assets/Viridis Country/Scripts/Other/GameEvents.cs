using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
    public static class GameEvents
    {
        public static event UnityAction <GameManager.GameResources, int> Resource_Gathered;
        public static void OnResourceGathered(GameManager.GameResources resource, int amount) => Resource_Gathered?.Invoke(resource, amount);

        public static event UnityAction <AudioManager.ConstructionAudioTypes> Construction_Placed;
        public static void OnConstructionPlaced(AudioManager.ConstructionAudioTypes cType) => Construction_Placed?.Invoke(cType);

        public static event UnityAction <AudioManager.ConstructionAudioTypes> Construction_Removed;
        public static void OnConstructionRemoved(AudioManager.ConstructionAudioTypes cType) => Construction_Removed?.Invoke(cType);

        public static event UnityAction Level_Start;
        public static void OnLevelStart() => Level_Start?.Invoke();

        public static event UnityAction Level_End;
        public static void OnLevelEnd() => Level_End?.Invoke();
    }
}
