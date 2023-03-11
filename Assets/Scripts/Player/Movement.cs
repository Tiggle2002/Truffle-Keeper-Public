using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField, FoldoutGroup("References")]
    private MovementData movementData;

    #region References
    public Rigidbody2D rb { get; private set; }
    private Transform entityTransform;
    [SerializeField, FoldoutGroup("References")] 
    private BoxCollider2D feet;

    private LayerMask blockLayer;
    #endregion

    #region Feedbacks
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player horizontalMovementFeedback;
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player jumpFeedback;
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player landFeedback;
    #endregion

    #region Movement Variables
    private Vector2 velocity;
    private float acceleration { get { return IsGrounded() ? movementData.maxAcceleration : movementData.maxAirAcceleration; } }
    private float maxSpeed;
    private float maxVerticalSpeed;
    private bool isFacingRight = true;
    private int jumpCount = 0;
    #endregion

    public void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>(false);
        entityTransform = transform.parent.GetComponentInParent<Transform>(false);
        blockLayer = LayerMask.GetMask("Ground");

        SetHorizontalSpeed(movementData.maxSpeed);
        SetVerticalSpeed(movementData.maxVerticalSpeed);
    }

    #region Movement Methods
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(feet.bounds.center, feet.bounds.size, 0f, Vector2.down, movementData.groundCheckBoxSize, blockLayer);
    }

    public bool CanJump()
    {
        return jumpCount < movementData.maxJumps || IsGrounded();
    }

    public void ApplyHorizontalVelocity(float directionX)
    {
        rb.ApplyHorizontalVelocity(directionX, acceleration, maxSpeed);

        PlayHorizontalMovementFeedback();
    }

    private void PlayHorizontalMovementFeedback()
    {
        if (horizontalMovementFeedback == null)
        {
            return;
        }
        if (rb.velocity.x != 0 && IsGrounded())
        {
            horizontalMovementFeedback.PlayFeedbacks();
            return;
        }
        else if ((rb.velocity.x == 0 || !IsGrounded()) && horizontalMovementFeedback.IsPlaying)
        {
            horizontalMovementFeedback.StopFeedbacks();
        }
    }

    public void ApplyVerticalVelocity(float directionY)
    {
        rb.ApplyVerticalVelocity(directionY, acceleration, maxVerticalSpeed);
    }

    public void AddHorizontalForce(float forceX)
    {
        if (!isFacingRight)
        {
            forceX *= -1;
        }
        rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Impulse);
    }

    public void SetHorizontalForce(float forceX)
    {
        velocity.x = forceX;
        velocity.x *= isFacingRight ? 1 : -1; 
        rb.velocity = velocity;
    }

    public void JumpIfGrounded()
    {
        if (CanJump())
        {
            PerformJump();
        }
    }

    public void PerformJump()
    {
        ++jumpCount;
        rb.PerformJump(movementData.jumpHeight);
        jumpFeedback?.PlayFeedbacks();
        StartCoroutine(PlayFeedbackOnLand());
    }

    public void UpdateGravityScale(float newScale = -1f)
    {
        if (newScale >= 0)
        {
            rb.gravityScale = newScale;
        }
        else if (IsGrounded())
        {
            rb.gravityScale = movementData.defaultGravityScale;
        }
        else
        {
            rb.gravityScale = rb.Falling() ? movementData.downwardMovementMultiplier : movementData.upwardMovementMultiplier;
        }
    }

    private IEnumerator PlayFeedbackOnLand()
    {
        feet.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        feet.gameObject.SetActive(true);
        yield return new WaitUntil(() => IsGrounded());
        jumpCount = 0;
        landFeedback?.PlayFeedbacks();
    }

    public void PlayLandFeedbacks()
    {
        landFeedback?.PlayFeedbacks();
    }

    public void FlipInDirectionOfMovement(float directionX = -1f)
    {
        if (directionX == -1f)
        {
            directionX = rb.velocity.x;
        }

        isFacingRight = directionX.IsFacingRight(isFacingRight);

        entityTransform.RotateInDirectionFacing(isFacingRight);
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public void SetHorizontalSpeed(float newSpeed)
    {
        if (newSpeed == maxSpeed)
        {
            return;
        }
        maxSpeed = Mathf.Clamp(newSpeed, 0f, 100f);
    }

    public void SetVerticalSpeed(float newSpeed)
    {
        if (newSpeed == maxVerticalSpeed)
        {
            return;
        }
        maxVerticalSpeed = Mathf.Clamp(newSpeed, 0f, 100f);
    }

    public void AddSpeed(float speedToAdd)
    {
        maxSpeed += speedToAdd;
    }

    public void SetDefault()
    {
        maxSpeed = movementData.maxSpeed;
    }
    #endregion

    #region Physics Checks
    public void OnDrawGizmos()
    {
        if (feet)
        DrawCheck();
    }

    private void DrawCheck()
    {
        Color rayColour = IsGrounded() ? Color.green : Color.red;

        Debug.DrawRay(feet.bounds.center + new Vector3(feet.bounds.extents.x, 0), Vector2.down * (feet.bounds.extents.y + movementData.groundCheckBoxSize), rayColour);
        Debug.DrawRay(feet.bounds.center - new Vector3(feet.bounds.extents.x, 0), Vector2.down * (feet.bounds.extents.y + movementData.groundCheckBoxSize), rayColour);
        Debug.DrawRay(feet.bounds.center - new Vector3(feet.bounds.extents.x, feet.bounds.extents.y + movementData.groundCheckBoxSize), Vector2.right * feet.bounds.extents.x * 2, rayColour);
    }
    #endregion
}


