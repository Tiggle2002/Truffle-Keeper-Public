using System;
using UnityEngine;
using System.Collections;

public class Timer
{
    #region Fields
    private float cooldownLength;
    private float currentTime;
    private bool timerActionInvoked = false;
    protected event Action onTimerDone;
    #endregion

    public Timer(float cooldownLength, bool resetOnInitialisation = false)
    {
        this.cooldownLength = cooldownLength;

        if (resetOnInitialisation)
        {
            ResetCountdown();
        }
    }

    public Timer(float cooldownLength, Action onTimerDone, bool resetOnInitialisation = false)
    {
        this.cooldownLength = cooldownLength;
        this.onTimerDone = onTimerDone;

        if (resetOnInitialisation)
        {
            ResetCountdown();
        }
    }

    public void Countdown()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (!timerActionInvoked)
        {
            timerActionInvoked = true;
            onTimerDone?.Invoke();
        }
    }

    public void ResetCountdown()
    {
        currentTime = cooldownLength;
        timerActionInvoked = false;
    }

    public void SetTimerLength(float newCooldownLength)
    {
        cooldownLength = newCooldownLength;
    }

    public void SetTimerDone() => currentTime = 0;

    public float CurrentTime() => currentTime;

    public bool Done() => currentTime <= 0;
}

public static class TimerUtilites
{
    public static IEnumerator Time(float time, Action actionToPerform)
    {
        yield return new WaitForSeconds(time);
        actionToPerform.Invoke();
    }
}
