using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class CollisionTriggeredTextSpawner : MonoBehaviour
{
    [Title("Text"), HideLabel, MultiLineProperty(3), SerializeField] protected string textToDisplay;

    [SerializeField] protected Sprite imageToDisplay;

    [PropertyRange(0.5, 5f), SerializeField] protected float textActiveTime;

    [SerializeField] protected Vector2 offset;

    [SerializeField] protected bool showOnce = true;

    [SerializeField] protected bool shown = false;

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (showOnce)
        {
            if (shown)
            {
                return;
            }
            shown = true;
        }
        DisplayText();
    }
  


    public virtual void DisplayText()
    {
        Vector3 textPos = transform.position + new Vector3(offset.x, offset.y);
        StartCoroutine(textPos.PresentTextMesh(textToDisplay, textActiveTime, imageToDisplay));
    }
}
