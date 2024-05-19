using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using static GameManager;

public class ResourceTouchHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    private GameManager gameManager;

    public static ResourceTouchHandler Instance { get; private set; }

    [SerializeField]
    private GameObject managerObject;

    [SerializeField]
    private Canvas resourcePanelCanvas;

    private Image plateImage;

    [SerializeField]
    private TextMeshProUGUI actionCounter;

    [SerializeField]
    private Image bgFader;

    private bool[] GameResourcesToShow = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

    [SerializeField]
    private GameObject constructionPrefab;

    [System.Serializable]
    public class ResourceModel
    {
        public Sprite buildingIcon;
        public Sprite resourceIcon;
        public Color resourceColor;
        public Mesh resourceMesh; // ?
        public ConstructionTemplate resourceScriptableObject;

        public ResourceModel(Sprite buildingIcon, Sprite resourceIcon, Color resourceColor, Mesh resourceMesh, ConstructionTemplate resourceScriptableObject)
        {
            this.buildingIcon = buildingIcon;
            this.resourceIcon = resourceIcon;
            this.resourceColor = resourceColor;
            this.resourceMesh = resourceMesh;
            this.resourceScriptableObject = resourceScriptableObject;
        }
    } // nao queria deixar tudo public mas nao consegui fazer funcionar no inspetor como private

    public ResourceModel[] resourceModel;

    [SerializeField]
    private GameObject constructionPreview;

    [SerializeField]
    private GameObject buildingBoxTemplate;

    private Image constructionHeld;

    private ConstructionTemplate constructionHeldScriptableObject;

    private List<GameObject> buildingBoxes = new List<GameObject>();

    private int buildingBoxCount = 0;

    private Vector2 touchPos;
    private Vector2 platePrevPos;
    private Coroutine movementCoroutine;
    private Coroutine blackoutCoroutine;
    private Coroutine handCoroutine;
    private Coroutine plateColorCoroutine;
    private Coroutine trashCoroutine;
    private bool smoothReturnEnded = true;
    private bool isFocused = false;
    private bool isHoldingBuilding = false;
    public bool isOnTrash = false;
    private bool priorityTrash = false;

    private GameObject lastSelected;
    private GameObject lastBuildingBox;
    private GameObject buildingToTrash;

    [SerializeField]
    private Image handObj;

    [SerializeField]
    private Sprite[] buildingBoxSprites;

    [SerializeField]
    private GameObject clearLevelTemplate;

    private Inputs input;

    [SerializeField]
    private Image trash;

    [SerializeField]
    private Image trashHelper;

    private PointerEventData cachedPointer;
    

    private void OnEnable()
    {
        GameEvents.Construction_Placed += UpdateActionText;
        GameEvents.Construction_Placed += UpdateHandAnim;
        GameEvents.Level_End += ClearLevel;
        GameEvents.Select_Construction += UpdateHandAnimGrow;
    }

    private void OnDisable()
    {
        GameEvents.Construction_Placed -= UpdateActionText;
        GameEvents.Construction_Placed -= UpdateHandAnim;
        GameEvents.Level_End -= ClearLevel;
        GameEvents.Select_Construction -= UpdateHandAnimGrow;
    }

    private void ClearLevel()
    {
        Instantiate(clearLevelTemplate);
    }

    private void UpdateActionText(AudioManager.ConstructionAudioTypes a)
    {
        actionCounter.text = GameManager.Instance.actionsMade.ToString();
    }

    private void UpdateHandAnim(AudioManager.ConstructionAudioTypes a)
    {
        StopCoroutine(handCoroutine);
        handCoroutine = StartCoroutine(HandInflate(handObj, new Vector2(0.8f, 0.8f), new Color32(255, 255, 255, 150), 0.3f));
    }

    private void UpdateHandAnimGrow(AudioManager.SoundEffects a)
    {
        StopCoroutine(handCoroutine);
        handCoroutine = StartCoroutine(HandInflate(handObj, new Vector2(1.2f, 1.2f), new Color32(255, 255, 255, 255), 0.3f));
    }

    public void RaiseTrash(GameObject refObj)
    {
        StopCoroutine(movementCoroutine);
        StopCoroutine(blackoutCoroutine);
        StopCoroutine(trashCoroutine);
        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -825), 0.1f));
        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
        trashCoroutine = StartCoroutine(SmoothReturn(trash, new Vector2(0, -600), 0.1f));
        buildingToTrash = refObj;
        priorityTrash = true;
        //trashHelper.gameObject.SetActive(true);
        trashHelper.raycastTarget = true;
    }

    public void LowerTrash()
    {
        priorityTrash = false;
        trashHelper.raycastTarget = false;
        trashHelper.gameObject.SetActive(false);
        trashCoroutine = StartCoroutine(SmoothReturn(trash, new Vector2(0, -1300), 0.1f));
        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.3f));
    }


    private void Awake()
    {
        input = new Inputs();
    }

    private void Start()
    {
        Instance = this;

        gameManager = GameManager.Instance;

        actionCounter.text = gameManager.actionsMade.ToString();

        managerObject.GetComponent<InputManager>().canDrag = true;

        plateImage = GetComponent<Image>();

        trashHelper.raycastTarget = false;

        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 1));
        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 1f));
        handCoroutine = StartCoroutine(HandInflate(handObj, new Vector2(0.8f, 0.8f), new Color32(255, 255, 255, 150), 0.3f));
        plateColorCoroutine = StartCoroutine(FadeColor(plateImage, new Color32(255, 255, 255, 200), 1f));
        trashCoroutine = StartCoroutine(SmoothReturn(trash, new Vector2(0, -1300), 0.1f));

        //trashHelper.gameObject.SetActive(false);

        GameResourcesToShow[0] = false;
        GameResourcesToShow[1] = gameManager.objectiveWood > 0 ? true : false;
        GameResourcesToShow[2] = gameManager.objectivePlank > 0 ? true : false;
        GameResourcesToShow[3] = gameManager.objectiveStone > 0 ? true : false;
        GameResourcesToShow[4] = gameManager.objectiveProcessedStone > 0 ? true : false;
        GameResourcesToShow[5] = gameManager.objectiveConstructionMaterials > 0 ? true : false;
        GameResourcesToShow[6] = gameManager.objectiveWater > 0 ? true : false;
        GameResourcesToShow[7] = gameManager.objectiveMilk > 0 ? true : false;
        GameResourcesToShow[8] = gameManager.objectiveFermentedMilk > 0 ? true : false;
        GameResourcesToShow[9] = gameManager.objectiveCheese > 0 ? true : false;
        GameResourcesToShow[10] = gameManager.objectiveSkin > 0 ? true : false;
        GameResourcesToShow[11] = gameManager.objectiveLeather > 0 ? true : false;
        GameResourcesToShow[12] = gameManager.objectiveWool > 0 ? true : false;
        GameResourcesToShow[13] = gameManager.objectiveCloth > 0 ? true : false;
        GameResourcesToShow[14] = gameManager.objectiveClothes > 0 ? true : false;
        GameResourcesToShow[15] = gameManager.objectiveWheat > 0 ? true : false;
        GameResourcesToShow[16] = gameManager.objectiveFlour > 0 ? true : false;
        GameResourcesToShow[17] = gameManager.objectiveBread > 0 ? true : false;
        GameResourcesToShow[18] = gameManager.objectiveGold > 0 ? true : false;

        for (int i = 0; i < GameResourcesToShow.Length; i++)
        {
            if (GameResourcesToShow[i] == true)
            {
                buildingBoxCount += 1;
                var clone = Instantiate(buildingBoxTemplate, this.transform);
                clone.name = "BuildingBox" + i;
                buildingBoxes.Add(clone);
            }
        }

        for (int i = 0; i < buildingBoxCount; i++)
        {
            int boxResourceType = int.Parse(new string(buildingBoxes[i].name.Where(char.IsDigit).ToArray()));

            if (buildingBoxCount > 6)
            {
                buildingBoxes[i].GetComponent<Image>().rectTransform.localScale = new Vector2(0.75f, 0.75f);
                buildingBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2((i % 3) * 275 - 275, -(i / 3) * 275 + 466);
            }
            else
            {
                buildingBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2((i % 2) * 420 - 210, -(i / 2) * 440 + 466);
            }
            

            GetChildWithName(buildingBoxes[i], "BuildingIcon").GetComponent<Image>().sprite = resourceModel[boxResourceType].buildingIcon;
            GetChildWithName(buildingBoxes[i], "BuildingResourceIcon").GetComponent<Image>().sprite = resourceModel[boxResourceType].resourceIcon;
            buildingBoxes[i].GetComponent<Image>().color = resourceModel[boxResourceType].resourceColor;
        }
    }

    private void Update()
    {
        trashHelper.rectTransform.anchoredPosition = Input.mousePosition;
        if (priorityTrash)
        {
            //Debug.Log("Mouse: " + Input.mousePosition +
            //    "\nanchored: " + trash.rectTransform.anchoredPosition);

            //Debug.Log("local: " + trash.rectTransform.localPosition);

            //Debug.Log(trash.rectTransform.anchoredPosition);

            //Debug.Log(EventSystem.current);

            var results = new List<RaycastResult>();
            
            EventSystem.current.RaycastAll(cachedPointer, results);
            
            foreach (RaycastResult hit in results)
            {
                if (hit.gameObject.CompareTag("Trash"))
                {
                    isOnTrash = true;
                    trash.color = Color.Lerp(trash.color, new Color32(255, 55, 55, 215), 0.1f);
                    return;
                }
            }

            trash.color = Color.Lerp(trash.color, new Color32(255, 255, 255, 100), 0.1f);
            isOnTrash = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cachedPointer = eventData;
        Debug.Log("cached");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //cachedPointer = eventData;
        Debug.Log("Drag Begin");
        touchPos = eventData.pointerCurrentRaycast.screenPosition;
        platePrevPos = plateImage.rectTransform.anchoredPosition;
        if (eventData.pointerCurrentRaycast.gameObject != null || priorityTrash)
        {
            if (priorityTrash)
            {
                return;
            }
            if (eventData.pointerCurrentRaycast.gameObject.name == "ResourcePlate")
            {
                managerObject.GetComponent<InputManager>().canDrag = false;
                StopCoroutine(movementCoroutine);
                lastSelected = eventData.pointerCurrentRaycast.gameObject;
            }
            if (eventData.pointerCurrentRaycast.gameObject.name.Contains("Box") || eventData.pointerCurrentRaycast.gameObject.name.Contains("Icon") && isFocused)
            {
                managerObject.GetComponent<InputManager>().canDrag = false;
                StopCoroutine(movementCoroutine);
                StopCoroutine(blackoutCoroutine);
                StopCoroutine(trashCoroutine);
                movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -825), 0.1f));
                blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
                trashCoroutine = StartCoroutine(SmoothReturn(trash, new Vector2(0, -600), 0.1f));

                constructionHeldScriptableObject = resourceModel[GetResourceIndex(eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite.name.Remove(0, 16))].resourceScriptableObject;

                lastBuildingBox = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
                lastBuildingBox.GetComponent<Image>().sprite = buildingBoxSprites[1];

                constructionHeld = Instantiate(constructionPreview, resourcePanelCanvas.transform).GetComponent<Image>();
                constructionHeld.sprite = eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite;
                constructionHeld.color = new Color32(255, 255, 255, 0);
                StartCoroutine(ConstructionPreviewAnim(constructionHeld));

                isHoldingBuilding = true;

                StopCoroutine(handCoroutine);
                handCoroutine = StartCoroutine(HandInflate(handObj, new Vector2(1.2f, 1.2f), new Color32(255, 255, 255, 255), 0.3f));
                StopCoroutine(plateColorCoroutine);
                plateColorCoroutine = StartCoroutine(FadeColor(plateImage, resourceModel[GetResourceIndex(eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite.name.Remove(0, 16))].resourceColor, 1f));
            }
        }
    }

    private int GetResourceIndex(string resourceName)
    {
        for (int i = 0; i < resourceModel.Length; i++)
        {
            if (resourceModel[i].resourceIcon.name.Contains(resourceName))
            {
                //Debug.LogWarning("found icon! name: " + resourceName + " index: " + i);
                return i;
            }
        }
        //Debug.LogWarning("Resource icon with name " + resourceName + " not found");
        return 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging: " + eventData.pointerCurrentRaycast.gameObject.name);
        if(eventData.pointerCurrentRaycast.gameObject == null)
        {
            return;
        }
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (lastSelected != null)
            {
                if (isFocused)
                {
                    if (plateImage.rectTransform.anchoredPosition.y <= 200)
                    {
                        //Debug.Log("ABC");
                        StopCoroutine(movementCoroutine);
                        StopCoroutine(blackoutCoroutine);
                        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.3f));
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.4f));
                        isFocused = false;
                        managerObject.GetComponent<InputManager>().canDrag = true;
                        lastSelected = null;
                        eventData.pointerDrag = null;
                    }
                    else
                    {
                        plateImage.rectTransform.anchoredPosition = new Vector2(0, platePrevPos.y + (touchPos.y - eventData.pointerCurrentRaycast.screenPosition.y) * -1);
                    }
                } else
                {
                    if (plateImage.rectTransform.anchoredPosition.y >= -400)
                    {
                        //Debug.Log("CBA");
                        StopCoroutine(movementCoroutine);
                        StopCoroutine(blackoutCoroutine);
                        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, 800), 0.3f));
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 180), 0.4f));
                        isFocused = true;
                        lastSelected = null;
                        eventData.pointerDrag = null;
                    }
                    else
                    {
                        plateImage.rectTransform.anchoredPosition = new Vector2(0, platePrevPos.y + (touchPos.y - eventData.pointerCurrentRaycast.screenPosition.y) * -1);
                    }
                }
            } else
            {
                if (eventData.pointerCurrentRaycast.gameObject.name.Contains("Preview") || isHoldingBuilding == true || priorityTrash == true)
                {
                    if (constructionHeld)
                    {
                        constructionHeld.rectTransform.anchoredPosition = eventData.position;
                    }
                    if (priorityTrash)
                    {
                        trashHelper.rectTransform.anchoredPosition = eventData.position;
                    }
                    

                    //Debug.Log("constructionPos.x = " + constructionHeld.rectTransform.anchoredPosition.x + ", \nconstruction.y = " + constructionHeld.rectTransform.anchoredPosition.y);

                    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(eventData, results);

                    // For each object that the raycast hits.
                    foreach (RaycastResult hit in results)
                    {

                        if (hit.gameObject.CompareTag("Trash"))
                        {
                            isOnTrash = true;
                            trash.color = Color.Lerp(trash.color, new Color32(255, 55, 55, 215), 0.1f);
                            return;
                        }
                    }

                    trash.color = Color.Lerp(trash.color, new Color32(255, 255, 255, 100), 0.1f);
                    isOnTrash = false;

                    //Debug.Log("Position x: " + eventData.position.x + " Position y: " + eventData.position.y);
                    //Debug.Log("constructionHeld.x = " + constructionHeld.rectTransform.anchoredPosition.x + ", constructionHeld.y = " + constructionHeld.rectTransform.anchoredPosition.y);
                    //Debug.Log("touchPos.x = " + eventData.pointerCurrentRaycast.screenPosition.x + ", touchPos.y = " + eventData.pointerCurrentRaycast.screenPosition.y);
                }
            }
        }
        else
        {
            if (isFocused)
            {

            } else
            {
                /*StopCoroutine(movementCoroutine);
                StopCoroutine(blackoutCoroutine);
                movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 1));
                blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
                isFocused = false;
                eventData.pointerDrag = null;*/
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag Ended");
        managerObject.GetComponent<InputManager>().canDrag = true;
        if (smoothReturnEnded == true)
        {
            //Debug.Log("DEF");
            StopCoroutine(movementCoroutine);
            StopCoroutine(blackoutCoroutine);
            movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.4f));
            blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.4f));
            lastSelected = null;
            isFocused = false;
        }

        if (isHoldingBuilding == true)
        {
            lastSelected = null;

            if (!isOnTrash)
            {
                Vector3 buildPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.pointerCurrentRaycast.screenPosition.x, eventData.pointerCurrentRaycast.screenPosition.y, Camera.main.nearClipPlane));

                Transform transformCam = Camera.main.transform;

                for (float i = 0; buildPosition.y > 0.5f; i = i + 0.1f)
                {
                    buildPosition += transformCam.forward * Time.deltaTime * i;
                }

                Debug.Log(buildPosition);

                GameObject constructionPlaced = Instantiate(constructionPrefab, buildPosition, Quaternion.Euler(new Vector3(0, -90, 0)));
                constructionPlaced.SetActive(false);
                constructionPlaced.GetComponent<Construction>().construcion = constructionHeldScriptableObject;
                constructionPlaced.SetActive(true);
                constructionPlaced.GetComponent<Construction>().SetDragging(false);
                //constructionPlaced.GetComponent<Construction>().SetDragging(true);
            } else
            {
                if (buildingToTrash)
                {
                    Destroy(buildingToTrash);
                    trashHelper.raycastTarget = false;
                    //trashHelper.gameObject.SetActive(false);
                }
            }

            isOnTrash = false;

            //constructionPlaced.GetComponent<Construction>().GetResourcesInRange
            Destroy(constructionHeld.gameObject);
            
            isHoldingBuilding = false;

            //GameManager.Instance.actionsMade++;
            actionCounter.text = GameManager.Instance.actionsMade.ToString();

            StopCoroutine(handCoroutine);
            handCoroutine = StartCoroutine(HandInflate(handObj, new Vector2(0.8f, 0.8f), new Color32(255, 255, 255, 150), 0.3f));

            StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.4f));

            StopCoroutine(trashCoroutine);
            trashCoroutine = StartCoroutine(SmoothReturn(trash, new Vector2(0, -1300), 0.1f));
        }

        if (lastBuildingBox != null)
        {
            lastBuildingBox.GetComponent<Image>().sprite = buildingBoxSprites[0];
        }
    }

    private IEnumerator ConstructionPreviewAnim(Image obj)
    {
        Vector2 startScale = obj.rectTransform.localScale;
        Color32 startColor = obj.color;
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        Vector2 scale = new Vector2(2.5f, 2.5f);
        Color32 color = new Color32(255, 255, 255, 200);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startScale, scale, smoothLerp);
            obj.color = Color.Lerp(startColor, color, smoothLerp);
            yield return null;
        }

        obj.rectTransform.localScale = scale;
        obj.color = color;

        startScale = obj.rectTransform.localScale;
        lerp = 0;
        smoothLerp = 0;
        duration = 0.2f;
        scale = new Vector2(1.8f, 1.8f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            obj.rectTransform.localScale = Vector2.Lerp(startScale, scale, smoothLerp);
            yield return null;
        }

        obj.rectTransform.localScale = scale;

        yield return null;
    }

    private IEnumerator HandInflate(Image hand, Vector2 scale, Color32 color, float duration)
    {
        Vector2 startScale = hand.rectTransform.localScale;
        Color32 startColor = hand.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            hand.rectTransform.localScale = Vector2.Lerp(startScale, scale, smoothLerp);
            hand.color = Color32.Lerp(startColor, color, smoothLerp);
            yield return null;
        }

        hand.rectTransform.localScale = scale;
        hand.color = color;
        yield return null;
    }

    private IEnumerator SmoothReturn(Image objTransform, Vector2 targetPosition, float duration)
    {
        smoothReturnEnded = false;
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
        smoothReturnEnded = true;
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

    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }


}
