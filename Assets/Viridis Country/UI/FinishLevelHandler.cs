using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLevelHandler : MonoBehaviour
{
    [SerializeField]
    private Image[] stars;

    [SerializeField]
    private Sprite[] starSprites;

    [SerializeField]
    private Image[] buttonImages;

    [SerializeField]
    private Image trophy;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image shadow;

    [SerializeField]
    private Image foreground;

    // Start is called before the first frame update
    void Start()
    {
        // Set amount of stars

        if (GameManager.Instance.actionsMade <= GameManager.Instance.levelVariables.threeStarsAmount)
        {
            // keep
        }
        else if (GameManager.Instance.actionsMade <= GameManager.Instance.levelVariables.twoStarsAmount)
        {
            stars[2].sprite = starSprites[1];
        }
        else if (GameManager.Instance.actionsMade >= GameManager.Instance.levelVariables.oneStarAmount)
        {
            stars[2].sprite = starSprites[1];
            stars[1].sprite = starSprites[1];
        }

        //

        foreground.gameObject.SetActive(false);

        // Positioning

        stars[0].rectTransform.anchoredPosition = new Vector2(-100, 1100);
        stars[1].rectTransform.anchoredPosition = new Vector2(0, 1100);
        stars[2].rectTransform.anchoredPosition = new Vector2(100, 1100);

        StartCoroutine(SmoothMoveTo(stars[0], new Vector2(-180, 358), 0.8f));
        StartCoroutine(SmoothMoveTo(stars[1], new Vector2(0, 402), 1f));
        StartCoroutine(SmoothMoveTo(stars[2], new Vector2(180, 358), 1.2f));

        trophy.rectTransform.anchoredPosition = new Vector2(0, -1100);

        StartCoroutine(SmoothMoveTo(trophy, new Vector2(0, 0), 1.4f));

        buttonImages[0].rectTransform.anchoredPosition = new Vector2(-100, -1100);
        buttonImages[1].rectTransform.anchoredPosition = new Vector2(0, -1100);
        buttonImages[2].rectTransform.anchoredPosition = new Vector2(100, -1100);

        StartCoroutine(SmoothMoveTo(buttonImages[0], new Vector2(-208, -363), 1.2f));
        StartCoroutine(SmoothMoveTo(buttonImages[1], new Vector2(0, -363), 1f));
        StartCoroutine(SmoothMoveTo(buttonImages[2], new Vector2(208, -363), 0.8f));

        StartCoroutine(SmoothMoveTo(background, new Vector2(0, 0), 1.6f));

        // Coloring

        stars[0].color = new Color32(150, 150, 150, 0);
        stars[1].color = new Color32(150, 150, 150, 0);
        stars[2].color = new Color32(150, 150, 150, 0);

        StartCoroutine(FadeColor(stars[0], new Color32(255, 255, 255, 255), 0.6f));
        StartCoroutine(FadeColor(stars[1], new Color32(255, 255, 255, 255), 0.6f));
        StartCoroutine(FadeColor(stars[2], new Color32(255, 255, 255, 255), 0.6f));

        trophy.color = new Color32(150, 150, 150, 0);

        StartCoroutine(FadeColor(trophy, new Color32(255, 255, 255, 255), 0.6f));

        shadow.color = new Color32(0, 0, 0, 0);

        StartCoroutine(FadeColor(shadow, new Color32(0, 0, 0, 150), 1));

        background.color = new Color32(0, 0, 0, 0);

        StartCoroutine(FadeColor(background, new Color32(255, 255, 255, 255), 0.8f));

        // Scaling

        stars[0].rectTransform.localScale = new Vector2(2, 2);
        stars[1].rectTransform.localScale = new Vector2(2, 2);
        stars[2].rectTransform.localScale = new Vector2(2, 2);

        StartCoroutine(ScaleObject(stars[0], new Vector2(1, 1), 1));
        StartCoroutine(ScaleObject(stars[1], new Vector2(1, 1), 1));
        StartCoroutine(ScaleObject(stars[2], new Vector2(1, 1), 1));

        trophy.rectTransform.localScale = new Vector2(0, 0);

        StartCoroutine(ScaleObject(trophy, new Vector2(1, 1), 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        StartCoroutine(FadeOut(0.4f, 0));
    }

    public void RestartLevel()
    {
        StartCoroutine(FadeOut(0.4f, 1));
    }

    public void ReturnHome()
    {
        StartCoroutine(FadeOut(0.4f, 2));
    }

    private IEnumerator SmoothMoveTo(Image obj, Vector2 targetPosition, float duration)
    {
        float lerp = 0;
        float smoothLerp = 0;
        Vector2 startPosition = obj.rectTransform.anchoredPosition;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

    }

    private IEnumerator FadeColor(Image image, Color32 targetColor, float duration)
    {
        Color32 startColor = image.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            yield return null;
        }

        image.color = targetColor;
    }

    private IEnumerator ScaleObject(Image obj, Vector2 targetScale, float duration)
    {
        float lerp = 0;
        float smoothLerp = 0;
        Vector2 startPosition = obj.rectTransform.localScale;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startPosition, targetScale, smoothLerp);
            yield return null;
        }
    }

    private IEnumerator FadeOut(float duration, int type)
    {
        foreground.gameObject.SetActive(true);

        Color startColor = shadow.color;
        Color startColor2 = foreground.color;
        float lerp = 0;
        float smoothLerp = 0;
        Vector2 startPosition = background.rectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(0, -1870);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            shadow.color = Color.Lerp(startColor, new Color(0, 0, 0, 0), smoothLerp);
            background.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            if (type == 1 || type == 2)
            {
                foreground.color = Color.Lerp(startColor, new Color(0, 0, 0, 255), smoothLerp);
            }
            yield return null;
        }

        switch (type) // polir
        {
            case 0:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            case 1:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            case 2:
                SceneManager.LoadScene("MainMenu");
                break;
        }
        shadow.color = new Color(0, 0, 0, 0);
        Destroy(this.gameObject);
    }
}
