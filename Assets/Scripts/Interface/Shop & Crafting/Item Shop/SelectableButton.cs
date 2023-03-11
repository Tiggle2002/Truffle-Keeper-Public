using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableButton : InterfaceButton, TEventListener<InterfaceEvent>
{
    #region Fields
    [FoldoutGroup("Sprites")]
    [SerializeField] private Sprite selectedSprite;
    [FoldoutGroup("Sprites")]
    [SerializeField] private Sprite normalSprite;

    private Image buttonImage;
    protected BaseInterface correspondingInterface;
    #endregion

    #region Unity Update Methods
    public void Awake()
    {
        InitialiseComponents();
    }
    #endregion

    protected virtual void InitialiseComponents()
    {
        buttonImage = GetComponent<Image>();
        correspondingInterface = GetComponentInParent<BaseInterface>();
    }

    #region Button Methods
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        Select();
    }

    public void Select()
    {
        InterfaceEvent.Trigger(InterfaceEventType.SlotSelected, correspondingInterface, null, this);
    }
    #endregion

    #region  TEvent Methods
    public void OnEvent(InterfaceEvent eventData)
    {
        if (eventData.eventInterface != correspondingInterface)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case InterfaceEventType.SlotSelected:
                if (eventData.shopButton != this)
                {
                    if (buttonImage.sprite == selectedSprite)
                    {
                        buttonImage.sprite = normalSprite;
                    }
                }
                else
                {
                    buttonImage.sprite = selectedSprite;
                }
                break;
            case InterfaceEventType.Opened:
                if (buttonImage.sprite == selectedSprite)
                {
                    buttonImage.sprite = normalSprite;
                }
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe<InterfaceEvent>();
    }

    public void OnDisable()
    {
        this.Unsubscribe<InterfaceEvent>();
    }
    #endregion
}
