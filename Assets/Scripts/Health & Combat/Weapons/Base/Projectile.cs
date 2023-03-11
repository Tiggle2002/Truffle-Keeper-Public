using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Projectile : DamageOnCollision
{
    #region Serialized Variables
    [FoldoutGroup("Collision Settings")]
    [HideIf("unityManagedCollisions")]
    [SerializeField, Required] protected ColliderData explosiveCollider;
    [FoldoutGroup("Projectile References"), SerializeField]
    private AmmoObject ammoObject;
    [FoldoutGroup("Projectile References"), SerializeField]
    private MMF_MMSoundManagerSound hitSoundFX;

    [FoldoutGroup("Projectile Settings"), MinValue(1), SerializeField]
    private float despawnTime;
    [FoldoutGroup("Projectile Settings"), MinValue(1), SerializeField]
    private bool damageInstigatorPosition;

    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player projectileSpawnFeedback;
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player projectileDespawnFeedback;
    #endregion

    #region Variables
    private Vector2 instigatorPos;
    private Timer despawnTimer;
    private SpriteRenderer sr;

    private Vector2 trajectory;
    private bool canFly = true;
    private float speed;

    private float initialVolume;
    private CollisionDetector explosiveDetector;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();

        if (hitSoundFX != null)
        {
            initialVolume = hitSoundFX.MinVolume;
        }
        despawnTimer = new(despawnTime, true);
    }

    protected override void CreateCollider()
    {
        collisionDetector = colliderData.type == ColliderType.Circle ? new CircleCollisionDetector(colliderData, transform) : new SquareCollisionDetector(colliderData, transform);

        if (ammoObject.explosive)
        {
            collisionDetector.AddImpenetrableAction(TimeExplosion).AddMaxPenetrationsReachedAction(Explode);

            explosiveDetector = colliderData.type == ColliderType.Circle ? new CircleCollisionDetector(explosiveCollider, transform) : new SquareCollisionDetector(explosiveCollider, transform);
            explosiveDetector.AddHitAction(DamageCollider);
        }
        else
        {
            collisionDetector.AddImpenetrableAction(PoolProjectile).AddHitAction(DamageCollider).AddMaxPenetrationsReachedAction(PoolProjectile);
        }
        collisionDetector.AddHitRequirment(TargetIsDamageable);
    }

    protected override void Update()
    {
        if (canFly)
        {
            base.Update();
            RotateTowardsMovement();
            despawnTimer.Countdown();
        }
    }

    public void FixedUpdate()
    {
        if (canFly)
        {
            ProjectileMovement();
        }
    }
    #endregion

    #region Methods
    public bool TargetIsDamageable(Collider2D target)
    {
        healthToDamage = target.gameObject.MMFGetComponentNoAlloc<Health>();

        return healthToDamage && healthToDamage.Damagable();
    }

    public void Initialise(Vector2 instigatorPos, Vector2 trajectory, int damage, float speed)
    {
        this.damage = damage + ammoObject.damage;
        this.speed = speed;
        this.instigatorPos = instigatorPos;
        this.trajectory = trajectory;

        StartProjectile();
    }

    private void StartProjectile()
    {
        collisionDetector.Reset();
        explosiveDetector?.Reset();
        despawnTimer.ResetCountdown();
        projectileSpawnFeedback?.PlayFeedbacks();
        EnableProjectile();

        if (ammoObject.dropoff)
        {
            LaunchProjectile();
        }
        if (ammoObject.explosive)
        {
            StartCoroutine(ExplodeAfterDelay(ammoObject.explosionDelay));
        }
    }

    private void TimeExplosion()
    {
        canFly = false;
        rb.velocity /= 2;
        rb.AddTorque(50f);
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        explosiveDetector.CheckForCollisions();
        PoolProjectile();
    }

    protected override void DamageCollider(Collider2D collider)
    {
        healthToDamage = collider.gameObject.MMFGetComponentNoAlloc<Health>();

        if (healthToDamage)
        {
            TryDamage(healthToDamage);
        }
    }

    protected override void TryDamage(Health healthToDamage)
    {
        if (healthToDamage != null && healthToDamage.Damagable())
        {
            this.healthToDamage = healthToDamage;
            DealDamage();
            PlayHitFeedback();
            if (hitSoundFX != null)
            {
                hitSoundFX.MinVolume += 0.1f;
            }
        }
    }

    protected override void ApplyKnockback()
    {
        if (!applyKnockback)
        {
            return;
        }

        if (healthToDamage is CharacterHealth)
        {
            CharacterHealth characterHealth = healthToDamage as CharacterHealth;
            if (characterHealth.ImmuneToKnockback())
            {
                return;
            }

            if (damageInstigatorPosition)
            {
                instigatorPos = transform.position;
            }
            characterHealth.Knockback(instigatorPos, GetKnockback());
        }
    }

    protected virtual void ProjectileMovement()
    {
        Vector2 desiredVelocity = trajectory * speed;
        if (!ammoObject.dropoff)
        {
            rb.velocity = desiredVelocity;
        }
        else
        {
            rb.gravityScale = rb.velocity.y > 0 ? 0.95f : 1.3f;
        }
    }

    private void LaunchProjectile()
    {
        rb.AddForce(trajectory * speed, ForceMode2D.Impulse);
    }

    private void RotateTowardsMovement()
    {
        if (ammoObject.dropoff)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    protected void EnableProjectile()
    {
        sr.enabled = true;
        canFly = true;
    }

    private IEnumerator DespawnProjectile()
    {
        yield return projectileDespawnFeedback?.PlayFeedbacksCoroutine(transform.position);
        Despawn();
    }

    protected void DisableProjectile()
    {
        rb.velocity = Vector2.zero;
        sr.enabled = false;
        canFly = false;
        if (hitSoundFX != null)
        {
            hitSoundFX.MinVolume = initialVolume;
        }
    }

    private void PoolProjectile()
    {
        DisableProjectile();
        StartCoroutine(DespawnProjectile());
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }
    #endregion

    protected override void OnDrawGizmos()
    {
        if (colliderData.type == ColliderType.Circle)
        {
            Gizmos.DrawWireSphere(transform.position + colliderData.offset, colliderData.circleRadius);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position + colliderData.offset, colliderData.squareSize);
        }
    }
}