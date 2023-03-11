using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DismantleStructureButton : InterfaceButton
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        Dismatle();
    }

    public void Dismatle()
    {
        StructureEvent.Trigger(StructureEventType.Demolish);
    }
}
