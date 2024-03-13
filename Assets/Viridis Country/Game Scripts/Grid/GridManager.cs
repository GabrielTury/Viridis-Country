using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField, Tooltip("X is Width, Z is height")]
    private int height, width;

    [SerializeField]
    private int totalGridHeight;

    private float gridGapSize = 1f;

    private GameObject[,] grid;

    [SerializeField]
    private GameObject gridCellPrefab;

    private void Awake()
    {
        grid = new GameObject[width, height];
    }
    private void Start()
    {
        CreateGrid();
    }

    //Cria a Grid
    private void CreateGrid()
    {
        if(gridCellPrefab == null)
        {
            Debug.LogError("No Grid Cell Prefab");
            return;
        }

        for(int x = 0;x < width;x++)
        {
            for(int z = 0;z < height;z++)
            {
                grid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridGapSize, totalGridHeight, z * gridGapSize), Quaternion.identity,transform);
#if UNITY_EDITOR
                grid[x,z].name = "Grid Space( X: " + x.ToString() + ", Z: " + z.ToString() + ")";
#endif
            }
        }
    }

}
