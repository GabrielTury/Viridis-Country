using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.UI;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;

public class AnalyticsManager : MonoBehaviour
{
    [SerializeField] Text consoleOutput;
    [SerializeField] ScrollRect consoleScrollRect;
    void Awake()
    {
        Application.logMessageReceived += OnLogMessageReceived;
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();

        Debug.Log($"Started UGS Analytics Sample with user ID: {AnalyticsService.Instance.GetAnalyticsUserID()}");
    }

    [ContextMenu("Consent")]
    public void GiveConsent()
    {
        AnalyticsService.Instance.StartDataCollection();

        Debug.Log($"Consent has been provided. The SDK is now collecting data!");
    }

    void OnLogMessageReceived(string condition, string stacktrace, LogType type)
    {
        if (consoleOutput == null)
            return;

        consoleOutput.text += $"{type}: {condition}\n";
        consoleScrollRect.normalizedPosition = Vector2.zero;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    [ContextMenu("OnMove")]

    public void Teste()
    {
        OnMove();
    }
    public static void OnMove()
    {
        string levelPlayed = SceneManager.GetActiveScene().name.ToString();
        MoveEvent moveEvent = new MoveEvent
        {
            numberOfMoves = GameManager.Instance.actionsMade,
            level = levelPlayed
        };

        AnalyticsService.Instance.RecordEvent(moveEvent);
    }
}

namespace Unity.Services.Analytics
{
    public class MoveEvent : Event //Eh melhor fazer eventos que o proprio evento eh uma informacao com os parametros para ajudar a decidir o que eh util do que n
    {
        public MoveEvent() : base("moveEvent")
        {

        }

        public int numberOfMoves { set { SetParameter("numberOfMoves", value); } }
        public string level { set { SetParameter("level", value); } }
    }
}

