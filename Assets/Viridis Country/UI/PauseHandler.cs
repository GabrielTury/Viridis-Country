using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseHandler : MonoBehaviour
{
    private AudioMixer AudioMixer;

    [SerializeField]
    private Image soundIcon;
    [SerializeField]
    private Sprite[] soundIconImages;

    [SerializeField]
    private Image musicIcon;
    [SerializeField]
    private Sprite[] musicIconImages;

    [SerializeField]
    private Image shadow;

    [SerializeField]
    private Image foreground;

    [SerializeField]
    private Image background;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn(0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchSound()
    {
        AudioMixer.GetFloat("", out float volume);
        AudioMixer.SetFloat("", volume == 1 ? 0 : 1);
        PlayerPrefs.SetFloat("SOUNDVOLUME", volume == 1 ? 0 : 1);
        soundIcon.sprite = soundIconImages[(int)volume];
    }

    public void SwitchMusic()
    {
        AudioMixer.GetFloat("", out float volume);
        AudioMixer.SetFloat("", volume == 1 ? 0 : 1);
        PlayerPrefs.SetFloat("MUSICVOLUME", volume == 1 ? 0 : 1);
        musicIcon.sprite = musicIconImages[(int)volume];
    }

    public void ResumeLevel()
    {
        StartCoroutine(FadeOut(0.4f, 0));
    }

    public void RestartLevel()
    {
        StartCoroutine(FadeOut(0.4f, 1));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(FadeOut(0.4f, 2));
    }



    private IEnumerator FadeIn(float duration)
    {
        Color startColor = shadow.color;
        float lerp = 0;
        float smoothLerp = 0;
        Vector2 startPosition = new Vector2(0, 1870);
        Vector2 targetPosition = new Vector2(0, 0);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            shadow.color = Color.Lerp(startColor, new Color(0, 0, 0, 0.5f), smoothLerp);
            background.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

        shadow.color = new Color(0, 0, 0, 0.5f);
    }

    private IEnumerator FadeOut(float duration, int type)
    {
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

        switch (type)
        {
            case 0:
                break;

            case 1:
                // reiniciar o level
                break;

            case 2:
                SceneManager.LoadScene("MainMenu");
                break;
        }
        shadow.color = new Color(0, 0, 0, 0);
        Destroy(this.gameObject);
    }
}
