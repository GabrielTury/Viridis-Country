using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.acceleration.x, Input.acceleration.y, -Input.acceleration.z);
        cube.transform.position += direction * Time.deltaTime * 3;
    }

    /* public void OnGUI()
     {
         int i = 0;

         while(i < Input.touchCount)
         {
             GUI.Label(new Rect(0,i *40,200,40), "Touch index" + i + ":" + Input.touchCount);
             i++;
         }
     }*/

    /*int i = 0;
        while(i<Input.touchCount)
        {
            Touch touch = Input.GetTouch(i);
    Ray ray = Camera.main.ScreenPointToRay(touch.position);
    RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider != null)
                {
                    Debug.Log("Finger"+ i + " "+hit.collider.name);
                    hit.collider.SendMessage("Touched", SendMessageOptions.DontRequireReceiver); //Executa a função que ta em string
                }
            }
        }*/
}
