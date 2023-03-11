using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StructureSlot : SelectableButton
{
    [FoldoutGroup("Button Feedbacks")]
    [SerializeField] private MMF_Player selectionFailedFeedback;
    protected StructureData structureData;

    [FoldoutGroup("References"), SerializeField]
    protected Image structureImage;
    protected bool unlocked;

    public void Clear()
    {
        SetStructureSprite(null);
        structureData = null;
    }

    public void SetUnlocked(bool unlocked)
    {
        this.unlocked = unlocked;
        structureImage.color = unlocked ? Color.white : Color.black;
    }

    public void SetStructureData(StructureData structureData)
    {
        this.structureData = structureData;
        SetStructureSprite(structureData.sprite);
    }

    private void SetStructureSprite(Sprite sprite)
    {
        structureImage.sprite = sprite;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (unlocked)
        base.OnPointerClick(eventData);

        SelectCraftableItem();

        PlaySelectionFeedback();
    }

    private void PlaySelectionFeedback()
    {
        if (unlocked) return;

        pointerClickFeedback?.StopFeedbacks();
        selectionFailedFeedback?.PlayFeedbacks();
    }

    private void SelectCraftableItem()
    {
        if (unlocked)
        {
            StructureEvent.Trigger(StructureEventType.StructureSelected, structureData);
        }
    }
}
