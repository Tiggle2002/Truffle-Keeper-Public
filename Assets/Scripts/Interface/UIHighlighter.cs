using MoreMountains.Feedbacks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum HighlightMode { UntilHovered, UntilPressed }
public class UIHighlighter : InterfaceButton
{
    [SerializeField] private MMF_Player highlightFeedbackLoop;

    [SerializeField] private HighlightMode highlightMode;

    private bool highlightPlayed = false;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (highlightMode == HighlightMode.UntilPressed)
        {
            highlightPlayed = true;
            highlightFeedbackLoop?.StopFeedbacks();
            ResetButton();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (highlightMode == HighlightMode.UntilHovered)
        {
            highlightPlayed = true;
            highlightFeedbackLoop?.StopFeedbacks();
            ResetButton();
        }
    }

    private void ResetButton()
    {
        transform.localScale = new(1, 1, 1);
        GetComponent<Image>().color = Color.white;
    }

    public void OnEnable()
    {
        if (!highlightPlayed)
        {
            highlightFeedbackLoop?.PlayFeedbacks();
        }
    }

    public void OnDisable()
    {
        {
            highlightFeedbackLoop?.StopFeedbacks();
        }
    }
}
