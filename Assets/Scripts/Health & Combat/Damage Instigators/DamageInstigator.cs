using Sirenix.OdinInspector;
using UnityEngine;

public class DamageInstigator : MonoBehaviour
{
    #region Damage Properties
    [SerializeField] protected bool damageManagedExternally;
    [FoldoutGroup("Damage Properties")]
    [HideIf("damageManagedExternally")]
    [SerializeField] protected int damage = 1;

    [FoldoutGroup("Damage Properties")]
    [SerializeField] protected bool passiveDamage = false;

    [FoldoutGroup("Damage Properties")]
    [HideIf("damageManagedExternally")]
    [SerializeField] protected LayerMask targetLayers;
    #endregion

    #region References
    public Rigidbody2D rb { get; private set; }
    protected Health healthToDamage;
    #endregion

    #region Unity Update Methods
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Damage Methods
    protected virtual void TryDamage(Health healthToDamage)
    {
        if (healthToDamage != null && healthToDamage.Damagable())
        {
            this.healthToDamage = healthToDamage;
            DealDamage();
        }
    }

    protected virtual void DealDamage()
    {
        if (healthToDamage.Damagable())
        {
            healthToDamage.Damage(damage, this);
        }
    }

    public virtual int DamageAmount()
    {
        return damage;
    }

    public bool PassiveDamager()
    {
        return passiveDamage;
    }
    #endregion
}

