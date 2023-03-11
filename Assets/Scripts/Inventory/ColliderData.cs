using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Collider Data", menuName = "Scriptable Object/Data/Collider Data")]
[InlineEditor]
public class ColliderData : ScriptableObject 
{
    [BoxGroup("Weapon Properties")]
    public bool limitedPenetrations;
    [BoxGroup("Weapon Properties")]
    [ShowIf("limitedPenetrations")]
    public int maxPenetrationCount;
    [BoxGroup("Weapon Properties")]
    public LayerMask targetLayers;
    [BoxGroup("Weapon Properties")]
    public LayerMask impenetrableLayer;

    [BoxGroup("Collider Properties"), EnumToggleButtons]
    public ColliderType type;
    [BoxGroup("Collider Properties"), ShowIf("type", ColliderType.Square)]
    public Vector2 squareSize;
    [BoxGroup("Collider Properties"), ShowIf("type", ColliderType.Circle)]
    public float circleRadius;
    [BoxGroup("Collider Properties"), ShowIf("type", ColliderType.Line)]
    public float lineLength;
    [BoxGroup("Collider Properties")]
    public Vector3 offset;
}
