using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static LevelSelectDrag Instance;
    public Vector3 dragOffset;
    public bool isDragging = false;
    private Vector3 prevPos;
    private RawImage selfImage;
    private Vector3 touchPos;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        selfImage = GetComponent<RawImage>();
        prevPos = selfImage.rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        prevPos = selfImage.rectTransform.anchoredPosition + new Vector2(dragOffset.x, dragOffset.y);
        touchPos = eventData.pointerCurrentRaycast.screenPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            dragOffset = new Vector3(0, Mathf.Clamp(prevPos.y + (touchPos.y - eventData.pointerCurrentRaycast.screenPosition.y) * -1, -99999999, 0), 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
