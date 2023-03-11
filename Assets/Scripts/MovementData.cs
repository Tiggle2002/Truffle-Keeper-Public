using Sirenix.OdinInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[InlineEditor, CreateAssetMenu(fileName = "Movement Data", menuName = "Scriptable Object/Data/Movement Data")]
public class MovementData : ScriptableObject
{
    #region Speed Variables       
    [SerializeField, Range(0f, 100f), FoldoutGroup("Speed Settings")]
    public float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f), FoldoutGroup("Speed Settings")]
    public float maxVerticalSpeed = 4f;
    [SerializeField, Range(0f, 100f), FoldoutGroup("Speed Settings")]
    public float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f), FoldoutGroup("Speed Settings")]
    public float maxAirAcceleration = 20f;
    #endregion

    #region Jump Variables
    [SerializeField, Range(0f, 100f), FoldoutGroup("Jump Settings")]
    public float jumpHeight = 4f;
    [SerializeField, Range(0, 5), FoldoutGroup("Jump Settings")]
    public int maxJumps = 0;
    [SerializeField, Range(0f, 100f), FoldoutGroup("Jump Settings")]
    public float downwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 100f), FoldoutGroup("Jump Settings")]
    public float upwardMovementMultiplier = 1.75f;
    [SerializeField, FoldoutGroup("Jump Settings")]
    public float coyoteTime = 0.5f;
    [SerializeField, Range(0.1f, 1f), FoldoutGroup("Jump Settings")]
    public float groundCheckBoxSize = 0.1f;
    [SerializeField, FoldoutGroup("Jump Settings")]
    public float defaultGravityScale = 1f;
    #endregion
}
