using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

public class Scaffolding : DestructibleObject
{
    [SerializeField] private Sprite scaffoldingSprite;
    [SerializeField] private bool setOnAwake;
    [SerializeField, ShowIf("setOnAwake"), Required] private Structure structure;
    private Structure scaffoldingStructure;
    private ScaffoldingType scaffoldingType;
    private SpriteRenderer sr;


    protected override void InitialiseComponents()
    {
        base.InitialiseComponents();
        sr = GetComponent<SpriteRenderer>();
        scaffoldingStructure = GetComponent<DynamicStructure>();
        if (setOnAwake)
        {
            Set(ScaffoldingType.Build);
        }
    }

    public void SetStructure(Structure structure)
    {
        this.structure = structure;
    }

    public void Set(ScaffoldingType type)
    {
        scaffoldingStructure.enabled = true;
        structure.SetState(StructureState.Constructing);
        sr.enabled = true;
        sr.sprite = scaffoldingSprite;
        scaffoldingType = type;
        Respawn();
    }

    protected override IEnumerator Kill()
    {
        if (!spawnAfterDeath) SpawnLoot();

        yield return deathFeedback?.PlayFeedbacksCoroutine(this.transform.position, 1f, false);

        if (spawnAfterDeath) SpawnLoot();

        RemoveScaffolding();
        sr.enabled = false;

        if (disableOnDeath)
        {
            gameObject.SetActive(false);
        }
        else if (destroyOnDeath)
        {
            Destroy(this);
        }
    }

    private void RemoveScaffolding()
    {
        switch (scaffoldingType)
        {
            case ScaffoldingType.Build:
                structure.Build();
                break;
            case ScaffoldingType.Upgrade:
                structure.Upgrade();
                break;
            case ScaffoldingType.Repair:
                structure.Repair();
                break;
        }
    }
}

public enum ScaffoldingType { Build, Upgrade, Repair }
