using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HitAndHurt : DamageOnCollision
{
    [FoldoutGroup("Damage Properties")]
    [HideIf("damageManagedExternally")]
    [SerializeField] protected int damageToSelf = 1;

    private Health health;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
    }

    protected override void DealDamage()
    {
        if (healthToDamage.Damagable() && !healthToDamage.ImmuneToInstigator(this))
        {
            ApplyKnockback();
            healthToDamage.Damage(damage, this);
            PlayHitFeedback();
            DamageSelf();
        }
    }

    protected void DamageSelf()
    {
        if (health.Damagable())
        {
            health.Damage(damageToSelf, this);
        }
    }
}