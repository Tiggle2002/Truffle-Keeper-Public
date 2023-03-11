using System.Collections;
using UnityEngine;

public class PlayerMovement : Movement
{
    private float canJumpTime;

    public bool WithinCoyoteTime()
    {
        return canJumpTime <= 0f;
    }

    public void SetHorizontalForceAccordingToInput(float forceX, float inputX)
    {
        Vector2 velocity;
        velocity.x = forceX;
        velocity.x *= inputX > 0 ? 1 : -1;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }
}
