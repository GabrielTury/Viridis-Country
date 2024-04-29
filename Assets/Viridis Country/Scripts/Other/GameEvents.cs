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




        public static event UnityAction<AudioManager.SoundEffects> Zoom;
        public static void OnZoom(AudioManager.SoundEffects sfx) => Zoom?.Invoke(sfx);

        public static event UnityAction<AudioManager.SoundEffects> Click;
        public static void OnClick(AudioManager.SoundEffects sfx) => Click?.Invoke(sfx);

        public static event UnityAction<AudioManager.SoundEffects> Select_Construction;
        public static void OnSelectConstruction(AudioManager.SoundEffects sfx) => Select_Construction?.Invoke(sfx);

        public static event UnityAction<AudioManager.SoundEffects> OneStar;
        public static void OnOneStar(AudioManager.SoundEffects sfx) => OneStar?.Invoke(sfx);

        public static event UnityAction<AudioManager.SoundEffects> TwoStar;
        public static void OnTwoStar(AudioManager.SoundEffects sfx) => TwoStar?.Invoke(sfx);

        public static event UnityAction<AudioManager.SoundEffects> ThreeStar;
        public static void OnThreeStar(AudioManager.SoundEffects sfx) => ThreeStar?.Invoke(sfx);



        public static event UnityAction Level_Start;
        public static void OnLevelStart() => Level_Start?.Invoke();

        public static event UnityAction Level_End;
        public static void OnLevelEnd() => Level_End?.Invoke();
    }
}
