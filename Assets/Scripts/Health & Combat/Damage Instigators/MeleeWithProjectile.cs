using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class MeleeWithProjectile : MeleeAbility
{
    #region Serialized Fields
    [FoldoutGroup("Ability References"), SerializeField]
    private AmmoObject ammo;
    [FoldoutGroup("Ability References"), SerializeField]
    private Movement movement;
    [FoldoutGroup("Ability References"), SerializeField]
    private Transform firePoint;
    [FoldoutGroup("Ability References"), SerializeField]
    private Transform endpoint;

    [FoldoutGroup("Ability Properties"), SerializeField]
    private float range;

    [FoldoutGroup("Ability Feedbacks"), SerializeField]
    private MMF_Player shootFeedback;

    private Shooter shooter;
    #endregion

    #region Unity Update Methods
    public override void Start()
    {
        base.Start();
        shooter = new Shooter(firePoint, ammo, GetProjectileTrajectory, 30f);
    }
    #endregion

    #region Methods
    public override IEnumerator PerformAbilityCoroutine(float delay = 0)
    {
         if (TargetsAvailable())
        {
            shootFeedback?.PlayFeedbacks();
            yield return new WaitForSeconds(delay);
            shooter.Shoot();
        }
    }

    public void ExtendAttackRange(Vector2 extension)
    {
         endpoint.transform.localPosition += (Vector3)extension;
    }
    #endregion

    #region Data
    public override bool TargetsAvailable()
    {
        return Physics2D.Linecast(transform.position, endpoint.position, colliderData.targetLayers);
    }

    public Vector2 GetProjectileTrajectory()
    {
        if (movement.IsFacingRight())
        {
            return new Vector2(1, 0);
        }
        return new Vector2(-1, 0);
    }
    #endregion

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Debug.DrawLine(meleePoint.position, endpoint.position);
    }
}
