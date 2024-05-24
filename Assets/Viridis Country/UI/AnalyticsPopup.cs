using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsPopup : MonoBehaviour
{
    [SerializeField]
    private string[] analyticsTitles;

    [SerializeField, TextAreaAttribute]
    private List<string> analyticsStrings = new List<string>();

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image shadow;

    private Coroutine animationCoroutine;
    private Coroutine coloringCoroutine;

    [SerializeField]
    private TextMeshProUGUI desc;

    // Start is called before the first frame update
    void Start()
    {
        AnalyticsManager an = FindObjectOfType<AnalyticsManager>();

        if (PlayerPrefs.GetInt("ANALYTICSENABLED") == 1)
        {
            an.GiveConsent();
            Destroy(this.gameObject);
        }

        background.rectTransform.localScale = new Vector2(0, 0);
        animationCoroutine = StartCoroutine(SlideIn(background, new Vector3(0, 0, 0), new Vector2(1,1), 1f, false));
        StartCoroutine(Coloring(shadow, new Color32(0, 0, 0, 100), 0.1f));

        int index = 0;

        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                index = 0;
                break;
            case SystemLanguage.Portuguese:
                index = 1;
                break;
            case SystemLanguage.Spanish:
                index = 2;
                break;
            case SystemLanguage.ChineseSimplified:
                index = 3;
                break;
            case SystemLanguage.ChineseTraditional:
                index = 4;
                break;
            case SystemLanguage.Japanese:
                index = 5;
                break;
            case SystemLanguage.Korean:
                index = 6;
                break;
            case SystemLanguage.Russian:
                index = 7;
                break;
            case SystemLanguage.Ukrainian:
                index = 8;
                break;
            case SystemLanguage.Vietnamese:
                index = 9;
                break;
            case SystemLanguage.Bulgarian:
                index = 10;
                break;
            case SystemLanguage.Czech:
                index = 11;
                break;
            case SystemLanguage.Danish:
                index = 12;
                break;
            case SystemLanguage.German:
                index = 13;
                break;
            case SystemLanguage.Greek:
                index = 14;
                break;
            case SystemLanguage.Catalan:
                index = 15;
                break;
            case SystemLanguage.Polish:
                index = 16;
                break;
            case SystemLanguage.Slovak:
                index = 17;
                break;
            case SystemLanguage.Slovenian:
                index = 18;
                break;
            case SystemLanguage.Thai:
                index = 19;
                break;
            case SystemLanguage.Turkish:
                index = 20;
                break;
            case SystemLanguage.Arabic:
                index = 21;
                break;
            case SystemLanguage.Italian:
                index = 22;
                break;
            case SystemLanguage.Finnish:
                index = 23;
                break;
            case SystemLanguage.French:
                index = 24;
                break;
            case SystemLanguage.Hebrew:
                index = 25;
                break;
            case SystemLanguage.Hungarian:
                index = 26;
                break;
            case SystemLanguage.Indonesian:
                index = 27;
                break;
            case SystemLanguage.Swedish:
                index = 28;
                break;
            default:
                index = 0;
                break;
                // Add more cases for other languages if needed
        }

        desc.text = analyticsStrings[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcceptAnalytics()
    {
        PlayerPrefs.SetInt("ANALYTICSENABLED", 1);
        StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(SlideIn(background, new Vector3(0, 2300, 0), new Vector2(0, 0), 0.6f, true));
        coloringCoroutine = StartCoroutine(Coloring(background, new Color32(117, 231, 134, 100), 0.25f));
        StartCoroutine(Coloring(shadow, new Color32(0, 0, 0, 0), 0.3f));
    }

    public void DeclineAnalytics()
    {
        PlayerPrefs.SetInt("ANALYTICSENABLED", 0);
        StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(SlideIn(background, new Vector3(0, 2300, 0), new Vector2(0, 0), 0.6f, true));
        coloringCoroutine = StartCoroutine(Coloring(background, new Color32(231, 117, 118, 100), 0.25f));
        StartCoroutine(Coloring(shadow, new Color32(0, 0, 0, 0), 0.3f));
    }

    private IEnumerator SlideIn(Image objImage, Vector3 targetPosition, Vector2 targetScale, float duration, bool kill = false)
    {
        Vector3 startPosition = objImage.rectTransform.anchoredPosition;
        Vector3 startScale = objImage.rectTransform.localScale;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, smoothLerp);
            objImage.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.anchoredPosition = targetPosition;

        if (kill)
        {
            Destroy(this.gameObject);
        }
        
        yield return null;
    }

    private IEnumerator Coloring(Image objImage, Color32 targetColor, float duration)
    {
        Color32 startColor = objImage.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            yield return null;
        }

        objImage.color = targetColor;
        yield return null;
    }
}
