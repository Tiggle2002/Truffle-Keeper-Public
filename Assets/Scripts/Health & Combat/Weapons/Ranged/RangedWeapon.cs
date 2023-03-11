using UnityEngine;
using SurvivalElements;
using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;

public class RangedWeapon : Item
{
    #region Variables
    [SerializeField] public bool consumable;
    [SerializeField] protected int ammoPoolSize;
    [SerializeField] protected GameObject firePoint;

    private AmmoCounter ammoCounter;
    [SerializeField] private bool infiniteAmmo;

    [FoldoutGroup("Feedbacks")]
    [HideIf("infiniteAmmo")]
    [SerializeField] private MMF_Player noAmmoFeedback;

    protected Shooter shooter;
    private Coroutine fireCoroutine;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        shooter = new(firePoint.transform, itemObject.ammoObject, GetProjectileTrajectory, itemObject.range);
        if (!infiniteAmmo)
        {
            ammoCounter = new(itemObject.ammoObject);
        }
    }
    #endregion

    #region Weapon Methods
    protected override void Process()
    {
        if (!itemObject.holdToUse)
        {
            FireWeapon();
            return;
        }

        if (fireCoroutine != null)
        {
            return;
        }

        fireCoroutine = StartCoroutine(FireWeaponAutomatic());
    }

    private void FireWeapon()
    {
        if (CanFire())
        {
            base.Process();
            if (!infiniteAmmo)
            {
                if (consumable)
                {
                    InventoryEvent.Trigger(InventoryEventType.ItemRemoved, new(itemObject, 1));
                }
                else
                {
                    InventoryEvent.Trigger(InventoryEventType.ItemConsumed, new EventItem(itemObject.ammoObject, 1));
                }
            }
            shooter.Shoot();
        }
        else if (!infiniteAmmo)
        {
            noAmmoFeedback?.PlayFeedbacks();
        }
    }

    private IEnumerator FireWeaponAutomatic()
    {
        while (true)
        {
            FireWeapon();
            yield return null;
        }
    }

    protected virtual bool CanFire() => (itemObject.ammoBased && ammoCounter.EnoughAmmo()) || consumable;

    protected virtual void InitialiseProjectile(Projectile projectile)
    {
        SetProjectilePositionAndRotation(projectile);
 
        Vector2 trajectory = GetProjectileTrajectory();

        projectile.Initialise(firePoint.transform.position, trajectory, itemObject.damage, itemObject.range);
    }

    protected virtual Vector2 GetProjectileTrajectory()
    {
        return PlayerManager.Instance.GetProjectileTrajectory().normalized;
    }

    protected virtual void SetProjectilePositionAndRotation(Projectile projectile)
    {
        projectile.transform.position = firePoint.transform.position;
        projectile.transform.rotation = firePoint.transform.rotation;
        projectile.gameObject.Unparent();
    }

    public override void CancelUse(bool finishCurrent = false)
    {
        if (animator != null)
        {
            animator.Play("Idle");
        }
        sr.enabled = false;
        ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    protected override IEnumerator PlayItemAnimation()
    {
        if (animator == null)
        {
            yield break;
        }
        ItemEvent.Trigger(ItemEventType.ItemInUse, itemObject);
        sr.enabled = true;
        animator.Play("Use");
        yield return new WaitForSeconds(itemObject.useLength);
        if (!itemObject.holdToUse)
        {
            animator.Play("Idle");
            sr.enabled = false;
            ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
        }
    }
    #endregion

    public void OnDestroy()
    {
        ammoCounter?.Dispose();
    }
}

public class AmmoCounter : TEventListener<InventoryEvent>, IDisposable
{
    #region Variables
    private int ammoCount;
    #endregion

    #region References
    private AmmoObject ammo;
    #endregion

    public AmmoCounter(AmmoObject ammo)
    {
        this.ammo = ammo;
        ammoCount = PlayerManager.Instance.inventory.FindQuantityOfItem(ammo);

        this.Subscribe<InventoryEvent>();
    }

    public bool EnoughAmmo()
    {
        Debug.Log(ammoCount);
        return ammoCount > 0;
    }

    public void ConsumeAmmo()
    {
        ammoCount--;
    }

    public void AddAmmo(int amount)
    {
        ammoCount += amount;
    }

    public bool AmmoMatches(string ammoID)
    {
        return ammoID == ammo.ID;
    }

    public void OnEvent(InventoryEvent eventData)
    {
        if (eventData.eventItem == null || eventData.eventItem.item == null)
        {
            return;
        }
        if (eventData.eventItem.item.type != ItemType.Ammunition || !AmmoMatches(eventData.eventItem.item.ID))
        {
            return;
        }

        switch (eventData.eventType)
        {
            case InventoryEventType.ItemAdded:
                AddAmmo(eventData.eventItem.quantity);
                break;
            case InventoryEventType.ItemPurchased:
                AddAmmo(eventData.eventItem.quantity);
                break;
            case InventoryEventType.ItemConsumed:
                ConsumeAmmo();
                break;
        }
    }

    public void Dispose()
    {
        this.Unsubscribe<InventoryEvent>();
    }
}
