using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialMonument : MonoBehaviour
{
    private bool waveTriggered = false;

    public MMF_Player waveStartedFeedback;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !waveTriggered)
        {
            waveTriggered = true;
            StartCoroutine(TriggerTutorialWave());
        }
    }

    private IEnumerator TriggerTutorialWave()
    {
        Vector3 tutorialText = new(transform.position.x, transform.position.y + 10f);
        StartCoroutine(tutorialText.PresentTextMesh("Defend", 2.5f));
        yield return new WaitForSeconds(1f);
        WaveEvent.Trigger(WaveEventType.TriggerWaveStart);
    }
}
