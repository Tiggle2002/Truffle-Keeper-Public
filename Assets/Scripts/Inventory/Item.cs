using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemObject ItemObject { get { return itemObject; } }

    #region Serialised Variables
    [SerializeField, Required] protected ItemObject itemObject;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player useFeedback;

    private bool itemInUse;
    #endregion

    #region References
    protected Animator animator;
    protected SpriteRenderer sr;
    #endregion

    protected Timer cooldown;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        cooldown = new(itemObject.cooldown);
    }

    public virtual void Update()
    {
        cooldown.Countdown();
    }

    public virtual void TryUse()
    {
        if (cooldown.Done())
        {
            Process();
            StartCoroutine(PlayItemAnimation());
            cooldown.ResetCountdown();
        }
    }

    protected virtual IEnumerator PlayItemAnimation()
    {

        ItemEvent.Trigger(ItemEventType.ItemInUse, itemObject);
        sr.enabled = true;
        if (animator != null)
        {
            animator.Play("Use");
        }
        yield return new WaitForSeconds(itemObject.useLength);
        if (animator != null)
        {
            animator.Play("Idle");
        }
        sr.enabled = false;
        ItemEvent.Trigger(ItemEventType.ItemNoLongerInUse);
    }

    protected virtual void Process()
    {
        useFeedback?.PlayFeedbacks();
    }

    public abstract void CancelUse(bool finishCurrent = false);
}
