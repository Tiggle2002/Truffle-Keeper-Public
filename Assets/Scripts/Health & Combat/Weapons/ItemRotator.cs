using Sirenix.OdinInspector;
using SurvivalElements;
using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    #region Fields
    [FoldoutGroup("Rotation Settings"), SerializeField]
    private bool delayedRotation;
    [ShowIf("delayedRotation"), FoldoutGroup("Rotation Settings"), SerializeField]
    private float rotationSpeed;
    [FoldoutGroup("Rotation Settings"), SerializeField]
    private float rotationOffset;
    [FoldoutGroup("Rotation Settings"), SerializeField]
    private bool canChangeItemScale = true;
    #endregion

    #region Unity Update Methods
    public void Update()
    {
        if (PlayerManager.Instance != null)
        {
            SetRotation();
            if (canChangeItemScale)
            {
                SetItemScale();
            }
        }
    }
    #endregion

    #region Methods
    private void Rotate(Quaternion rotation)
    {
        if (delayedRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed);
        }
        else
        {
            transform.rotation = rotation;
        }
    }

    private void SetRotation()
    {
        Vector3 rotationChange = PlayerManager.Instance.GetMousePosition() - transform.position;
        float rotationZ = Mathf.Atan2(rotationChange.y, rotationChange.x) * Mathf.Rad2Deg;
        rotationZ += rotationOffset;

        if (!PlayerManager.Instance.GetComponentInChildren<Movement>().IsFacingRight() && rotationOffset != 0)
        {
            rotationZ += 90;
        }

        Quaternion newRot = Quaternion.Euler(0f, 0f, rotationZ);

        Rotate(newRot);
    }

    private void SetItemScale()
    {
        if (PlayerManager.Instance.GetProjectileTrajectory().x >= 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, -1f, 1f);
        }
    }
    #endregion
}
