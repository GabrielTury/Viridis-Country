using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    public bool isAvailable {  get; private set; }
    public float simplePosX { get; private set; }

    public float simplePosZ { get; private set; }

    [SerializeField]
    private string _resource;
    public string resource
    {
        get { return _resource; }
        private set { resource = value; }
    }
        

    [SerializeField]
    private string _tileType;
    public string tileType 
    {
        get { return _tileType; }
        private set { tileType = value; }
    }

    private void Awake()
    {
        isAvailable = true;

        simplePosX = transform.position.x;
        simplePosZ = transform.position.z;
    }

    private void SetResource()
    {

    }

    public void SetAvailability(bool newValue)
    {
        isAvailable = newValue;
    }
}
