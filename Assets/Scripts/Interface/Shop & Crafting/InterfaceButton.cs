using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Tools;
using UnityEngine.Events;
using System;
using Sirenix.OdinInspector;

public class InterfaceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Feedbacks
    [FoldoutGroup("Button Feedbacks")]
    [SerializeField] protected MMF_Player pointerEnterFeedback;
    [FoldoutGroup("Button Feedbacks")]
    [SerializeField] protected MMF_Player pointerExitFeedback;
    [FoldoutGroup("Button Feedbacks")]
    [SerializeField] protected MMF_Player pointerClickFeedback;
    #endregion

    #region Pointer Methods
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        pointerClickFeedback?.PlayFeedbacks();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterFeedback?.StopFeedbacks();
        pointerExitFeedback?.StopFeedbacks(true);
        pointerEnterFeedback?.PlayFeedbacks();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        pointerExitFeedback?.StopFeedbacks();
        pointerEnterFeedback?.StopFeedbacks(true);
        pointerExitFeedback?.PlayFeedbacks();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {

    }
    #endregion
}
