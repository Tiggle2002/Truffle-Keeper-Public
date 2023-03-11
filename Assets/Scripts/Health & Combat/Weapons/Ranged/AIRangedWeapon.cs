using Sirenix.OdinInspector;
using UnityEngine;


public class AIRangedWeapon : MonoBehaviour
{
    #region References
    [SerializeField, Required] private GameObject firePoint;
    [SerializeField, Required] private WeaponData weaponData;
    #endregion

    #region Fields
    private Timer shootCooldown;
    private Shooter shooter;
    private ZoneTargetSelector targetSelector;
    private bool weaponEnabled = true;
    private bool isFacingRight = true;
    #endregion

    #region Unity Update Methods
    public void Awake()
    {
        InitialiseWeaponComponents();
        SetWeaponEnabled(true);
    }

    public void Update()
    {
        shootCooldown.Countdown();
        if (weaponEnabled)
        {
            Rotate();
            Shoot();
        }
    }
    #endregion

    private void InitialiseWeaponComponents()
    {
        targetSelector = GetComponent<ZoneTargetSelector>();
        shooter = new(firePoint.transform, weaponData.ammoData, GetProjectileTrajectory, weaponData.projectileSpeed);
        shootCooldown = new(weaponData.cooldown);
    }

    private void Rotate()
    {
        SetRotation();
        FlipAccordingToMovement(targetSelector.TargetDirectionSign());
        SetItemScale();
    }

    public void Shoot()
    {
        if (CanFire())
        {
            shooter.Shoot();
            shootCooldown.ResetCountdown();
        }
    }

    private void SetRotation()
    {
        if (!targetSelector.TargetAvailable())
        {
            return;
        }
        Vector3 rotationChange = targetSelector.GetTargetDirection();
        float rotationZ = Mathf.Atan2(rotationChange.y, rotationChange.x) * Mathf.Rad2Deg;
        Mathf.Clamp(rotationZ, -90f, 90f);
        Quaternion newRot = Quaternion.Euler(0f, 0f, rotationZ);
        transform.rotation = newRot;
    }

    private void SetItemScale()
    {
        if (isFacingRight)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, -1f, 1f);
        }
    }

    public void FlipAccordingToMovement(float direction)
    {
        if (isFacingRight && direction < 0)
        {
            isFacingRight = false;

            transform.localRotation = Quaternion.Euler(0, -180f, 0);
        }
        if (!isFacingRight && direction > 0)
        {
            isFacingRight = true;

            transform.localRotation = Quaternion.Euler(0, 0f, 0);
        }
    }

    private bool CanFire()
    {
        return targetSelector.TargetAvailable() && shootCooldown.Done();
    }

    private Vector2 GetProjectileTrajectory()
    {
        return targetSelector.GetTargetDirectionFrom(firePoint.transform);
    }

    private void SetWeaponEnabled(int healthPercentage)
    {
        if (healthPercentage > 0 && weaponEnabled == false)
        {
            SetWeaponEnabled(true);
        }
        if (healthPercentage == 0)
        {
            SetWeaponEnabled(false);
        }
    }

    private void SetWeaponEnabled(bool enabled)
    {
        weaponEnabled = enabled;
    }

    public void Upgrade()
    {
        weaponData = weaponData.upgradedData;
        shooter.SetAmmoPool(weaponData.ammoData, 3);
        targetSelector.ChangeColliderData(weaponData.colliderData);
    }

    public void OnEnable()
    {
        GetComponentInChildren<Health>(true).AddListener(SetWeaponEnabled);
        GetComponentInChildren<Structure>(true).AddUpgradedListener(Upgrade);
    }

    public void OnDisable()
    {
        GetComponentInChildren<Health>(true).RemoveListener(SetWeaponEnabled);
        GetComponentInChildren<Structure>(true).RemoveUpgradedListener(Upgrade);
    }
}
