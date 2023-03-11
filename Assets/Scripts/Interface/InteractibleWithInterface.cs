using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public abstract class InteractibleOnTrigger : MonoBehaviour, TEventListener<PlayerEvent>
{
    private bool canInteract;

    public void OnEvent(PlayerEvent eventData)
    {
        if (!canInteract) return;

        switch (eventData.eventType)
        {
            case PlayerEventType.PlayerInteracted:
                PlayInteraction();
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.Subscribe();
            transform.SetInteractable();
            SetInteractEnabled(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.Unsubscribe();
            transform.RemoveInteractable();
            SetInteractEnabled(false);

            EndInteraction();
        }
    }

    public abstract void PlayInteraction();

    public abstract void EndInteraction();

    public void SetInteractEnabled(bool enabled)
    {
        canInteract = enabled;
    }
}

public class InteractibleWithInterface : InteractibleOnTrigger
{
    public bool canBeOpened { get; set; }
    public BaseInterface correspondingInterface;

    public void Awake()
    {
        correspondingInterface = GetComponentInChildren<BaseInterface>();
    }

    public override void PlayInteraction()
    {
        HUDEvent.Trigger(HUDEventType.InterfaceToggled, correspondingInterface);
    }

    public override void EndInteraction()
    {
        if (correspondingInterface.Open)
        {
            correspondingInterface.CloseInterface();
        }
    }
}