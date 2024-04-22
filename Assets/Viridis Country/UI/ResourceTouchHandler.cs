using GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameManager;

public class ResourceTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameManager gameManager;

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

    Vector2 touchPos;
    Vector2 platePrevPos;
    Coroutine movementCoroutine;
    Coroutine blackoutCoroutine;
    bool smoothReturnEnded = true;
    bool isFocused = false;
    bool isHoldingBuilding = false;

    GameObject lastSelected;

    private Inputs input;

    private void OnEnable()
    {
        GameEvents.Construction_Placed += UpdateActionText;
    }

    private void OnDisable()
    {
        GameEvents.Construction_Placed -= UpdateActionText;
    }

    private void UpdateActionText(AudioManager.ConstructionAudioTypes a)
    {
        actionCounter.text = GameManager.Instance.actionsMade.ToString();
    }

    private void Awake()
    {
        input = new Inputs();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        managerObject.GetComponent<InputManager>().canDrag = true;

        plateImage = GetComponent<Image>();

        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 1));
        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 1f));

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

            buildingBoxes[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2((i % 2) * 420 - 210, -(i / 2) * 440 + 466);

            GetChildWithName(buildingBoxes[i], "BuildingIcon").GetComponent<Image>().sprite = resourceModel[boxResourceType].buildingIcon;
            GetChildWithName(buildingBoxes[i], "BuildingResourceIcon").GetComponent<Image>().sprite = resourceModel[boxResourceType].resourceIcon;
            buildingBoxes[i].GetComponent<Image>().color = resourceModel[boxResourceType].resourceColor;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag Begin");
        touchPos = eventData.pointerCurrentRaycast.screenPosition;
        platePrevPos = plateImage.rectTransform.anchoredPosition;
        if (eventData.pointerCurrentRaycast.gameObject.name == "ResourcePlate")
        {
            managerObject.GetComponent<InputManager>().canDrag = false;
            StopCoroutine(movementCoroutine);
            lastSelected = eventData.pointerCurrentRaycast.gameObject;
        }
        if (eventData.pointerCurrentRaycast.gameObject.name.Contains("Box") || eventData.pointerCurrentRaycast.gameObject.name.Contains("Icon"))
        {
            managerObject.GetComponent<InputManager>().canDrag = false;
            StopCoroutine(movementCoroutine);
            StopCoroutine(blackoutCoroutine);
            movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.1f));
            blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));

            constructionHeldScriptableObject = resourceModel[GetResourceIndex(eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite.name.Remove(0, 16))].resourceScriptableObject;

            constructionHeld = Instantiate(constructionPreview, resourcePanelCanvas.transform).GetComponent<Image>();
            constructionHeld.sprite = eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().sprite;
            
            isHoldingBuilding = true;
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
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
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
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 120), 0.6f));
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
                
                if (eventData.pointerCurrentRaycast.gameObject.name.Contains("Preview") || isHoldingBuilding == true)
                {
                    constructionHeld.rectTransform.anchoredPosition = eventData.position;
                    Debug.Log("Position x: " + eventData.position.x + " Position y: " + eventData.position.y);
                    Debug.Log("constructionHeld.x = " + constructionHeld.rectTransform.anchoredPosition.x + ", constructionHeld.y = " + constructionHeld.rectTransform.anchoredPosition.y);
                    Debug.Log("touchPos.x = " + eventData.pointerCurrentRaycast.screenPosition.x + ", touchPos.y = " + eventData.pointerCurrentRaycast.screenPosition.y);
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
            movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 1));
            blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
            lastSelected = null;
            isFocused = false;
        }

        if (isHoldingBuilding == true)
        {
            lastSelected = null;
            Vector3 buildPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.pointerCurrentRaycast.screenPosition.x, eventData.pointerCurrentRaycast.screenPosition.y, Camera.main.nearClipPlane));

            Transform transformCam = Camera.main.transform;

            for (float i = 0; buildPosition.y > 0.5f; i = i + 0.1f)
            {
                buildPosition += transformCam.forward * Time.deltaTime * i;
            }

            Debug.Log(buildPosition);

            GameObject constructionPlaced = Instantiate(constructionPrefab, buildPosition, Quaternion.identity);
            constructionPlaced.SetActive(false);
            constructionPlaced.GetComponent<Construction>().construcion = constructionHeldScriptableObject;
            constructionPlaced.SetActive(true);
            constructionPlaced.GetComponent<Construction>().SetDragging(true);
            Destroy(constructionHeld.gameObject);
            
            isHoldingBuilding = false;

            GameManager.Instance.actionsMade++;
            actionCounter.text = GameManager.Instance.actionsMade.ToString();
        }
    
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        //Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name); 
    }

    public void OnPointerDown(PointerEventData eventData) 
    { 
        //Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    //public void OnPointerEnter(PointerEventData eventData) { Debug.Log("Mouse Enter"); }

    //public void OnPointerExit(PointerEventData eventData) { Debug.Log("Mouse Exit"); }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Mouse Up");
    }

    void Update()
    {
        
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
