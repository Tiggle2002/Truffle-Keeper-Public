using MoreMountains.Tools;
using SurvivalElements;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlaceableItem : Item
{
    [Range(0, 8)]
    public int placementRange;

    public Vector3 offset;

    private BoxCollider2D bc;

    private Tilemap blockTileMap;

    private Vector3Int highlightedTilePos;

    private Vector3Int mousePosOnGrid
    {
        get
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Vector3Int mouseGridPos = blockTileMap.WorldToCell(mousePos);

            mouseGridPos.z = 0;

            return mouseGridPos;
        }
    }

    private Vector3Int playerPos
    {
        get
        {
            return blockTileMap.WorldToCell(PlayerManager.Instance.transform.position);
        }
    }

    private bool playerInPlacementRange
    {
        get
        {
            return Calculator.InRange(playerPos, mousePosOnGrid, placementRange);
        }
    }

    public GameObject itemToPlacePrefab;

    protected override void Awake()
    {
        base.Awake();

        blockTileMap = GameObject.Find("World Grid").transform.GetChild(0).GetComponent<Tilemap>();
    }



    public override void Update()
    {
       base.Update();
       CheckConditions();
    }

    private void HighlightIfPlaceable()
    {
        sr.enabled = true;
    }


    protected override void Process()
    {
        base.Process();
        if (!CheckConditions())
            {
            return;
        }

        Instantiate(itemToPlacePrefab, mousePosOnGrid, Quaternion.identity);
        InventoryEvent.Trigger(InventoryEventType.ItemConsumed, new EventItem(itemObject, 1));
    }

    private bool CheckConditions()
    {
        if (playerInPlacementRange)
        {
            if (true)
            {
                if (EmptySpace() && CheckForTileUnder() && CheckStructureNotAlreadyPresent())
                {
                    transform.position = mousePosOnGrid + offset;
                    transform.position += (Vector3Int.one / 2);
                    sr.enabled = true;
                    return true;
                }
            }
        }
        else
        {
            sr.enabled = false;
            return false;
        }
        return false;
    }

    private bool EmptySpace()
    {
        if (blockTileMap.GetTile<Tile>(mousePosOnGrid) == null)
        {
            return true;
        }

        return false;
    }

    private bool CheckForTileUnder()
    {

        if (blockTileMap.GetTile<Tile>(mousePosOnGrid + Vector3Int.down) != null)
        {
            return true;
        }

        return false;
    }

    private bool CheckStructureNotAlreadyPresent()
    {
        return true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(mousePosOnGrid, Vector2.one);
    }

    public override void CancelUse(bool finishCurrent = false)
    {
        throw new System.NotImplementedException();
    }
}
