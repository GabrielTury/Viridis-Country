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

    private Image plateImage;

    [SerializeField]
    private Image bgFader;

    private bool[] GameResourcesToShow = { false, false, false, false }; // 0 = None, 1 = Water, 2 = Wood, 3 = Stone

    [System.Serializable]
    public class ResourceModel
    {
        public Sprite buildingIcon;
        public Sprite resourceIcon;
        public Color resourceColor;
        public Mesh resourceMesh; // ?

        public ResourceModel(Sprite buildingIcon, Sprite resourceIcon, Color resourceColor, Mesh resourceMesh)
        {
            this.buildingIcon = buildingIcon;
            this.resourceIcon = resourceIcon;
            this.resourceColor = resourceColor;
            this.resourceMesh = resourceMesh;
        }
    } // nao queria deixar tudo public mas nao consegui fazer funcionar no inspetor como private

    public ResourceModel[] resourceModel;

    [SerializeField]
    private GameObject buildingBoxTemplate;

    private List<GameObject> buildingBoxes = new List<GameObject>();

    private int buildingBoxCount = 0;

    Vector2 touchPos;
    Vector2 platePrevPos;
    Coroutine movementCoroutine;
    Coroutine blackoutCoroutine;
    bool smoothReturnEnded = true;
    bool isFocused = false;

    private Inputs input;

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
        GameResourcesToShow[1] = gameManager.objectiveWater > 0 ? true : false;
        GameResourcesToShow[2] = gameManager.objectiveWood > 0 ? true : false;
        GameResourcesToShow[3] = gameManager.objectiveStone > 0 ? true : false;

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
        Debug.Log("Drag Begin");
        touchPos = eventData.pointerCurrentRaycast.screenPosition;
        platePrevPos = plateImage.rectTransform.anchoredPosition;
        if (eventData.pointerCurrentRaycast.gameObject.name == "ResourcePlate")
        {
            managerObject.GetComponent<InputManager>().canDrag = false;
            StopCoroutine(movementCoroutine);
        }    
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging: " + eventData.pointerCurrentRaycast.gameObject.name);
        if(eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "ResourcePlate")
            {
                if (isFocused)
                {
                    if (plateImage.rectTransform.anchoredPosition.y <= 200)
                    {
                        Debug.Log("ABC");
                        StopCoroutine(movementCoroutine);
                        StopCoroutine(blackoutCoroutine);
                        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 0.3f));
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
                        isFocused = false;
                        managerObject.GetComponent<InputManager>().canDrag = true;
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
                        Debug.Log("CBA");
                        StopCoroutine(movementCoroutine);
                        StopCoroutine(blackoutCoroutine);
                        movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, 800), 0.3f));
                        blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 120), 0.6f));
                        isFocused = true;
                        eventData.pointerDrag = null;
                    }
                    else
                    {
                        plateImage.rectTransform.anchoredPosition = new Vector2(0, platePrevPos.y + (touchPos.y - eventData.pointerCurrentRaycast.screenPosition.y) * -1);
                    }
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
        Debug.Log("Drag Ended");
        managerObject.GetComponent<InputManager>().canDrag = true;
        if (smoothReturnEnded == true)
        {
            Debug.Log("DEF");
            StopCoroutine(movementCoroutine);
            StopCoroutine(blackoutCoroutine);
            movementCoroutine = StartCoroutine(SmoothReturn(plateImage, new Vector2(0, -610), 1));
            blackoutCoroutine = StartCoroutine(FadeColor(bgFader, new Color32(0, 0, 0, 0), 0.6f));
            isFocused = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name); 
    }

    public void OnPointerDown(PointerEventData eventData) 
    { 
        Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    //public void OnPointerEnter(PointerEventData eventData) { Debug.Log("Mouse Enter"); }

    //public void OnPointerExit(PointerEventData eventData) { Debug.Log("Mouse Exit"); }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");
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
