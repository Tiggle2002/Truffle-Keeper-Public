using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using System.Collections;
using Sirenix.OdinInspector;

public class WorldGenerationManager : SingletonMonoBehaviour<WorldGenerationManager>
{
    #region World Properties
    [Header("World Settings")]
    public bool randomSeed = false;
    [HideIf("randomSeed", true)]
    public int seed;
    [Range(0, 10)]
    public int octaveCount = 5;
    [Range(0, 10)]
    public int worldSmoothCount = 5;
    [Range(0, 1f)]
    public float worldFillAmount;
    #endregion

    #region Tilemap Properties
    [Header("Tilemap References")]
    public Tilemap worldTilemap;
    public Tilemap objectTilemap;
    public Tilemap wallTilemap;

    public TileBase surfaceLayerTile;
    public TileBase rockLayerTile;
    public TileBase deepRockLayerTile;
    public TileBase darkLayerTile;
    #endregion

    #region Biome Properties 
    [Header("Biome Settings")]
    public int biomeSize;
    public int biomeTransitionLength;
    public List<Biome> biomes;
    public Biome currentBiome { get; private set; }
    #endregion

    #region World Arrays
    private int[,] worldArray;
    private float[,] noiseMap;

    private int[] surfaceLayerHeightMap;
    private int[] stoneLayerHeightMap;
    private int[] deepStoneLayerHeightMap;
    private int[] darkLayerHeightMap;
    #endregion

    #region Variables
    public int worldWidth { get; private set; }

    #endregion

    #region Layer Values
    public List<WorldLayer> worldLayers;
    #endregion

    #region References
    private ObjectGenerator objectGenerator;
    #endregion

    public override void Awake()
    {
        base.Awake();
    }

    public void Generate()
    {
        if (randomSeed)
        {
            seed = (int)DateTime.Now.Ticks;
        }
        StopAllCoroutines();
        ResetTilemap();
        InitialiseVariables();

        GenerateWorldBase();
        GenerateWorldCaves();
        worldArray.MooreCellularAutomaton(worldSmoothCount);

        //EditorCoroutineUtility.StartCoroutineOwnerless(FillSurfaceTiles());

    }

    private void ResetTilemap()
    {
        worldTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    private void InitialiseVariables()
    {
        worldWidth = biomes.Count * biomeSize;
        worldArray = new int[worldWidth, worldWidth];

        Random.InitState(seed);
        objectGenerator = FindObjectOfType<ObjectGenerator>();
    }

    #region Base World Generation
    private void GenerateWorldBase()
    {
        GenerateSurface();
        GenerateUnderground();
        CombineSurfaceAndUnderground();


        FillWorldMap();
    }

    #region Surface Generation
    private void GenerateSurface()
    {
        GenerateBiomeHeightMaps();
        GenerateSurfaceHeightMaps();
        BlendSurfaceHeights();
    }

    private void GenerateBiomeHeightMaps()
    {
        int biomeStartX = 0;
        int biomeEndX = biomeSize;
        for (int b = 0; b < biomes.Count; b++)
        {
            biomes[b].surfaceHeightMap = NoiseGenerator.GenerateHeightMap(biomeSize, biomes[b].frequency, biomes[b].amplitude, biomes[b].lacunarity, biomes[b].persistence, biomes[b].roughness);
            biomes[b].surfaceHeightMap.AddMinimumHeightToHeightMap(worldLayers[3].layerHeight);

            biomes[b].biomeStartX = biomeStartX;
            biomes[b].biomeEndX = biomeEndX;

            biomeStartX += biomeSize;
            biomeEndX += biomeSize;
        }
    }

    private void GenerateSurfaceHeightMaps()
    {
        worldLayers[3].upperHeightMap = new int[worldWidth];

        int startX = 0;
        int endX = biomeSize;
        for (int b = 0; b < biomes.Count; b++)
        {
            for (int x = 0; x < biomeSize; x++)
            {
                for (int y = worldLayers[3].layerHeight; y <= biomes[b].surfaceHeightMap[x]; y++)
                {
                    worldLayers[3].upperHeightMap[startX + x] = y;
                }
            }
            startX += biomeSize;
            endX += biomeSize;
        }

        worldLayers[3].upperHeightMap.AddMinimumHeightToHeightMap(worldLayers[3].layerHeight);
    }

    private void BlendSurfaceHeights()
    {
        int transitionStartX = biomeSize - biomeTransitionLength;
        int transitionEndX = biomeSize + biomeTransitionLength;

        for (int b = 0; b < biomes.Count - 1; b++)
        {
            Vector2 startPos = new Vector2(transitionStartX, worldLayers[3].upperHeightMap[transitionStartX]);

            Vector2 endPos = new Vector2(transitionEndX, worldLayers[3].upperHeightMap[transitionEndX]);

            int[] transitionHeightMap = NoiseGenerator.GenerateHeightMap(worldWidth, biomes[b].frequency * 5, biomes[b].amplitude / 20, biomes[b].lacunarity * 2, biomes[b].persistence / 10, biomes[b].roughness * 2);

            for (int x = transitionStartX; x <= transitionEndX; x++)
            {
                float percentComplete = (float)(x - transitionStartX) / (float)(50);

                int height = Mathf.RoundToInt(Vector2.Lerp(startPos, endPos, percentComplete).y + transitionHeightMap[x]);

                worldLayers[3].upperHeightMap[x] = height;
            }
            transitionStartX += biomeSize;
            transitionEndX += biomeSize;
        }
    }
    #endregion

    #region Underground Generation
    private void GenerateUnderground()
    {
        GenerateUndergroundLayersHeightMaps();
    }

    private void GenerateUndergroundLayersHeightMaps()
    {
        for (int i = 2; i >= 0; i--)
        {
            worldLayers[i].upperHeightMap = NoiseGenerator.GenerateHeightMap(worldWidth, worldLayers[i].frequency, worldLayers[i].amplitude, worldLayers[i].lacunarity, worldLayers[i].persistence, worldLayers[i].roughness);

            worldLayers[i].upperHeightMap.AddMinimumHeightToHeightMap(worldLayers[i].layerHeight);
        }
    }

    #endregion

    #region Surface & Underground Generation
    private void CombineSurfaceAndUnderground()
    {
        GenerateLowerHeightMapForLayers();
        CheckForLayerGaps();
    }

    private void GenerateLowerHeightMapForLayers()
    {
        for (int i = 0; i < worldLayers.Count; i++)
        {
            worldLayers[i].lowerHeightMap = new int[worldWidth];
            worldLayers[i].lowerHeightMap.AddMinimumHeightToHeightMap(worldLayers[i].layerHeight);
        }
    }

    private void CheckForLayerGaps()
    {
        for (int i = 0; i <= 2; i++)
        {
            WorldLayer upperLayer = worldLayers[i + 1];
            WorldLayer lowerLayer = worldLayers[i];

            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                if (upperLayer.lowerHeightMap[x] - lowerLayer.upperHeightMap[x] > 0)
                {
                    FillGapsInLayers(lowerLayer, upperLayer, x);
                }
            }
        }
    }

    private void FillGapsInLayers(WorldLayer lowerLayer, WorldLayer upperLayer, int coordX)
    {
        upperLayer.lowerHeightMap[coordX] = lowerLayer.upperHeightMap[coordX];
    }
    #endregion
    #endregion

    #region Cave Generation
    private void GenerateWorldCaves()
    {
        GenerateCaveNoise();
        GenerateSurfaceCave();
    }

    private void GenerateCaveNoise()
    {
        for (int l = 0; l < worldLayers.Count - 1; l++)
        {
            float[,] noiseMap = NoiseGenerator.Generate2DPerlinMap(worldWidth, worldWidth, worldLayers[l].caveFrequency, worldLayers[l].caveAmplitude, worldLayers[l].lacunarity, worldLayers[l].persistence);
            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                for (int y = worldLayers[l].lowerHeightMap[x]; y < worldLayers[l].upperHeightMap[x]; y++)
                {
                    worldArray[x, y] = Mathf.RoundToInt(noiseMap[x, y]);
                }
            }
        }
    }

    private void GenerateSurfaceCave()
    {
        for (int b = 0; b < biomes.Count; b++)
        {
            int cavePointX = Random.Range(biomes[b].biomeStartX + biomes[b].cavePadding, biomes[b].biomeEndX - biomes[b].cavePadding);
            int caveEndY = Random.Range(50, 75);
            
            Vector3Int caveStart = new(cavePointX, worldLayers[3].upperHeightMap[cavePointX]);

            GenerateWormCave(biomes[b], caveStart, caveEndY);
        }
    }

    private void GenerateWormCave(Biome biome, Vector3Int caveStart, int caveEnd)
    {
        int[] perlinWormMap = NoiseGenerator.GenerateHeightMap(1000, biome.caveFrequency, biome.caveAmplitude, biome.caveLacunarity, biome.cavePersistence, biome.caveRoughness);

        for (int y = caveStart.y; y >= caveEnd; y--)
        {
            int caveMinX = perlinWormMap[y] - biome.caveWidth;
            int caveMaxX = perlinWormMap[y] + biome.caveWidth;

            for (int x = caveMinX; x < caveMaxX; x++)
            {
                if (caveStart.x + x < worldArray.GetUpperBound(0) && caveStart.x + x > worldArray.GetLowerBound(0))
                    worldArray[caveStart.x + x, y] = 0;

                if (Random.value < 0.5 && caveStart.x + x - 1 > worldArray.GetLowerBound(0) && caveStart.x + x - 1 < worldArray.GetLowerBound(0))
                {
                    worldArray[caveStart.x + x - 1, y] = Random.value < 0.5 ? 1 : 0;
                    worldArray[caveStart.x + x + 1, y] = Random.value < 0.5 ? 1 : 0;
                }
            }
        }
    }
    #endregion

    #region Tilemap Methods
    private IEnumerator FillSurfaceTiles()
    {
        int biomeStartX = 0;
        int biomeEndX = biomeSize;

        for (int b = 0; b < biomes.Count; b++)
        {
            for (int x = biomeStartX; x < biomeEndX; x++)
            {
                for (int y = worldLayers[3].lowerHeightMap[x]; y <= worldLayers[3].upperHeightMap[x]; y++)
                {
                    if (worldArray[x, y] == 1)
                    {
                        Vector3Int tilePos = new(x, y);

                        worldTilemap.SetTile(tilePos, biomes[b].surfaceTile);
                    }
                }
                yield return null;
            }
            biomeStartX += biomeSize;
            biomeEndX += biomeSize;
        }
        //EditorCoroutineUtility.StartCoroutineOwnerless(FillUndergroundTiles());
    }

    private IEnumerator FillUndergroundTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                for (int y = worldLayers[i].lowerHeightMap[x]; y <= worldLayers[i].upperHeightMap[x]; y++)
                {
                    Vector3Int tilePos = new(x, y);

                    if (worldArray[x, y] == 1)
                    {
                        worldTilemap.SetTile(tilePos, worldLayers[i].layerTile);
                    }
                    else
                    {
                        worldTilemap.SetTile(tilePos, null);
                    }
                }
                yield return null;
            }
        }
        GenerateObjects();
    }
    #endregion

    #region Object Placement
    private void GenerateObjects()
    {
        StartCoroutine(PlaceTree());
    }

    private IEnumerator PlaceTree()
    {
        List<int> availableSpots = FindAvailableSpotsOnSurface(5);

        for (int i = 0; i < availableSpots.Count; i++)
        {
            if (Random.value < 0.5)
            {
                int positionX = availableSpots[i];

                Vector2Int position = new(positionX, worldLayers[3].upperHeightMap[availableSpots[i]] + 1);
                (TileBase[], int[,]) tree = objectGenerator.GenerateTree();
                PlaceWallObject(position, tree);
                yield return null;
            }
        }
    }

    private List<int> FindAvailableSpotsOnSurface(int spaceSize)
    {
        {
            Random.InitState(seed);

            int currentHeight = 0;
            int counter = 0;

            List<int> availableXCoordinates = new List<int>();
            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                if (worldArray[x, worldLayers[3].upperHeightMap[x] + 1] == 1)
                {
                    counter = 1;
                    continue;
                }

                if (spaceSize == 1)
                {
                    availableXCoordinates.Add(x);
                    continue;
                }

                if (currentHeight == worldLayers[3].upperHeightMap[x])
                {
                    ++counter;

                    if (counter == spaceSize)
                    {
                        int returnToStart = counter - 1;

                        int startXPoint = x - returnToStart;

                        availableXCoordinates.Add(startXPoint);

                        counter = 0;
                    }
                }
                else
                {
                    currentHeight = worldLayers[3].upperHeightMap[x];
                    counter = 1;
                }
            }
            return availableXCoordinates;
        }
    }
    private void PlaceWallObject(Vector2Int lowerLeftCornerPosition, (TileBase[], int[,]) objectMap)
    {
        for (int w = 0; w < objectMap.Item2.GetUpperBound(0); w++)
        {
            for (int h = 0; h < objectMap.Item2.GetUpperBound(1); h++)
            {
                if (objectMap.Item2[w, h] >= 1)
                {
                    worldArray[lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h] = 1;
                }
                else
                {
                    worldArray[lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h] = 0;
                }

                Vector3Int tilePos = new(lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h);

                if (objectMap.Item2[w, h] == 1)
                {
                    wallTilemap.SetTile(tilePos, objectMap.Item1[1]);
                }
                else if (objectMap.Item2[w, h] == 2)
                {
                    wallTilemap.SetTile(tilePos, objectMap.Item1[2]);
                }
                else
                {
                    wallTilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    private void PlaceObject(Vector2Int lowerLeftCornerPosition, (TileBase[], int[,]) objectMap)
    {
        for (int w = 0; w < objectMap.Item2.GetUpperBound(0); w++)
        {
            for (int h = 0; h < objectMap.Item2.GetUpperBound(1); h++)
            {
                if (objectMap.Item2[w, h] >= 1)
                {
                    worldArray[lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h] = 1;
                }
                else
                {
                    worldArray[lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h] = 0;
                }

                Vector3Int tilePos = new(lowerLeftCornerPosition.x + w, lowerLeftCornerPosition.y + h);

                if (objectMap.Item2[w, h] == 1)
                {
                    worldTilemap.SetTile(tilePos, objectMap.Item1[1]);
                }
                else if (objectMap.Item2[w, h] == 2)
                {
                    worldTilemap.SetTile(tilePos, objectMap.Item1[2]);
                }
                else if (objectMap.Item2[w, h] == 0)
                {
                    worldTilemap.SetTile(tilePos, null);
                }
            }
        }
    }
    #endregion

    private void FillWorldMap()
        {
            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= worldLayers[3].upperHeightMap[x]; y++)
                {
                    worldArray[x, y] = 1;
                }
            }
        }

    private void UpdateWorldHeight()
        {
            for (int x = 0; x < worldArray.GetUpperBound(0); x++)
            {
                for (int y = worldArray.GetUpperBound(1) - 1; y >= 0; y--)
                {
                    if (worldArray[x, y] == 1)
                    {
                        worldLayers[3].upperHeightMap[x] = y;
                        break;
                    }
                }
            }
        }
}


public static class NoiseGenerator
{
    public static int[] GenerateHeightMap(int width, float frequency, float amplitude, int lacunarity, float persistence, float roughness)
    {
        #region Generation Values
        int[] heightMap = new int[width];

        Vector2[] offsets = NoiseGenerator.GenerateOctaveOffsets(WorldGenerationManager.Instance.octaveCount, WorldGenerationManager.Instance.seed);

        float perlinAddition = 0;
        #endregion

        for (int i = 0; i < offsets.Length; i++)
        {
            for (int x = 0; x < width; x++)
            {
                float perlinHeight = NoiseGenerator.GetPerlinValue(offsets[i], frequency, perlinAddition);

                int heightToAdd = Mathf.RoundToInt(perlinHeight * amplitude);

                heightMap[x] += heightToAdd;

                perlinAddition += roughness;
            }

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return heightMap;
    }

    public static float[,] Generate2DPerlinMap(int width, int height, float frequency = 0.1f, float amplitude = 1f, int lacunarity = 2, float persistence = 0.05f)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < noiseMap.GetUpperBound(0); x++)
        {
            for (int y = 0; y < noiseMap.GetUpperBound(1); y++)
            {
                float noiseHeight = 0;
                float freq = frequency;
                float amp = amplitude;
                for (int i = 0; i < WorldGenerationManager.Instance.octaveCount; i++)
                {
                    float noiseX = x * freq;
                    float noiseY = y * freq;

                    float perlinValue = Mathf.PerlinNoise(noiseX, noiseY);

                    noiseHeight += perlinValue * amp;

                    freq *= lacunarity;
                    amp *= persistence;
                }
                noiseMap[x, y] = noiseHeight;

            }
        }

        return noiseMap;
    }

    public static float GetPerlinValue(Vector2 offset, float frequency, float perlinAddition = 0)
    {
        float noiseX = ((offset.x) * frequency) + perlinAddition;

        float noiseY = ((offset.y) * frequency) + perlinAddition;

        return Mathf.PerlinNoise(noiseX, noiseY);
    }

    public static Vector2[] GenerateOctaveOffsets(int octaveCount, int seed)
    {
        Vector2[] offsets = new Vector2[octaveCount];

        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = new Vector2(Random.Range(-100000, 100000f), Random.Range(-100000, 100000f));
        }

        return offsets;
    }

    public static void AddMinimumHeightToHeightMap(this int[] heightMap, int minHeight)
    {
        for (int x = 0; x < heightMap.Length; x++)
        {
            heightMap[x] += minHeight;
        }
    }

    public static int[,] GenerateSemiCircle(int radius)
    {
        int[,] craterMap = new int[2*radius + 1, radius];

        for (int x = 0; x < craterMap.GetUpperBound(0); x++)
        {
            for (int y = 0; y < radius; y++)
            {
                if (Mathf.Abs(Mathf.Pow(x - radius, 2)) + Mathf.Abs(Mathf.Pow(y - radius, 2)) - Mathf.Pow(radius, 2 ) < 1)
                {
                    craterMap[x, y] = 1;
                }
                else
                {
                    craterMap[x, y] = -1;
                }
            }
        }

        return craterMap;
    }

}

public static class CellularAutomaton
{
    public static void CircularMooreCellularAutomaton(this int[,] array, int radius, int smoothCount, int deadValue, int aliveValue)
    {
        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x < array.GetUpperBound(0); x++)
            {
                for (int y = 0; y < array.GetUpperBound(1); y++)
                {
                    if (Mathf.Abs(Mathf.Pow(x - radius, 2)) + Mathf.Abs(Mathf.Pow(y - radius, 2)) - Mathf.Pow(radius, 2) < 1)
                    {
                        int surroundingTiles = GetSurroundingTileCount(array, x, y);

                        if (surroundingTiles > 4)
                        {
                            array[x, y] = aliveValue;
                        }

                        if (surroundingTiles < 4)
                        {
                            array[x, y] = deadValue;
                        }
                    }
                    else if (Random.value < 0.5)
                    {
                        int surroundingTiles = GetSurroundingTileCount(array, x, y);

                        if (surroundingTiles > 4)
                        {
                            array[x, y] = aliveValue;
                        }

                        if (surroundingTiles < 4)
                        {
                            array[x, y] = deadValue;
                        }
                    }
                }
            }
        }
    }

    public static void MooreCellularAutomaton(this int[,] array, int smoothCount)
    {
        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x < array.GetUpperBound(0); x++)
            {
                for (int y = 0; y < array.GetUpperBound(1); y++)
                {
                    int surroundingTiles = GetSurroundingTileCount(array, x, y);

                    if (surroundingTiles > 4)
                    {
                        array[x, y] = 1;
                    }

                    if (surroundingTiles < 4)
                    {
                        array[x, y] = 0;
                    }
                }
            }
        }
    }

    private static int GetSurroundingTileCount(int[,] array, int x, int y)
    {
        int surroundingTileCount = 0;
        for (int X = x - 1; X <= x + 1; X++)
        {
            for (int Y = y - 1; Y <= y + 1; Y++)
            {
                if (X >= 0 && X < array.GetUpperBound(0) && Y >= 0 && Y < array .GetUpperBound(1))
                {
                    if (Y != y || X != x)
                    {
                        surroundingTileCount += array[X, Y];
                    }
                }
            }
        }
        return surroundingTileCount;
    }
}

[System.Serializable]
public class WorldLayer
{
    public string layerName;

    #region Layer Surface Generation
    [Header("Layer Surface Properties")]
    public int layerHeight = 25;

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

    public TileBase layerTile;
    #endregion

    #region Layer Cave Generation
    [Header("Cave Generation Properties")]
    [Range(0.000001f, 0.5f)]
    public float caveFrequency;

    [Range(0, 1)]
    public float caveAmplitude;

    [Range(0f, 1f)]
    public float cavePersistence;

    [Range(1, 10)]
    public int caveLacunarity;
    #endregion


    [HideInInspector] public int[] upperHeightMap;

    [HideInInspector] public int[] lowerHeightMap;
}
