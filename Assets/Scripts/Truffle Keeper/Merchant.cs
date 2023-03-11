using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Merchant : InterfaceButton, TEventListener<WaveEvent>, TEventListener<PlayerEvent>
{
    [FoldoutGroup("References")]
    [SerializeField] protected ItemShop correspondingShop;

    [FoldoutGroup("Properties")]
    [SerializeField] private bool activeAtStart;

    [FoldoutGroup("Merchant Feedback"), SerializeField]
    private MMF_Player spawnFeedback;

    [FoldoutGroup("Merchant Feedback"), SerializeField]
    private MMF_Player despawnFeedback;

    private bool shoppingAllowed = false;
    private Image merchantImage;
    private SpriteRenderer sr;
    private Animator animator;
    private bool registerCollisions = true;

    protected void Awake()
    {
        merchantImage = GetComponent<Image>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!activeAtStart)
        {
            shoppingAllowed = false;
            registerCollisions = false;
        }
        else
        {
            StartCoroutine(Spawn());
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1.5f);

        sr.enabled = true;
        spawnFeedback?.PlayFeedbacks();
        animator.Play("Spawn");

        yield return new WaitForSeconds(2f);
        animator.Play("Idle");
    }

    private void OpenShop()
    {
        StartCoroutine(Spawn());
        registerCollisions = true;
    }

    private IEnumerator CloseShop()
    {
        shoppingAllowed = false;
        registerCollisions = false;
        yield return new WaitForSeconds(0.5f);
        despawnFeedback?.PlayFeedbacks();
        animator.Play("Despawn");
        yield return new WaitForSeconds(2f);

        if (merchantImage)
        {
            merchantImage.enabled = false;
        }
        if (sr)
        {
            sr.enabled = false;
        }
    }
 
    #region Events
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.button == PointerEventData.InputButton.Right && shoppingAllowed)
        {
            HUDEvent.Trigger(HUDEventType.InterfaceToggled, correspondingShop);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
    #endregion


    public void OnEvent(WaveEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WaveEventType.WaveBegun:
                StopAllCoroutines();
                StartCoroutine(CloseShop());
                break;
            case WaveEventType.WaveDefeated:
                if (Random.value < 0.5f)
                {
                    OpenShop();
                }
                break;
        }
    }

    public void OnEvent(PlayerEvent eventData)
    {
        switch (eventData.eventType)
        {
            case PlayerEventType.PlayerInteracted:
                if (shoppingAllowed)
                {
                    HUDEvent.Trigger(HUDEventType.InterfaceToggled, correspondingShop);
                }
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<WaveEvent>();
        this.Subscribe<PlayerEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<WaveEvent>();
        this.Unsubscribe<PlayerEvent>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && registerCollisions)
        {
            transform.SetInteractable();

            shoppingAllowed = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && registerCollisions)
        {
            transform.RemoveInteractable();
            shoppingAllowed = false;
            correspondingShop?.CloseInterface();
        }
    }
}
