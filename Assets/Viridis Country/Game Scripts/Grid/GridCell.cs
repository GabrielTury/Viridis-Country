using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
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
        simplePosX = transform.position.x;
        simplePosZ = transform.position.z;
    }

    private void SetResource()
    {

    }
}
