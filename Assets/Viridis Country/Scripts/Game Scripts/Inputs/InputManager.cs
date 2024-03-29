using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float dragObjectHeight;

    [SerializeField, Range(1f, 30f)]
    private float cameraMoveSpeed;

    [SerializeField]
    private Vector2 maxCameraPosition;

    private Inputs inputs;

    private bool isDragging;

    private bool isMovingCamera;

    private Camera mainCamera;

    private void Awake()
    {
        inputs = new Inputs();
        mainCamera = Camera.main;
    }

    #region Input Delegates
    private void Touch_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Touch perf");
        
        Ray ray = mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.layer == 6) // 6 é a layer das construções
            {
                Debug.Log(hit.collider.gameObject.name);
                StartCoroutine(Drag(hit.collider.gameObject));
                hit.collider.gameObject.SendMessage("SetDragging", true);
            }
            else if(hit.collider != null)
            {
                StartCoroutine(MoveCamera(Touchscreen.current.primaryTouch.position.ReadValue()));
            }
        }
    }

    private void Touch_canceled(InputAction.CallbackContext obj)
    {
        isDragging = false;
        isMovingCamera = false;
    }
    #endregion
    private IEnumerator MoveCamera(Vector2 startTouchPosition)
    {
        isMovingCamera = true;  
        while(isMovingCamera)
        {
            Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector2 touchDelta = currentTouchPosition - startTouchPosition;

            Vector3 moveDirection = mainCamera.transform.TransformDirection(new Vector3(touchDelta.x, 0, touchDelta.y));

            moveDirection.Normalize();

            Vector3 newPosition = mainCamera.transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.z) * cameraMoveSpeed * Time.deltaTime;

            newPosition.x = Mathf.Clamp(newPosition.x, -maxCameraPosition.x, maxCameraPosition.x);
            newPosition.z = Mathf.Clamp(newPosition.z, -maxCameraPosition.y, maxCameraPosition.y);

            mainCamera.transform.position = newPosition;

            yield return null;
        }
        
    }
    private IEnumerator Drag(GameObject obj)
    {
        float dist = Vector3.Distance(obj.transform.position, mainCamera.transform.position);

        //Debug.Log("Drag");
        isDragging = true;
        //Vector3 offset = transform.position - worldPos;

        while(isDragging)
        {
            Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(currentTouchPosition);

            Vector3 position = new Vector3(currentTouchPosition.x, currentTouchPosition.y, mainCamera.WorldToScreenPoint(obj.transform.position).z); //transforma o z do objeto em um ponto na tela
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(position); //transforma os pontos na tela em coordenadas                                                                        

            worldPosition.y = dragObjectHeight; //sobrescreve altura do objeto que está sen arrastado

            obj.transform.position = worldPosition;

            yield return null;
        }

        obj.SendMessage("SetDragging", false);
    }

    private void OnEnable()
    {
       inputs.Enable();
        inputs.TouchInputs.Touch.performed += Touch_performed; //se inscreve para o evento performed
        inputs.TouchInputs.Touch.canceled += Touch_canceled;  //se inscreve para o evento canceled
    }
    private void OnDisable()
    {
        inputs.Disable();
        inputs.TouchInputs.Touch.performed -= Touch_performed; //se desinscreve para o evento performed
        inputs.TouchInputs.Touch.canceled -= Touch_canceled;  //se desinscreve para o evento canceled
    }
}
