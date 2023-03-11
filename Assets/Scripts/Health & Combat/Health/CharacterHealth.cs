using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterHealth : Health
{
    #region Knockback Properties
    [FoldoutGroup("Damage Properties")]
    [SerializeField] private bool takesKnockback;

    [FoldoutGroup("Damage Properties"), ShowIf("takesKnockback", true)]
    [SerializeField] private Vector2 knockbackResistance;
    #endregion

    #region Knockback Methods
    public virtual void Knockback(Vector2 instigatorPos, Vector2 force)
    {
        Vector2 knockbackForce = force - knockbackResistance;
        Mathf.Clamp(knockbackForce.x, 0, 100);
        Mathf.Clamp(knockbackForce.y, 0, 100);
        PhysicsMethods.Knockback(rb, knockbackForce, instigatorPos);
    }

    public bool ImmuneToKnockback()
    {
        return !takesKnockback;
    }
    #endregion

    protected override void TriggerDeathEvent()
    {
        base.TriggerDeathEvent();
        SpawnLoot();
    }

    public void SpawnLoot()
    {
        if (!entityObject.lootable)
        {
            return;
        }
        
        entityObject.lootToDrop.SpawnMultipleLoot(transform.position + Vector3.up);
    }
}
