using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    private Inputs inputs;

    private bool isDragging;

    private Camera mainCamera;

    private void Awake()
    {
        inputs = new Inputs();
        mainCamera = Camera.main;
    }

    #region Input Delegates
    private void Touch_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Touch perf");
        
        Ray ray = mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.layer == 6) // 6 é a layer das construções
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
    #endregion

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

            Vector3 position = new Vector3(currentTouchPosition.x, currentTouchPosition.y, mainCamera.WorldToScreenPoint(obj.transform.position).z); //transforma o z do objeto em um ponto na tela
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(position); //transforma os pontos na tela em coordenadas                                                                        

            worldPosition.y = 1f; //sobrescreve altura do objeto que está sen arrastado

            obj.transform.position = worldPosition;

            yield return null;
        }

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
