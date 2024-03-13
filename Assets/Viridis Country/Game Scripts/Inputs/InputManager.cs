using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    private Inputs inputs;

    [SerializeField]
    private float speed;

    private Vector3 screenWorldPos;


    private bool isDragging;

    private Camera mainCamera;

    private void Awake()
    {
        inputs = new Inputs();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        inputs.TouchInputs.Touch.performed += Touch_performed;
        inputs.TouchInputs.Touch.canceled += Touch_canceled;



        
    }

    private void Touch_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Touch perf");
        
        Ray ray = mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                StartCoroutine(Drag(hit.collider.gameObject));

            }

        }
    }

    private void Touch_canceled(InputAction.CallbackContext obj)
    {
        isDragging = false;
    }

    private IEnumerator Drag(GameObject obj)
    {
        float dist = Vector3.Distance(obj.transform.position, mainCamera.transform.position);

        Debug.Log("Drag");
        isDragging = true;
        //Vector3 offset = transform.position - worldPos;

        while(isDragging)
        {
            Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(currentTouchPosition);
            screenWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(currentTouchPosition.x, currentTouchPosition.y, mainCamera.transform.position.y));
            screenWorldPos.y = obj.transform.position.y;

            obj.transform.position = Vector3.Lerp(obj.transform.position, screenWorldPos, speed * Time.deltaTime);
            yield return null;
        }

    }

    private void OnEnable()
    {
       inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
        inputs.TouchInputs.Touch.performed -= Touch_performed;
        inputs.TouchInputs.Touch.canceled -= Touch_canceled;
    }
}
