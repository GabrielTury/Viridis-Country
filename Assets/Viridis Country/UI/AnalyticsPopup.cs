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

    [SerializeField]
    private List<string> analyticsStrings = new List<string>();

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image shadow;

    private Coroutine animationCoroutine;
    private Coroutine coloringCoroutine;

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (PlayerPrefs.GetInt("ANALYTICSENABLED") == 1)
        {
            AnalyticsService.Instance.StartDataCollection();
            Destroy(this);
        }

        background.rectTransform.localScale = new Vector2(0, 0);
        animationCoroutine = StartCoroutine(SlideIn(background, new Vector3(0, 0, 0), new Vector2(1,1), 1f, false));
        StartCoroutine(Coloring(shadow, new Color32(0, 0, 0, 100), 0.1f));
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
