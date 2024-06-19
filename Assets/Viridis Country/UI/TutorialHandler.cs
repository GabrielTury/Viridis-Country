using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    private Image selfImage;
    // Start is called before the first frame update
    void Start()
    {
        selfImage = GetComponent<Image>();
        selfImage.rectTransform.localScale = new Vector3(3, 3, 3);
    }

    // Update is called once per frame
    void Update()
    {
        selfImage.rectTransform.localScale = Vector3.Lerp(selfImage.rectTransform.localScale, new Vector3(1, 1, 1), 0.1f);
    }
}
