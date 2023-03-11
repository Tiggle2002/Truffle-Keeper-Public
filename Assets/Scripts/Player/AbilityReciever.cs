using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityReciever : MonoBehaviour, TEventListener<AbilityEvent>
{
    private Movement movement;
    private CharacterHealth health;
    public float miningSpeed { get; private set; } = 1;
    private List<AbilityData> abilitiesApplied = new();

    public void Awake()
    {
        movement = transform.Find("Player Movement").GetComponent<Movement>();
        health = GetComponent<CharacterHealth>();
    }

    public void TriggerAbility(AbilityData abilityData)
    {
        if (abilityData.healthToAdd > 0)
        {
            health.Heal(abilityData.healthToAdd);
        }
        if (abilitiesApplied.Contains(abilityData)) return;
        
        if (abilityData.speedBoostX > 0)
        {
            movement.AddSpeed(abilityData.speedBoostX);
            StartCoroutine(RemoveAbilityAfterTime(abilityData));
        }
        if (abilityData.miningSpeedPercentageToAdd > 0)
        {
            miningSpeed += abilityData.miningSpeedPercentageToAdd;
            StartCoroutine(RemoveMiningAbilityAfterTime(abilityData));
        }
        abilitiesApplied.Add(abilityData);
    }

    public IEnumerator RemoveAbilityAfterTime(AbilityData abilityData)
    {
        yield return new WaitForSeconds(abilityData.abilityLength);
        movement.AddSpeed(-abilityData.speedBoostX);
        abilitiesApplied.Remove(abilityData);
    }

    public IEnumerator RemoveMiningAbilityAfterTime(AbilityData abilityData)
    {
        yield return new WaitForSeconds(abilityData.abilityLength);
        miningSpeed -= abilityData.miningSpeedPercentageToAdd;
        abilitiesApplied.Remove(abilityData);
    }

    public void OnEvent(AbilityEvent eventData)
    {
        switch (eventData.eventType)
        {
            case AbilityEventType.AbilityTrigger:
                TriggerAbility(eventData.abilityData);
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
}
