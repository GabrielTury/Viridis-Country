using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [SerializeField]
    private RewardedAdsButton button;

    private void Awake()
    {
        InitializeAds();
        button.gameObject?.SetActive(false);
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        AdEvents.OnAdsInitialized();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    
    public void ActivateButton()
    {
        button.gameObject?.SetActive(true);
        button.LoadAd();
    }

    private void OnEnable()
    {
        AdEvents.Energy_Depleted += ActivateButton;
    }

    private void OnDisable()
    {
        AdEvents.Energy_Depleted -= ActivateButton;
    }
}
