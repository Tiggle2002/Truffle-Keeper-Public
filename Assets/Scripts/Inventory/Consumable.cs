using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Consumable : Item
{
    [FoldoutGroup("Ability References"), SerializeField] private AbilityData abilityData;
    [FoldoutGroup("Ability References"), SerializeField] private CharacterHealth characterHealth;

    protected override void Awake()
    {
        base.Awake();
        characterHealth = GetComponentInParent<CharacterHealth>();
    }

    protected override IEnumerator PlayItemAnimation()
    {
        ItemEvent.Trigger(ItemEventType.ItemInUse, itemObject);
        sr.enabled = true;
        yield return new WaitForSeconds(itemObject.useLength);
        sr.enabled = false;
        ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
        InventoryEvent.Trigger(InventoryEventType.ItemRemoved, new(itemObject, 1));
    }


    protected override void Process()
    {
        useFeedback?.PlayFeedbacks();
        AbilityEvent.TriggerOnCharacterHealth(AbilityEventType.AbilityTrigger, characterHealth ,abilityData);
    }


    public override void CancelUse(bool finishCurrent = false)
    {
        
    }
}

public struct AbilityEvent
{
    public AbilityEventType eventType;
    public EntityObject entityObject;
    public CharacterHealth characterHealth;
    public AbilityData abilityData;

    public AbilityEvent(AbilityEventType eventType, EntityObject entityObject, CharacterHealth characterHealth, AbilityData abilityData)
    {
        this.eventType = eventType;
        this.entityObject = entityObject;
        this.characterHealth = characterHealth;
        this.abilityData = abilityData;
    }

    static AbilityEvent eventToCall;

    public static void TriggerOnEntitiesOfType(AbilityEventType eventType, EntityObject entityType, AbilityData abilityData)
    {
        eventToCall.eventType = eventType;
        eventToCall.entityObject = entityType;
        eventToCall.abilityData = abilityData;

        EventBus.TriggerEvent(eventToCall);
    }

    public static void TriggerOnCharacterHealth(AbilityEventType eventType, CharacterHealth characterHealth, AbilityData abilityData)
    {
        eventToCall.eventType = eventType;
        eventToCall.characterHealth = characterHealth;
        eventToCall.abilityData = abilityData;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum AbilityEventType { AbilityTrigger }
