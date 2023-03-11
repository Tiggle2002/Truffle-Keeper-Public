using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    public Tilemap Tilemap;
    public TileBase tile;
    public int chunkSize;

    public void Start()
    {
        StartCoroutine(LoadChunk());
    }

    public IEnumerator LoadChunk()
    {
        for (int x = 0; x < chunkSize - 1; x++)
        {
            for (int y = 0; y < chunkSize - 1; y++)
            {
                Vector3Int tilePos = new(x, y);
                Tilemap.SetTile(tilePos, tile);
                yield return null;
            }
        }
    }
}
