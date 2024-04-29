
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    public bool isAvailable {  get; private set; }
    public bool isColectible {  get; private set; }
    public float simplePosX { get; private set; }

    public float simplePosZ { get; private set; }

    public GameManager.GameResources[] resource
    {
        get;
        private set;
    }

    public enum TileType
    {
        Normal,
        Water
    }
        

    [SerializeField]
    private TileType _tileType;
    public TileType tileType 
    {
        get { return _tileType; }
        private set { tileType = value; }
    }
    GridCell()
    {
        resource = new GameManager.GameResources[2];
        for(int i = 0; i < resource.Length; i++)
            resource[i] = GameManager.GameResources.None;
    }

    private void Awake()
    {
        isAvailable = true;

        simplePosX = transform.position.x;
        simplePosZ = transform.position.z;
    }
    private void Start()
    {
        if(tileType == TileType.Water)
        {
            resource[0] = GameManager.GameResources.Water;
        }
    }
    public void SetResource(GameManager.GameResources newResource, int index)
    {
        resource[index] = newResource;
    }

    public void SetAvailability(bool newValue)
    {
        isAvailable = newValue;
    }

    public void SetColectability(bool newValue)
    {
        isColectible = newValue;
    }


}
