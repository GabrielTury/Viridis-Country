using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField]
    private int levelID;

    [SerializeField]
    private Image star1;

    [SerializeField]
    private Image star2;

    [SerializeField]
    private Image star3;

    private Image selfImage;

    private Color32 selfColor;

    // Start is called before the first frame update
    void Start()
    {
        selfImage = GetComponent<Image>();

        selfColor = selfImage.color;

        /* 
        if (<previous stage is not completed>) {
            selfImage.color = new Color32(selfImage.color.r - 50, selfImage.color.g - 50, selfImage.color.b - 50, 155);
        }
        */

        star1.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star3.rectTransform.anchoredPosition = new Vector2(0, 0);

        star1.color = new Color32(255, 255, 255, 0);
        star2.color = new Color32(255, 255, 255, 0);
        star3.color = new Color32(255, 255, 255, 0);

        selfImage.rectTransform.localScale = new Vector2(1, 0);

        StartCoroutine(SmoothMoveToWithDelay(star1, new Vector2(-125, 175), 0.2f, levelID * 0.05f));
        StartCoroutine(SmoothMoveToWithDelay(star2, new Vector2(0, 200), 0.2f, levelID * 0.05f));
        StartCoroutine(SmoothMoveToWithDelay(star3, new Vector2(125, 175), 0.2f, levelID * 0.05f));

        StartCoroutine(FadeColorWithDelay(star1, new Color32(255, 255, 255, 255), 0.2f, levelID * 0.05f));
        StartCoroutine(FadeColorWithDelay(star2, new Color32(255, 255, 255, 255), 0.2f, levelID * 0.05f));
        StartCoroutine(FadeColorWithDelay(star3, new Color32(255, 255, 255, 255), 0.2f, levelID * 0.05f));

        StartCoroutine(SmoothScaleWithDelay(selfImage, new Vector2(1,1), 0.2f, levelID * 0.05f));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SmoothMoveToWithDelay(Image obj, Vector2 targetPosition, float duration, float delay = 0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay + 0.1f);
        }
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

    private IEnumerator FadeColorWithDelay(Image image, Color32 targetColor, float duration, float delay = 0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay + 0.1f);
        }
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

    private IEnumerator SmoothScaleWithDelay(Image obj, Vector2 targetScale, float duration, float delay = 0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay + 0.1f);
        }
        float lerp = 0;
        float smoothLerp = 0;
        Vector2 startScale = obj.rectTransform.localScale;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

    }
}
