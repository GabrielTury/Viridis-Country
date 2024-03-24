using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    void Start()
    {
        planetTransform = planet.transform;
        cameraHolderTransform = cameraHolder.transform; 
        galaxyTransform = galaxy.transform;
        gameLogoRTransform = gameLogoImage.rectTransform;
        mainCameraTransform = mainCamera.transform;
        mainCameraTransform.localPosition = new Vector3(0, 0.525f, -1.277f);

        StartCoroutine(SmoothStepToTarget(mainCameraTransform, new Vector3(0, 1.48f, -4.87f), 2));

        blackoutImage.gameObject.SetActive(true);
        blackoutImageCoroutine = StartCoroutine(FadeColor(blackoutImage, new Color(0, 0, 0, 0), 1));

        levelSelectBGImage.gameObject.SetActive(false);

        tooltipImage.color = new Color(255, 255, 255, 0);
    }

    void Update()
    {
        cameraHolderTransform.Rotate(0, earthRotationSpeed * Time.deltaTime, 0);
        //galaxyTransform.Rotate(0, galaxyRotationSpeed * Time.deltaTime, 0);
        galaxyTransform.eulerAngles = new Vector3(-90, galaxyRotationSpeed * Time.deltaTime, 0);

        gameLogoRTransform.anchoredPosition = new Vector3(0, (Mathf.Sin(Time.time) * 10) - 320.0f, 0.0f);

        if (showInputTooltip == false && Time.time > 3)
        {
            StartCoroutine(FadeColor(tooltipImage, new Color(255, 255, 255, 255), 60));
            showInputTooltip = true;
        }
    }

    public void GoToLevelSelect()
    {
        transitionCamera.SetActive(true);
        Vector3 cameraPositionTarget = transitionCamera.transform.position;
        Vector3 cameraRotationTarget = transitionCamera.transform.eulerAngles;
        transitionCamera.transform.position = mainCamera.transform.position;
        transitionCamera.transform.rotation = mainCamera.transform.rotation;
        StartCoroutine(SmoothStepToTarget(transitionCamera.transform, cameraPositionTarget, 2, cameraRotationTarget));
        blackoutImageCoroutine = StartCoroutine(FadeColor(blackoutImage, new Color(0, 0, 0, 255), 56));
        StartCoroutine(FadeColor(tooltipImage, new Color(255, 255, 255, 0), 60));
        StartCoroutine(FadeAfterSeconds(4));
        Destroy(mainCamera);
    }

    public void EnterLevel(int levelID)
    {
        StartCoroutine(FadeToLevel(1, 1));
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

    private IEnumerator FadeColor(Image image, Color targetColor, float duration)
    {
        Color startColor = image.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            image.color = Color.Lerp(startColor, targetColor, smoothLerp);
            yield return null;
        }

        image.color = targetColor;
    }

    private IEnumerator FadeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        StopCoroutine(blackoutImageCoroutine);
        StartCoroutine(FadeColor(blackoutImage, new Color(0, 0, 0, 0), 1));
        levelSelectBGImage.gameObject.SetActive(true);
    }

    private IEnumerator FadeToLevel(float seconds, int levelID)
    {
        StopCoroutine(blackoutImageCoroutine);
        StartCoroutine(FadeColor(blackoutImage, new Color(0, 0, 0, 255), 10 * seconds));
        yield return new WaitForSeconds(seconds);

        if (levelID == 1)
        {
            SceneManager.LoadScene("Gameplay_Test");
        }
    }
}
