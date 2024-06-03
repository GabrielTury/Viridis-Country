using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlanetRotation : MonoBehaviour
{
    private Vector3 prevRotation;
    // Start is called before the first frame update
    void Start()
    {
        prevRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        var newRot = prevRotation + LevelSelectDrag.Instance.dragOffset;

        transform.rotation = Quaternion.Euler(newRot.x, newRot.y, newRot.z);
    }
}
