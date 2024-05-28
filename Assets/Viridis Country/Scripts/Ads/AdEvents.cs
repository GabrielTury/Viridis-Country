using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class AdEvents
{

    public static event UnityAction Energy_Depleted;
    public static void OnEnergyDepleted() => Energy_Depleted?.Invoke();

    public static event UnityAction Ad_Completed;
    public static void OnAdCompleted() => Ad_Completed?.Invoke();

    public static event UnityAction Ads_Initialized;
    public static void OnAdsInitialized() => Ads_Initialized?.Invoke();

    public static event UnityAction Player_Initialized;
    public static void OnPlayerInitialized() => Player_Initialized?.Invoke();

    public static event UnityAction<string, double> Achievement_Completed;
    public static void OnAchievementCompleted(string id, double progress) => Achievement_Completed?.Invoke(id, progress);
    
}
