using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

public enum ColliderType { Circle, Square, Line }

public class DamageOnCollision : DamageWithKnockback
{
    [FoldoutGroup("Collision Settings")]
    [SerializeField] private bool unityManagedCollisions = true;
    [FoldoutGroup("Collision Settings")]
    [SerializeField] private bool alwaysOn = false;
    [FoldoutGroup("Collision Settings")]
    [SerializeField] private float timeToWaitUntilRepeat = 1;

    [FoldoutGroup("Collision Settings")]
    [HideIf("unityManagedCollisions")]
    [SerializeField, Required] protected ColliderData colliderData;

    [FoldoutGroup("Feedback")]
    [SerializeField] private MMF_Player targetHitFeedback;

    protected CollisionDetector collisionDetector;
    protected List<Collider2D> collidersAlreadyHit = new();

    #region Unity Update Methods
    protected override void Awake()
    {
        base.Awake();
        if (!unityManagedCollisions)
        {
            CreateCollider();
        }
    }

    protected virtual void Update()
    {
        if (!unityManagedCollisions && alwaysOn)
        {
            collisionDetector?.CheckForCollisions();
        }
    }
    #endregion

    protected IEnumerator EnableRepeatHits(Collider2D collider)
    {
        yield return new WaitForSeconds(timeToWaitUntilRepeat);

        if (unityManagedCollisions)
        {
            if (collidersAlreadyHit.Contains(collider))
            {
                collidersAlreadyHit.Remove(collider);
            }
        }
        else
        {
            collisionDetector.EnableRepeatHitsOnCollider(collider);
        }
    }

    protected virtual void CreateCollider()
    {
        collisionDetector = colliderData.type == ColliderType.Circle ? new CircleCollisionDetector(colliderData, transform) : new SquareCollisionDetector(colliderData, transform);
        collisionDetector.AddHitAction(DamageCollider);
    }

    #region Damage
    protected virtual void DamageCollider(Collider2D collider)
    {
        healthToDamage = collider.gameObject.MMFGetComponentNoAlloc<Health>();

        if (healthToDamage)
        {
            TryDamage(healthToDamage);
            if(unityManagedCollisions)
            {
                collidersAlreadyHit.Add(collider);
            }

            if (alwaysOn)
            {
                StartCoroutine(EnableRepeatHits(collider));
            }
        }
    }

    protected override void DealDamage()
    {
        int damageToApply = healthToDamage.ImmuneToInstigator(this) ? 0 : damage;

        if (healthToDamage.Damagable())
        {
            ApplyKnockback();
            healthToDamage.Damage(damageToApply, this);
            PlayHitFeedback();
        }
    }

    protected void PlayHitFeedback()
    {
        if (healthToDamage is CharacterHealth)
        {
            targetHitFeedback?.PlayFeedbacks();
        }
    }
    #endregion

    #region Collision Detection Events
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utilities.LayerInLayerMask(collision.gameObject.layer, targetLayers) || !unityManagedCollisions)
        {
            return;
        }



        Collider2D collider = collision.gameObject.MMFGetComponentNoAlloc<Collider2D>();
        if (!collidersAlreadyHit.Contains(collider))
        {
            DamageCollider(collider);
        }
    }

    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!Utilities.LayerInLayerMask(collision.gameObject.layer, targetLayers) || !unityManagedCollisions)
        {
            return;
        }


        Collider2D collider = collision.gameObject.MMFGetComponentNoAlloc<Collider2D>();
        if (!collidersAlreadyHit.Contains(collider))
        {
            DamageCollider(collider);
        }

    }
    #endregion

    #region Trigger Detection Events
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utilities.LayerInLayerMask(collision.gameObject.layer, targetLayers) || !unityManagedCollisions)
        {
            return;
        }


        if (!collidersAlreadyHit.Contains(collision))
        {
            DamageCollider(collision);
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!Utilities.LayerInLayerMask(collision.gameObject.layer, targetLayers) || !unityManagedCollisions )
        {
            return;
        }

        if (!collidersAlreadyHit.Contains(collision))
        {
            DamageCollider(collision);
        }
    }
    #endregion

    protected virtual void OnDrawGizmos()
    {
        switch (colliderData.type)
        {
            case ColliderType.Circle:
                Gizmos.DrawWireSphere(transform.position + colliderData.offset, colliderData.circleRadius);
                break;
            case ColliderType.Square:
                Gizmos.DrawWireCube(transform.position + colliderData.offset, colliderData.squareSize);
                break;
        }
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {

    }
}
