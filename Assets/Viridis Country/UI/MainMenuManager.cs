using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject planet;
    private Transform planetTransform;

    [SerializeField]
    private float rotationSpeed;
    // yea
    // Start is called before the first frame update
    void Start()
    {
        planetTransform = planet.transform;
    }

    void Update()
    {
        planetTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
