using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new Construction", menuName = "Construction")]
public class ConstructionTemplate : ScriptableObject
{
    [Tooltip("Range from which it can gather resources")]
    public int gatherRadius;

    public Mesh constructionMesh;

    public Material material;

    public AnimationClip animClip;
    
    public GameManager.GameResources[] resourceToGather;
    
    [Tooltip("Atualmente tem o maximo de 2 recursos, tem que trocar o tamanho do array no gridcell para alterar")]
    public GameManager.GameResources[] secondaryResource;

    public GridCell.TileType tileType;

    public AudioManager.ConstructionAudioTypes constructionType;

    public Sprite renderSprite;

    public GameObject boneTransform;
}
