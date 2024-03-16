using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public float simplePosX { get; private set; }

    public float simplePosZ { get; private set; }

    public float resource { get; private set; }

    [SerializeField]
    private string _tileType;
    public string tileType 
    {
        get { return tileType; }
        private set { tileType = _tileType; }
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
