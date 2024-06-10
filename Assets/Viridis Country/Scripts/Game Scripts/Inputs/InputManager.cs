using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float dragObjectHeight;

    #region Camera movement variables
    private Camera mainCamera;

    private bool isMovingCamera;

    //[SerializeField, Range(1f, 10f)]
    private float cameraMoveSpeed = 7;

    [SerializeField]
    public float minXPos, maxXPos, minZPos, maxZPos;

    #endregion

    private Inputs inputs;

    private Coroutine dragCoroutine;

    private float dragCooldown;

    private bool isDragging;

    public bool canDrag = true;

    private RaycastHit hitTracker; 

    private Coroutine zoomCoroutine;
    [SerializeField]
    private float zoomSpeed;

    [SerializeField]
    private Image grabTimerImage;

    private Image cloneTimerImage;

    private GameObject canvas;

    private void Awake()
    {
        inputs = new Inputs();
        mainCamera = Camera.main;      

        canvas = FindFirstObjectByType<Canvas>().gameObject;
    }

    #region Input Delegates
    private void Touch_performed(InputAction.CallbackContext obj)
    {
        Ray ray = mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;
        if (canDrag)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject.layer == 6) // 6 � a layer das constru��es
                {
                    Debug.Log(hit.collider.gameObject.name);
                    dragCoroutine = StartCoroutine(Drag(hit.collider.gameObject));
                }
                else
                {
                    StartCoroutine(MoveCamera(Touchscreen.current.primaryTouch.position.ReadValue()));
                }
            }
            else
            {
                StartCoroutine(MoveCamera(Touchscreen.current.primaryTouch.position.ReadValue()));
            }
        }
    }

    private void Touch_canceled(InputAction.CallbackContext obj)
    {
        isDragging = false;
        isMovingCamera = false;
        if (dragCooldown < 0.5f && dragCoroutine != null)
            StopCoroutine(dragCoroutine);

        if(cloneTimerImage != null)
        {
            Destroy(cloneTimerImage.gameObject);
            cloneTimerImage = null;
        }
    }
    #endregion
    private IEnumerator MoveCamera(Vector2 startTouchPosition)
    {
        isMovingCamera = true;

        int overDirection = 0;
        while(isMovingCamera && canDrag)
        {
            Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.delta.ReadValue();
            
            currentTouchPosition.x /= Screen.width;
            currentTouchPosition.y /= Screen.height;

            //if(CheckCameraBoundaries(out overDirection))
            //{
            //    mainCamera.transform.position -= Quaternion.Euler(0,45,0) * Vector3.right.normalized * currentTouchPosition.x * cameraMoveSpeed;
            //    mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.forward.normalized * currentTouchPosition.y * cameraMoveSpeed * 1.4f;
            //}
            //else
            //{
            //    RepositionCamera(overDirection);
            //}

            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.right.normalized * currentTouchPosition.x * cameraMoveSpeed;
            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.forward.normalized * currentTouchPosition.y * cameraMoveSpeed * 1.4f;

            if(!CheckCameraBoundaries(out overDirection))
            {
                RepositionCamera(overDirection);
            }

            yield return null;
        }
        
    }

    private void RepositionCamera(int dir)
    {
        switch (dir)
        {
            case 0:
                break;
            case 1:
                mainCamera.transform.position = new Vector3(maxXPos, mainCamera.transform.position.y, mainCamera.transform.position.z);
                break;
            case 2:
                mainCamera.transform.position = new Vector3(minXPos, mainCamera.transform.position.y, mainCamera.transform.position.z);
                break;
            case 3:
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, maxZPos);
                break;
            case 4:
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, minZPos);
                break;

        }
    }
    private IEnumerator Drag(GameObject obj)
    {
        float dist = Vector3.Distance(obj.transform.position, mainCamera.transform.position);
        Construction construction = obj.GetComponent<Construction>();
        Coroutine check = null;
        //Debug.Log("Drag");
        isDragging = true;
        //Vector3 offset = transform.position - worldPos;
        dragCooldown = 0;
        
        cloneTimerImage = Instantiate(grabTimerImage, canvas.transform);
        cloneTimerImage.rectTransform.position = Touchscreen.current.primaryTouch.position.ReadValue();
        Color myColor = Color.white;
        myColor.a = 0.5f;
        cloneTimerImage.color = myColor;

        while (dragCooldown < 0.5f)
        {
            dragCooldown += Time.deltaTime;
            cloneTimerImage.fillAmount = dragCooldown * 2;
            yield return new WaitForEndOfFrame();
        }

        Destroy(cloneTimerImage.gameObject);
        cloneTimerImage = null;

        Ray rayB = mainCamera.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(rayB, out hit))
        {
            if (hit.collider == null || hit.collider.gameObject.layer != 6) // 6 eh a layer das construcoes
            {
                StopCoroutine(dragCoroutine);
                //Destroy(clone.gameObject);
                yield break;
            }
        }

        while (isDragging && canDrag && dragCooldown > 0.5f)
        {
            obj.GetComponent<Collider>().gameObject.SendMessage("SetDragging", true); //Avisa o objeto que esta sendo carregado

            Vector2 currentTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            //Debug.Log("Touch Pos:" + currentTouchPosition);
            Ray ray = mainCamera.ScreenPointToRay(currentTouchPosition);

            Vector3 position = new Vector3(currentTouchPosition.x, currentTouchPosition.y, mainCamera.WorldToScreenPoint(obj.transform.position).z); //transforma o z do objeto em um ponto na tela
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(position); //transforma os pontos na tela em coordenadas                                                                        

            worldPosition.y = dragObjectHeight; //sobrescreve altura do objeto que est� sen arrastado

            obj.transform.position = worldPosition;
            //Debug.Log("Camera width" + mainCamera.pixelWidth);
            //Debug.Log("Camera height" + mainCamera.pixelHeight);
            //if(currentTouchPosition.x > mainCamera.Scree)


            MoveCameraOnBorder(currentTouchPosition);
            if(check == null)
                check = StartCoroutine(construction.CheckFuturePosition());

            //Debug.Log("Input GG" + currentTouchPosition);
            yield return new WaitForEndOfFrame();
        }

        if (canDrag && dragCooldown > 0.5f)
        {
            Debug.Log("Cooldown: "+ dragCooldown);
            
            if(check != null)
                StopCoroutine(check);

            obj.SendMessage("SetDragging", false); //avisa o objeto que ele n�o est� sendo mais carregado
        }
    }

    private void MoveCameraOnBorder(Vector2 currentTouchPosition)
    {
        int direction = 0;
        if (currentTouchPosition.x > Screen.width - 200)
        {
            //Move Camera to the rights
            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.right.normalized * -0.01f;
            if (!CheckCameraBoundaries(out direction))
            {
                RepositionCamera(direction);
            }

        }
        else if (currentTouchPosition.x < 200)
        {
            //Move Camera to the left
            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.right.normalized * 0.01f;
            if (!CheckCameraBoundaries(out direction))
            {
                RepositionCamera(direction);
            }
        }
        else if (currentTouchPosition.y > Screen.height - 200)
        {
            //Move Camera up
            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.forward.normalized * -0.01f;
            if (!CheckCameraBoundaries(out direction))
            {
                RepositionCamera(direction);
            }
        }
        else if (currentTouchPosition.y < 200)
        {
            //Move Camera Down
            mainCamera.transform.position -= Quaternion.Euler(0, 45, 0) * Vector3.forward.normalized * 0.01f;
            if (!CheckCameraBoundaries(out direction))
            {
                RepositionCamera(direction);
            }
        }
    }

    private void ZoomStart()
    {
        if(zoomCoroutine == null)
            zoomCoroutine = StartCoroutine(ZoomLogic());
    }
    private void ZoomEnd()
    {
        StopCoroutine(zoomCoroutine);

        zoomCoroutine = null;
    }

    private IEnumerator ZoomLogic()
    {
        float previousDistance = 0f;
        float distance = 0f;
        while(true)
        {
           

            distance = Vector2.Distance(inputs.TouchInputs.PrimaryTouchPosition.ReadValue<Vector2>(),
                                        inputs.TouchInputs.SecondaryTouchPosition.ReadValue<Vector2>());
            //ZoomOut
            if(distance < previousDistance)
            {
                if(mainCamera.orthographicSize < 8)
                {
                    float newSize = mainCamera.orthographicSize;
                    newSize += 0.5f;
                    mainCamera.orthographicSize = Mathf.Lerp( mainCamera.orthographicSize, newSize, zoomSpeed * Time.deltaTime);
                }
                else
                {
                    mainCamera.orthographicSize = 8;
                }
            }
            //Zoom In
            else if(distance > previousDistance)
            {
                if(mainCamera.orthographicSize > 2)
                {
                    float newSize = mainCamera.orthographicSize;
                    newSize -= 0.5f;
                    mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newSize, zoomSpeed * Time.deltaTime);
                }
                else
                {
                    mainCamera.orthographicSize = 2;
                }
            }


            previousDistance = distance;
            yield return null;
        }
    }
    /// <summary>
    /// Check if the camera is inside preset boundaries
    /// </summary>
    /// <returns>True is inside boundaries False if outside boudaries</returns>
    private bool CheckCameraBoundaries(out int direction)
    {
        direction = 0;
        if(mainCamera.transform.position.x > maxXPos)//Check for X pos
        {
            direction = 1;
            return false;
        }
        else if(mainCamera.transform.position.x < minXPos)
        {
            direction = 2;
            return false;
        }
        else if(mainCamera.transform.position.z > maxZPos)//Check for Z pos
        {
            direction = 3;
            return false;
        }
        else if(mainCamera.transform.position.z < minZPos)
        {
            direction = 4;
            return false;
        }
        else//Return 
        {
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 lineVect;

        Gizmos.DrawLine(lineVect = new Vector3(minXPos, 10, 0), (Vector3.down * 10) + lineVect);
        Gizmos.DrawLine(lineVect = new Vector3(maxXPos, 10, 0), (Vector3.down * 10) + lineVect);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(lineVect = new Vector3(0, 10, minZPos), (Vector3.down * 10) + lineVect);
        Gizmos.DrawLine(lineVect = new Vector3(0, 10, maxZPos), (Vector3.down * 10) + lineVect);
    }
    private void OnEnable()
    {
       inputs.Enable();
        inputs.TouchInputs.Touch.performed += Touch_performed; //se inscreve para o evento performed
        inputs.TouchInputs.Touch.canceled += Touch_canceled;  //se inscreve para o evento canceled
        inputs.TouchInputs.SecondaryTouchContact.started += _ => ZoomStart(); //o _ indica que n�o precisa de receber o argumento que o evento passa (o contexto no caso)
        inputs.TouchInputs.SecondaryTouchContact.canceled += _ => ZoomEnd();
    }
    private void OnDisable()
    {
        inputs.Disable();
        inputs.TouchInputs.Touch.performed -= Touch_performed; //se desinscreve para o evento performed
        inputs.TouchInputs.Touch.canceled -= Touch_canceled;  //se desinscreve para o evento canceled
        inputs.TouchInputs.SecondaryTouchContact.started -= _ => ZoomStart();
        inputs.TouchInputs.SecondaryTouchContact.canceled -= _ => ZoomEnd();
    }
}
