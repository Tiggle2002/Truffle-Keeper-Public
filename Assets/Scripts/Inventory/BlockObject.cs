using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SurvivalElements
{
    [CreateAssetMenu(fileName = "Block", menuName = "Scriptable Object/Survival Element/Item/Block")]
    public class BlockObject : ItemObject
    {
        public Tile tile;
    }
}