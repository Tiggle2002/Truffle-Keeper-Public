using System;
using System.Collections;
using UnityEngine;

public class SentryStructure : DynamicStructure
{
    [SerializeField] private SpriteRenderer sentrySR;

    protected override void InitialiseComponents()
    {
        base.InitialiseComponents();
        if (disableOnStart)
        {
            sentrySR.enabled = false;
        }
    }

    protected override void SetEnabled(bool enabled)
    {
        base.SetEnabled(enabled);
        sentrySR.enabled = enabled;
    }
}
