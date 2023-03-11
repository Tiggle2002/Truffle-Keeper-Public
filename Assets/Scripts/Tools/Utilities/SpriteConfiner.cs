using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteConfiner : Confiner
{
    private SpriteRenderer spriteRenderer;

    protected override void InitialiseComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void UpdateTargetBounds()
    {
        boundsToConfine = spriteRenderer.bounds;
    }
}

