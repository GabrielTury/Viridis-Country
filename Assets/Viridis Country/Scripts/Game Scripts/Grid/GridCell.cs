
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    public bool isAvailable {  get; private set; }
    public float simplePosX { get; private set; }

    public float simplePosZ { get; private set; }

    public GameManager.GameResources resource
    {
        get;
        private set;
    }
        

    [SerializeField]
    private string _tileType;
    public string tileType 
    {
        get { return _tileType; }
        private set { tileType = value; }
    }
    GridCell()
    {
        resource = GameManager.GameResources.None;
    }

    private void Awake()
    {
        isAvailable = true;

        simplePosX = transform.position.x;
        simplePosZ = transform.position.z;
    }

    public void SetResource(GameManager.GameResources newResource)
    {
        resource = newResource;
    }

    public void SetAvailability(bool newValue)
    {
        isAvailable = newValue;
    }


}
