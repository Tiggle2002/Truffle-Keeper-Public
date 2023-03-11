using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ShopPageButton : SelectableButton
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        SelectPage();
    }

    protected abstract void SelectPage();
}