using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Object/Data/Damage/Weapon Data")]
[InlineEditor]
public class WeaponData : ScriptableObject
{
    public bool ranged; 
    public bool comboBasedDamage = false;
    public bool upgradeable = false;

    [PropertyRange(0.01f, 50f)]
    public float cooldown;

    [ShowIf("comboBasedDamage")]
    public int[] damages;

    [HideIf("comboBasedDamage")]
    public int damage;

    [ShowIf("ranged")]
    public AmmoObject ammoData;
    [ShowIf("ranged")]
    public float projectileSpeed;

    [ShowIf("upgradeable")]
    public WeaponData upgradedData;
 
    public ColliderData colliderData;
}

[System.Serializable]
public class ComboData
{
    public int damage;
    public ColliderData colliderData;
    public float attackDelay;
    public string animationName;
    public float animationLength;
    public float boostX;
    public MeleeAnimationType animationType;
    [Range(0f, 1f)]
    public int animationIndex;
    public AudioClip audioClip;
    public Vector2 knockback;

    [HideInInspector] public int animation;

    public int Animation
    {
        get
        {
            if (animation == 0)
            {
                animation = Animator.StringToHash(animationName);
            }

            return Animator.StringToHash(animationName);
        }
    }
}