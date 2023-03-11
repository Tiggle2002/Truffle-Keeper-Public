using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class DamageWithKnockback : DamageInstigator
{
    #region Knockback Properties
    [FoldoutGroup("Knockback Properties")]
    [SerializeField] protected bool applyKnockback;

    [FoldoutGroup("Knockback Properties"), ShowIf("applyKnockback", true), MinValue(0)]
    [SerializeField] protected Vector2 force = Vector2.zero;

    [FoldoutGroup("Knockback Properties"), ShowIf("applyKnockback", true), MinValue(0)]
    [SerializeField] protected Vector2 randomness = Vector2.zero;
    #endregion

    #region Damage Methods
    protected override void DealDamage()
    {
        if (healthToDamage.Damagable())
        {
            ApplyKnockback();
            healthToDamage.Damage(damage, this);
        }
    }
    #endregion

    #region Knockback Methods
    public void SetKnockback(Vector2 newKnockback)
    {
        force = newKnockback;
    }

    protected virtual void ApplyKnockback()
    {
        if (!applyKnockback) return;

        if (healthToDamage is CharacterHealth)
        {
            CharacterHealth characterHealth = healthToDamage as CharacterHealth;
            if (characterHealth.ImmuneToKnockback())
            {
                return;
            }

            characterHealth.Knockback(transform.position, GetKnockback());
        }
    }

    protected Vector2 GetKnockback()
    {
        Vector2 knockbackForce = force + GetKnockbackRandomness();

        return knockbackForce;
    }

    protected Vector2 GetKnockbackRandomness()
    {
        if (randomness == Vector2.zero)
        {
            return Vector2.zero;
        }

        float rndKnockbackX = Random.Range(0, randomness.x);
        float rndKnockbackY = Random.Range(0, randomness.y);

        return new Vector2(rndKnockbackX, rndKnockbackY);
    }
    #endregion
}
