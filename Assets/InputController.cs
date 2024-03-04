using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        while(i < Input.touchCount)
        {
            Touch touch = Input.GetTouch(i);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider != null)
                {
                    Debug.Log("Finger"+ i + " "+hit.collider.name);
                    hit.collider.SendMessage("Touched", SendMessageOptions.DontRequireReceiver); //Executa a fun��o que ta em string
                }
            }
        }
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
}
