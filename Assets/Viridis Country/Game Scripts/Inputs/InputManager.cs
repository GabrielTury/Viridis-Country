using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    private Inputs inputs;

    private Camera mainCamera;

    private void Awake()
    {
        inputs = new Inputs();
    }

    private void Start()
    {
        inputs.TouchInputs.Touch.started += Touch_started;
        inputs.TouchInputs.Touch.performed += Touch_performed;
        inputs.TouchInputs.Touch.canceled += Touch_canceled;



        mainCamera = FindObjectOfType<Camera>();
    }
    private void Touch_started(InputAction.CallbackContext obj)
    {
        Vector2 screenPos = inputs.TouchInputs.ScreenPosition.ReadValue<Vector2>();
        //Vector2 screenPos = obj.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Debug.Log(worldPos);
    }

    private void Touch_performed(InputAction.CallbackContext obj)
    {
        Vector2 screenPos = inputs.TouchInputs.ScreenPosition.ReadValue<Vector2>();
        //Vector2 screenPos = obj.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Debug.Log(worldPos);
    }

    private void Touch_canceled(InputAction.CallbackContext obj)
    {
        Vector2 screenPos = inputs.TouchInputs.ScreenPosition.ReadValue<Vector2>();
        //Vector2 screenPos = obj.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Debug.Log(worldPos);
    }



    private void OnEnable()
    {
       inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }
}
