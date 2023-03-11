using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Shooter
{
    #region MyRegion
    private Transform firePoint;
    private ObjectPool<PoolObject> ammoPool;
    private int damage;
    private float speed;
    private AmmoObject projectile;
    private Func<Vector2> ProjectileTrajectory;
    #endregion

    public Shooter(Transform firePoint, AmmoObject projectile, Func<Vector2> projectileTrajectory, float speed, int poolSize = 5)
    {
        this.firePoint = firePoint;
        this.speed = speed;
        this.projectile = projectile;
        this.ProjectileTrajectory = projectileTrajectory;

        damage = projectile.damage;
        ammoPool = new(projectile.prefab, poolSize, firePoint);
    }

    public virtual void Shoot()
    {
        Projectile projectile = ammoPool.Pull().GetComponent<Projectile>();
        InitialiseProjectile(projectile);
    }

    protected virtual void InitialiseProjectile(Projectile projectile)
    {
        SetProjectilePositionAndRotation(projectile);

        Vector2 trajectory = ProjectileTrajectory.Invoke(); ;

        projectile.Initialise(firePoint.position, trajectory, damage, speed) ;
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

    public void SetAmmoPool(AmmoObject projectile, int poolSize)
    {
        ammoPool.Clear();
        ammoPool = new(projectile.prefab, 5, firePoint);
    }
}
