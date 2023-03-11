using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum MeleeAnimationType { Swing, Thrust, Stab }
[RequireComponent(typeof(MeleeAbility))]
public  class MeleeWeapon : Item, TEventListener<GameEvent>
{
    #region Variables
    [SerializeField] private float comboWindowTime;
    [SerializeField] private float timeBetweenCombos;

    private int comboIndex = 0;
    private bool takingComboInput = true;
    private bool weaponInUse = false;
    private bool attackQueued = false;
    private Coroutine animationCoroutine;
    private Coroutine cancelCoroutine;
    #endregion

    #region References
    private MeleeAbility meleeDamage;

    #endregion

    protected override void Awake()
    {
        base.Awake();

       meleeDamage = GetComponent<MeleeAbility>();
       meleeDamage.SetDamage(itemObject.damage);

       meleeDamage.enabled = false;
       takingComboInput = true;
    }

    protected override void Process()
    {
        base.Process();

        StartCoroutine(EnableMeleeDamage());
        PlayItemAnimation();
    }

    public override void TryUse()
    {
        if (!cooldown.Done()) return;

        if (itemObject.comboWeapon)
        {
            TryPerformCombo();
        }
        else if (!itemObject.comboWeapon)
        {
            base.TryUse();
        }
    }

    private void TryPerformCombo()
    {
        if (attackQueued)
        {
            return;
        }

        if (!weaponInUse)
        {
            weaponInUse = true;
            ComboAttack();
        }
        else if (takingComboInput)
        {
            attackQueued = true;
        }
    }

    private void ComboAttack()
    {
        StartCoroutine(ProcessInputTime());
        StartCoroutine(EnableMeleeDamage());
        animationCoroutine = StartCoroutine(PlayItemAnimation());
    }

    private IEnumerator ProcessInputTime()
    {
        takingComboInput = true;
        yield return new WaitForSeconds(itemObject.ComboUseLength(comboIndex) - 0.01f);
        takingComboInput = false;
    }

    protected IEnumerator EnableMeleeDamage()
    {
        float attackDelay = itemObject.attackDelay;
        Vector2 knockback = itemObject.combos[comboIndex].knockback;
        if (knockback != Vector2.zero)
        {
            meleeDamage.SetKnockback(knockback);
        }

        if (itemObject.comboWeapon)
        {
            meleeDamage.SetDamage(itemObject.combos[comboIndex].damage);
            meleeDamage.ChangeCollider(itemObject.combos[comboIndex].colliderData);
            attackDelay = itemObject.combos[comboIndex].attackDelay;
        }

        meleeDamage.enabled = true;
        if (itemObject.comboWeapon)
        {
            SetSound();
        }
        useFeedback?.PlayFeedbacks();
        yield return StartCoroutine(meleeDamage?.PerformAbilityCoroutine(attackDelay));
        meleeDamage.StopAllCoroutines();
        meleeDamage.enabled = false;
    }

    protected override IEnumerator PlayItemAnimation()
    {
        ItemEvent.Trigger(ItemEventType.ItemInUse, itemObject);

        EnableWeaponVisuals();
        TriggerWeaponEvent();

        yield return new WaitForSeconds(itemObject.ComboUseLength(comboIndex) + 0.05f);
        ItemEvent.Trigger(ItemEventType.HaltMovement);


        if (!attackQueued || comboIndex == itemObject.combos.Length - 1)
        {
            if (comboIndex >= 0)
            {
                cooldown.ResetCountdown();
            }
            weaponInUse = false;
            attackQueued = false;

            comboIndex = 0;
        }
        else if (attackQueued)
        {
            yield return new WaitForSeconds(timeBetweenCombos);

            IncrementCombo();
            attackQueued = false;
            ComboAttack();
            yield break;
        }

        DisableWeaponVisuals();
        ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
    }

    private void IncrementCombo()
    {
        comboIndex++;

        if (comboIndex >= itemObject.combos.Length)
        {
            comboIndex = 0;
        }
    }

    private void EnableWeaponVisuals()
    {
        sr.enabled = true;
        animator.speed = itemObject.AnimatorSpeed();

        if (itemObject.comboWeapon)
        {
            animator.Play(itemObject.combos[comboIndex].Animation, -1, 0);
        }
        else
        {
            animator.Play("UseOne");
        }
    }

    private void DisableWeaponVisuals()
    {
        sr.enabled = false;
    }

    private void TriggerWeaponEvent()
    {
        if (itemObject.combos[comboIndex].animationType == MeleeAnimationType.Swing)
        {
            if (itemObject.combos[comboIndex].animationIndex == 0)
            {
                ItemEvent.Trigger(ItemEventType.FirstSwing);
            }
            else if (itemObject.combos[comboIndex].animationIndex == 1)
            {
                ItemEvent.Trigger(ItemEventType.SecondSwing);
            }
        }
        else if (itemObject.combos[comboIndex].animationType == MeleeAnimationType.Thrust)
        {
            if (itemObject.combos[comboIndex].animationIndex == 0)
            {
                ItemEvent.Trigger(ItemEventType.FirstThrust);
            }
            else if (itemObject.combos[comboIndex].animationIndex == 1)
            {
                ItemEvent.Trigger(ItemEventType.SecondThrust);
            }
        }
        else if (itemObject.combos[comboIndex].animationType == MeleeAnimationType.Stab)
        {
            if (itemObject.combos[comboIndex].animationIndex == 0)
            {
                ItemEvent.Trigger(ItemEventType.FirstStab);
            }
            else if (itemObject.combos[comboIndex].animationIndex == 1)
            {
                ItemEvent.Trigger(ItemEventType.SecondStab);
            }
        }
    }

    public override void CancelUse(bool finishCurrent = false)
    {
        if (animationCoroutine != null && finishCurrent)
        {
            cancelCoroutine = StartCoroutine(CancelAfterUse());
            return;
        }
        StopAllCoroutines();
        meleeDamage.StopAllCoroutines();
        DisableWeaponVisuals();
        attackQueued = false;
        weaponInUse = false;
        meleeDamage.enabled = false;
        takingComboInput = false;
        comboIndex = 0;
        useFeedback?.StopFeedbacks();

        ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
    }

    private IEnumerator CancelAfterUse()
    {
        comboIndex = 0;
        attackQueued = false;
        if (animationCoroutine != null)
        {
            yield return animationCoroutine;
        }
        CancelUse();
        useFeedback?.StopFeedbacks();
    }

    private void SetSound()
    {
        if (itemObject.combos[comboIndex].audioClip != null)
        {
            useFeedback.GetFeedbackOfType<MMF_MMSoundManagerSound>().Sfx = itemObject.combos[comboIndex].audioClip;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<GameEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<GameEvent>();
        CancelUse();
    }

    public void OnEvent(GameEvent eventData)
    {
        switch (eventData.eventType)
        {
            case GameEventType.LevelFailed:
                CancelUse();
                break;
        }
    }
}


