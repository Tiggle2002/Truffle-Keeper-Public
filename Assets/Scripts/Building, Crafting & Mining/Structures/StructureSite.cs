using System;
using UnityEngine;

public enum StructureSize { Small, Medium, Large }

public class StructureSite : MonoBehaviour, TEventListener<StructureEvent>
{
    #region Fields
    [SerializeField] private Sprite buildResourcesSprite;

    public bool StructurePresent { get { return structurePresent; } }
    public Scaffolding scaffolding { get; private set; }
    private bool structurePresent = false;

    private StructureSize compatibleSize;
    private SpriteRenderer sr;

    #endregion

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        scaffolding = GetComponent<Scaffolding>();
        transform.RotateAroundOrigin();
    }

    #region Methods
    public void BuildStructureScaffolding(StructureData structureData)
    {
        if (!structurePresent)
        {
            Structure structure = Instantiate(structureData.prefab, transform.position + structureData.prefabSpawnOffset, Quaternion.identity, transform).GetComponentInChildren<Structure>();
            scaffolding.enabled = true;
            scaffolding.SetStructure(structure);
            scaffolding.Set(ScaffoldingType.Build);

            structurePresent = true;
            StructureEvent.Trigger(StructureEventType.StructureBuilt);
        }
    }

    public void DestroyStructure()
    {
        GameObject structure = transform.GetChild(1).gameObject;
        Destroy(structure);

        scaffolding.enabled = false;
        structurePresent = false;
        sr.enabled = true;
        sr.sprite = buildResourcesSprite;
    }

    public bool CanHoldStructureOfSize(StructureSize size)
    {
        if (compatibleSize == StructureSize.Large)
        {
            return true;
        }
        else if (compatibleSize == StructureSize.Medium)
        {
            if (size == StructureSize.Large)
            {
                return false;
            }
            return true;
        }

        return size == StructureSize.Small;
    }
    #endregion

    #region Collsion Event Methods
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        transform.SetInteractable();
        this.SetSite();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        transform.RemoveInteractable();
        this.UnsetSite();
    }
    #endregion

    #region TEvent Methods
    public void OnEvent(StructureEvent eventData)
    {
        if (eventData.site != this)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case StructureEventType.Build:
                BuildStructureScaffolding(eventData.data);
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe();
    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }
    #endregion
}
