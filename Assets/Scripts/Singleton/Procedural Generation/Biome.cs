using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Biome
{

    [Space, Header("Biome")]
    #region
    public string name;
    #endregion


    [Space, Header("Surface Layer Settings")]
    #region
    public TileBase surfaceTile;

    public TileBase treeTile;

    public TileBase leafTile;

    [Range(0, 10)]
    public int octaveCount;

    [Range(0.000001f, 0.05f)]
    public float frequency;

    [Range(0.000001f, 0.5f)]
    public float roughness;

    [Range(1, 500)]
    public float amplitude;

    [Range(0f, 1f)]
    public float persistence;

    [Range(1, 10)]
    public int lacunarity;

    [HideInInspector] public int[] surfaceHeightMap;

    [HideInInspector] public int biomeStartX;
    [HideInInspector] public int biomeEndX;
    #endregion


    [Space, Header("Cave Generation Settings")]
    #region
    public TileBase caveTile;

    public TileBase deepCaveTile;

    public TileBase darkCaveTile;

    [Range(0, 5)]
    public int maxCaveCount;

    [Range(0, 100)]
    public int cavePadding = 20;

    [Range(1, 20)]
    public int caveWidth = 5;

    [Range(0.000001f, 0.05f)]
    public float caveRoughness;

    [Range(0.000001f, 0.05f)]
    public float caveFrequency;

    [Range(1, 500)]
    public float caveAmplitude;

    [Range(0f, 1f)]
    public float cavePersistence;

    [Range(1, 10)]
    public int caveLacunarity;
    #endregion

}


