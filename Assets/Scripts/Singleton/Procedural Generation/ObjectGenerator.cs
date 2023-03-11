using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class WorldObject
{
    public string name;

    public TileBase[] objectTiles;
}

public class ObjectGenerator : MonoBehaviour
{
    [Header("Tree Tiles")]
    public TileBase oakWoodTile;
    public TileBase oakLeavesTile;

    [Header("Water Tile")]
    public TileBase waterTile;
    public TileBase dirtTile;

    [Header("World Blocks")]
    public TileBase stickTile;
    public TileBase rockTile;

    public (TileBase[] tiles, int[,] objectMap) GenerateTree()
        {
            int treeHeight = Random.Range(7, 8 + 1);
            int treeWidth = /*Random.value < 0.5 ? 3 : 5;*/ 4;

            int[,] treeMap = new int[treeWidth, treeHeight + 3];

            TileBase[] treeTiles = new TileBase[] { null, oakWoodTile , oakLeavesTile };

            if (treeWidth == 3)
            {
                treeMap[1, 0] = 1;
                treeMap[0, 0] = Random.value < 0.5 ? 1 : 0;
                treeMap[2, 0] = Random.value < 0.5 ? 1 : 0;
            }
            else
            {
                treeMap[1, 0] = 1;
                treeMap[0, 0] = Random.value < 0.5 ? 1 : 0;
                treeMap[2, 0] = Random.value < 0.5 ? 1 : 0;
            }

            for (int y = 1; y < treeHeight - 3; y++)
            {
                treeMap[1, y] = 1;
            }

            for (int x = 0; x < treeMap.GetUpperBound(0); x++)
            {
                for (int y = treeHeight - 3; y < treeMap.GetUpperBound(1); y++)
                {
                    treeMap[x, y] = 2;
                }
            }

            return (treeTiles, treeMap);
        }

    public (TileBase[] tiles, int[,] objectMap) GenerateRiver()
    {
        TileBase[] waterTiles = new TileBase[] { dirtTile, waterTile };

        int[,] objectMap = NoiseGenerator.GenerateSemiCircle(10);

        return (waterTiles, objectMap);
    }


}
