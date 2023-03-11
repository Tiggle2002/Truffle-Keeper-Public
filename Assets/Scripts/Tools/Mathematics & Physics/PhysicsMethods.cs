using Sirenix.Utilities;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PhysicsMethods
{
    public static void Knockback(Rigidbody2D rb, Vector2 force, Vector3 instigatorPos)
    {
        if (rb == null)
        {
            return;
        }

        Vector2 forceToApply = force;

        forceToApply.x *= Mathf.Sign(rb.transform.position.x - instigatorPos.x);

        if (rb.velocity.y > 0)
        {
            rb.velocity = Vector2.zero;
        }
        rb.AddForce(forceToApply, ForceMode2D.Impulse);
    }
}

