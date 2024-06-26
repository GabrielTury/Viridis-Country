using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MainMenuManager : MonoBehaviour
{
    #region Background
    [SerializeField]
    private GameObject planet;
    private Transform planetTransform;

    [SerializeField]
    private GameObject cameraHolder;
    private Transform cameraHolderTransform;

    [SerializeField]
    private GameObject mainCamera;
    private Transform mainCameraTransform;
    private Vector3 mainCameraVelocity = Vector3.zero;
    private Coroutine cameraBegin;

    [SerializeField]
    private GameObject galaxy;
    private Transform galaxyTransform;

    [SerializeField]
    private float earthRotationSpeed;

    [SerializeField]
    private float galaxyRotationSpeed;
    #endregion

    [SerializeField]
    private Canvas sceneCanvas;

    [SerializeField]
    private Image gameLogoImage;
    private RectTransform gameLogoRTransform;

    [SerializeField]
    private Image blackoutImage;
    private Coroutine blackoutImageCoroutine;

    [SerializeField]
    private GameObject transitionCamera;

    [SerializeField]
    private Image levelSelectBGImage;

    [SerializeField]
    private Image tooltipImage;

    private bool showInputTooltip;

    [SerializeField]
    private GameObject analyticsPopup;

    private bool hasGoneToLevelSel = false;
    private bool hasGoneToStage = false;

    [SerializeField]
    private Image energyHandler;
    private Coroutine energyShakeCoroutine;

    [SerializeField]
    private GameObject buyEnergyMenuPrefab;

    [SerializeField]
    private GameObject levelEarth;

    [SerializeField]
    private AudioMixer audioMixer;

    void Start()
    {
        planetTransform = planet.transform;
        cameraHolderTransform = cameraHolder.transform;
        galaxyTransform = galaxy.transform;
        gameLogoRTransform = gameLogoImage.rectTransform;
        mainCameraTransform = mainCamera.transform;
        mainCameraTransform.localPosition = new Vector3(0, 0.525f, -1.277f);

        cameraBegin = StartCoroutine(SmoothStepToTarget(mainCameraTransform, new Vector3(0, 1.48f, -4.87f), 2));

        blackoutImage.gameObject.SetActive(true);
        blackoutImageCoroutine = StartCoroutine(FadeColor(blackoutImage, new Color32(0, 0, 0, 0), 1));

        levelSelectBGImage.gameObject.SetActive(false);

        tooltipImage.color = new Color(255, 255, 255, 0);

        energyShakeCoroutine = StartCoroutine(EmptyCoroutine());
    }

    void Update()
    {
        cameraHolderTransform.Rotate(0, earthRotationSpeed * Time.deltaTime, 0);
        //galaxyTransform.Rotate(0, galaxyRotationSpeed * Time.deltaTime, 0);
        galaxyTransform.eulerAngles = new Vector3(-90, galaxyRotationSpeed * Time.deltaTime, 0);

        gameLogoRTransform.anchoredPosition = new Vector3(0, (Mathf.Sin(Time.time) * 10) - 320.0f, 0.0f);

        if (showInputTooltip == false && Time.time > 3)
        {
            StartCoroutine(FadeColor(tooltipImage, new Color32(255, 255, 255, 255), 1));
            showInputTooltip = true;
        }
    }

    public void GoToLevelSelect()
    {
        if (hasGoneToLevelSel == false)
        {
            hasGoneToLevelSel = true;
            //transitionCamera.SetActive(true);
            //Vector3 cameraPositionTarget = transitionCamera.transform.position;
            //Vector3 cameraRotationTarget = transitionCamera.transform.eulerAngles;
            //transitionCamera.transform.position = mainCamera.transform.position;
            //transitionCamera.transform.rotation = mainCamera.transform.rotation;
            //StartCoroutine(SmoothStepToTarget(transitionCamera.transform, cameraPositionTarget, 2, cameraRotationTarget));
            StopCoroutine(cameraBegin);
            StartCoroutine(SmoothStepToTarget(mainCamera.transform, new Vector3(0, 0.525f, -1.277f), 2-0.75f));
            StopCoroutine(blackoutImageCoroutine);
            blackoutImageCoroutine = StartCoroutine(FadeColor(blackoutImage, new Color32(0, 0, 0, 255), 2-0.75f));
            StartCoroutine(FadeColor(tooltipImage, new Color(255, 255, 255, 0), 1));
            StartCoroutine(FadeAfterSeconds(2.5f-0.75f));
            //Destroy(mainCamera);
        }
    }

    public void EnterLevel(int levelID)
    {
        if (hasGoneToStage == false && SessionManager.Instance.energyAmount > 0)
        {
            if (levelID > 1 && levelID <= 19)
            {
                if (SessionManager.Instance.playerLevels["level " + (levelID - 1)] != 0)
                {
                    hasGoneToStage = true;
                    PlayerPrefs.SetInt("LEVELID", levelID);
                    StartCoroutine(FadeToLevel(0.75f, levelID));
                }
            } else
            {
                if (levelID > 19)
                {
                    Application.OpenURL("https://gabriel-tury.itch.io/viridis-country");
                    return;
                }
                hasGoneToStage = true;
                PlayerPrefs.SetInt("LEVELID", levelID);
                StartCoroutine(FadeToLevel(0.75f, levelID));
            }
        } else
        {
            StopCoroutine(energyShakeCoroutine);
            energyShakeCoroutine = StartCoroutine(ShakeEnergy(energyHandler));
        }
        
    }

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
    }

    private IEnumerator ShakeEnergy(Image obj)
    {
        Vector2 startScale = new Vector2(1, 1);
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        Vector2 scale = new Vector2(2f, 2f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startScale, scale, smoothLerp);
            yield return null;
        }

        obj.rectTransform.localScale = scale;

        startScale = obj.rectTransform.localScale;
        lerp = 0;
        smoothLerp = 0;
        duration = 0.2f;
        scale = new Vector2(1f, 1f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startScale, scale, smoothLerp);
            yield return null;
        }

        yield return null;
    }

    public void BuyEnergyPopup()
    {
        Instantiate(buyEnergyMenuPrefab, sceneCanvas.transform);
    }

    private IEnumerator SmoothStepToTarget(Transform objTransform, Vector3 targetPosition, float duration, Vector3 targetRotation = default)
    {
        Vector3 startPosition = objTransform.localPosition;
        Vector3 startRotation = objTransform.eulerAngles;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothLerp);
            if (targetRotation != default)
            {
                objTransform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, smoothLerp);
            }
            yield return null;
        }

        objTransform.localPosition = targetPosition;
        if (targetRotation != default)
        {
            objTransform.eulerAngles = targetRotation;
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

    private IEnumerator FadeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        StartCoroutine(SmoothMove(energyHandler, new Vector2(0, -170), 0.5F));

        StopCoroutine(blackoutImageCoroutine);
        StartCoroutine(FadeColor(blackoutImage, new Color32(0, 0, 0, 0), 1));
        levelEarth.SetActive(true);
        levelSelectBGImage.gameObject.SetActive(true);
    }

    private IEnumerator FadeToLevel(float seconds, int levelID)
    {
        StopCoroutine(blackoutImageCoroutine);
        StartCoroutine(FadeColor(blackoutImage, new Color32(0, 0, 0, 255), seconds));
        yield return new WaitForSeconds(seconds);

        SceneManager.LoadScene("Intermediary");
    }

    private IEnumerator SmoothMove(Image objTransform, Vector2 targetPosition, float duration)
    {
        Vector3 startPosition = objTransform.rectTransform.anchoredPosition;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objTransform.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

        objTransform.rectTransform.anchoredPosition = targetPosition;
    }
}
