using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AchievemntsManager : MonoBehaviour
{
    private void Start()
    {
        
        
    }
    [ContextMenu("ShowUI")]
    private void ShowUI()
    {
        Social.ShowAchievementsUI();
    }

    void CreateAchievements()
    {
        PlayTutorial();
        PlayedWholeTutorial();
        FirstThreeStars();
        ReceivedTenStars();
        ReceivedThirtyStars();
        
    }
    void PlayTutorial()
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = "Achievement01";
        achievement.percentCompleted = 0;
        achievement.ReportProgress(result => {
            if (result)
                Debug.Log("Successfully reported progress");
            else
                Debug.Log("Failed to report progress");
        });
    }
    void PlayedWholeTutorial()
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = "Achievement02";
        achievement.percentCompleted = 0;
        achievement.ReportProgress(result => {
            if (result)
                Debug.Log("Successfully reported progress");
            else
                Debug.Log("Failed to report progress");
        });
    }

    void FirstThreeStars()
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = "Achievement03";
        achievement.percentCompleted = 0;
        achievement.ReportProgress(result => {
            if (result)
                Debug.Log("Successfully reported progress");
            else
                Debug.Log("Failed to report progress");
        });
    }

    void ReceivedTenStars()
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = "Achievement04";
        achievement.percentCompleted = 0;
        achievement.ReportProgress(result => {
            if (result)
                Debug.Log("Successfully reported progress");
            else
                Debug.Log("Failed to report progress");
        });
    }
    void ReceivedThirtyStars()
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = "Achievement05";
        achievement.percentCompleted = 0;
        achievement.ReportProgress(result => {
            if (result)
                Debug.Log("Successfully reported progress");
            else
                Debug.Log("Failed to report progress");
        });
    }

    void ReportAchievement(string achievementID, double progress = 100.0)
    {
        Social.ReportProgress(achievementID, progress, success => {
            if (success)
            {
                Debug.Log("Conquista reportada com sucesso.");
            }
            else
            {
                Debug.Log("Falha ao reportar conquista.");
            }
        });
    }

    private void OnEnable()
    {
        AdEvents.Player_Initialized += CreateAchievements;
        AdEvents.Achievement_Completed += ReportAchievement;
    }
    private void OnDisable()
    {
        AdEvents.Player_Initialized -= CreateAchievements;
        AdEvents.Achievement_Completed -= ReportAchievement;
    }
}
