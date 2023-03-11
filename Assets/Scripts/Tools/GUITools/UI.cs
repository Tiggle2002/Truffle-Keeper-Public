
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UI
{
    public static IEnumerator FadeCanvasGroup(CanvasGroup groupToFade, float targetAlpha, float duration)
    {
        float currentAlpha = groupToFade.alpha;

        for (float t = 0; t < 1; t+= Time.deltaTime / duration)
        {
            float lerpedAlpha = Mathf.Lerp(currentAlpha, targetAlpha, t);

            groupToFade.alpha = lerpedAlpha;

            yield return null;
        }

        groupToFade.alpha = targetAlpha;
    }

    public static IEnumerator LerpFilledImage(Image image,float newValue, float lerpValue)
    {
        if (image.fillAmount > newValue)
        {
            while (image.fillAmount > newValue)
            {
                image.fillAmount = Mathf.MoveTowards(image.fillAmount, newValue, lerpValue);
                yield return null;
            }
        }
        else
        {
            while (newValue > image.fillAmount)
            {
                image.fillAmount = Mathf.MoveTowards(image.fillAmount, newValue, lerpValue);
                yield return null;
            }
        }
        image.fillAmount = newValue;
    }

    public static void ToggleInterface(bool interfaceOpen, Action closeInterface, Action openInterface)
    {
        if (interfaceOpen)
        {
            closeInterface?.Invoke();
        }
        else
        {
            openInterface?.Invoke();
        }
    }
}

