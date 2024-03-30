using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameManager;

public class ResourceTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, /*IPointerExitHandler, IPointerEnterHandler,*/ IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameManager gameManager;

    private Image plateImage;

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

    private void Start()
    {
        gameManager = GameManager.Instance;

        plateImage = GetComponent<Image>();

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
    public void OnBeginDrag(PointerEventData eventData) { Debug.Log("Drag Begin"); }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging: " + eventData.pointerCurrentRaycast.gameObject.name);
        if (eventData.pointerCurrentRaycast.gameObject.name == "ResourcePlate")
        {
            plateImage.rectTransform.anchoredPosition = new Vector2(0, eventData.pointerCurrentRaycast.screenPosition.y);
        }
        
    }

    public void OnEndDrag(PointerEventData eventData) { Debug.Log("Drag Ended"); }

    public void OnPointerClick(PointerEventData eventData) { Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name); }

    public void OnPointerDown(PointerEventData eventData) { Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name); }

    //public void OnPointerEnter(PointerEventData eventData) { Debug.Log("Mouse Enter"); }

    //public void OnPointerExit(PointerEventData eventData) { Debug.Log("Mouse Exit"); }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");
    }

    void Update()
    {
        
    }

    GameObject GetChildWithName(GameObject obj, string name)
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
