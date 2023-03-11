using UnityEngine;

public static class PhysicsMovement
{
    public static void HaltHorizontalMovementImmediate(this Rigidbody2D rb)
    {
        Vector2 velocity = rb.velocity;
        velocity.x = 0f;
        rb.velocity = velocity;
    }

    public static void HaltMovementImmediate(this Rigidbody2D rb)
    {
        rb.velocity = Vector2.zero;
    }

    public static bool Falling(this Rigidbody2D rb)
    {
        return rb.velocity.y < 0;
    }

    public static void ApplyHorizontalVelocity(this Rigidbody2D rb, float directionX, float acceleration, float maxSpeed)
    {
        Vector2 velocity = rb.velocity;
        
        float maxSpeedChange = acceleration * Time.deltaTime;

        float desiredVeloctiy = directionX * maxSpeed;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVeloctiy, maxSpeedChange);

        rb.velocity = velocity;
    }

    public static void ApplyVerticalVelocity(this Rigidbody2D rb, float directionY, float acceleration, float maxSpeed)
    {
        Vector2 velocity = rb.velocity;

        float maxSpeedChange = acceleration * Time.deltaTime;

        float desiredVelocity = directionY * maxSpeed;

        velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity, maxSpeedChange);

        rb.velocity = velocity;
    }

    public static void PerformJump(this Rigidbody2D rb, float height)
    {
        float verticalSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * height);

        if (rb.velocity.y > 0f)
        {
            verticalSpeed = Mathf.Max(verticalSpeed - rb.velocity.y, 0f);
        }
        else if (rb.velocity.y < 0f)
        {
            verticalSpeed += Mathf.Abs(rb.velocity.y);
        }

        Vector2 airborneVelocity = rb.velocity;
        airborneVelocity.y += verticalSpeed;

        rb.velocity = airborneVelocity;
    }

    public static bool IsFacingRight(this float inputDirection, bool isFacingRight)
    {
        return (isFacingRight && inputDirection >= 0) || (!isFacingRight && inputDirection > 0);
    }

    public static void RotateInDirectionFacing(this Transform transform, bool isFacingRight)
    {
        float rotY = isFacingRight.FlipYAxisIfFalse();
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
    }

    public static float FlipYAxisIfFalse(this bool boolean)
    {
        return boolean ? 0f : -180f;
    }
}


