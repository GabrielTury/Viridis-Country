using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new Construction", menuName = "Construction")]
public class ConstructionTemplate : ScriptableObject
{
    [Tooltip("Range from which it can gather resources")]
    public int gatherRadius;

    public Mesh constructionMesh;

    public Material material;

    public GameManager.GameResources resourceToGather;

    public GridCell.TileType tileType;
}
