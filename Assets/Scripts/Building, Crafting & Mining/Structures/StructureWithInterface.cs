using System.Collections;
using UnityEngine;

public interface IInteractible
{
    void SetInteractEnabled(bool enabled);
}

public class StructureWithInterface : Structure, IAttachedInterface
{
    public bool canBeOpened { get; set; }
    public BaseInterface correspondingInterface;

    public void Awake()
    {
        correspondingInterface = GetComponentInChildren<BaseInterface>();
    }

    public override bool Upgradeable()
    {
        return structureData.upgradeable;
    }

    public override bool Repairable()
    {
        return false;
    }

    protected override void Initialise()
    {
        
    }

    public void OnEvent(PlayerEvent eventData)
    {
        switch (eventData.eventType)
        {
            case PlayerEventType.PlayerInteracted:
                if (canBeOpened)
                {
                    ToggleInterface();
                }
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.SetInteractable();
            SetInteractEnabled(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.RemoveInteractable();
            SetInteractEnabled(false);
            if (correspondingInterface.Open)
            {
                correspondingInterface.CloseInterface();
            }
        }
    }

    public override void OnEnable()
    {
        this.Subscribe();
    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }

    public void ToggleInterface()
    {
        HUDEvent.Trigger(HUDEventType.InterfaceToggled, correspondingInterface);
    }

    public void SetInteractEnabled(bool enabled)
    {
        canBeOpened = enabled;
    }
}