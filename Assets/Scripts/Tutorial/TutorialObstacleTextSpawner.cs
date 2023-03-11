using SurvivalElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { Use, Interact}
public class TutorialObstacleTextSpawner : CollisionTriggeredTextSpawner
{
    public ItemObject requiredItem;
    public InputType requiredInput;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (requiredItem != null &&  PlayerManager.Instance.inventory.FindQuantityOfItem(requiredItem) != 1)
        {
            return;
        }
        base.OnTriggerEnter2D(collision);
    }

    public override void DisplayText()
    {
        Vector3 textPos = transform.position + new Vector3(offset.x, offset.y);
        StartCoroutine(textPos.PresentTextMeshCoroutine(textToDisplay, PlayerClicked, imageToDisplay));
    }

    public bool PlayerClicked()
    {
        if (requiredItem != null && PlayerManager.Instance.inventory.FindQuantityOfItem(requiredItem) != 1)
        {
            return false;
        }

        if (requiredInput == InputType.Use)
        {
            return PlayerManager.Instance.useAction.IsPressed();
        }
        else
        {
            return PlayerManager.Instance.interactAction.IsPressed();
        }
    }
}
