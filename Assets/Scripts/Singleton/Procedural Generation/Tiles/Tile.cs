using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SurvivalElements;

[CreateAssetMenu(fileName = "Tile", menuName = "World Generation/Tile/Tile")]
public class Tile : RuleTile<Tile.Neighbor> {
    public bool customField;
    
    [SerializeField] private int tileHealth = 5;
    public int health { get { return tileHealth; } }

    public AudioClip minedSound;

    #region Rule Tile Methods
    public List<TileBase> Siblings = new List<TileBase>();

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        base.RuleMatch(neighbor, other);
        switch (neighbor)
        {
            case Neighbor.This:
                {
                    return other == this || Siblings.Contains(other);
                }
            case Neighbor.NotThis:
                {
                    return other != this && !Siblings.Contains(other);
                }
        }
        return base.RuleMatch(neighbor, other);
    }
    #endregion

}

public enum TileType { Block, Wall }