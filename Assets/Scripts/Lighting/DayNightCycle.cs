using UnityEngine.Rendering.Universal;
using UnityEngine;
using Sirenix.OdinInspector;

public class DayNightCycle : SingletonMonoBehaviour<DayNightCycle>
{
    public float dayLengthInMinutes = 5f;
    public Color dayColor = Color.white;
    public Color eveningColor = Color.red;
    public Color nightColor = Color.blue;
    public Color morningColor = Color.green;

    [Header("Light Intensity")]
    public float morningIntensity = 1f;
    public float dayIntensity = 1f;
    public float eveningIntensity = 0.5f;
    public float nightIntensity = 0.2f;

    [ReadOnly] public float timeOfDay = 0f;
    private Light2D globalLight;
    private TimeType dayTime = TimeType.Morning;
    private TimeType previousTime = TimeType.Morning;

    private void Start()
    {
        globalLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        UpdateTime();
        UpdateLight();
    }

    private void UpdateTime()
    {
        timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);
        timeOfDay %= 1f;

        // Check if time of day has changed
        TimeType currentTime = GetCurrentTime();
        if (previousTime != currentTime)
        {
            TimeEvent.Trigger(currentTime);
            previousTime = currentTime;
        }
    }

    private void UpdateLight()
    {
        if (timeOfDay <= 0.25f)
        {
            dayTime = TimeType.Morning;
            globalLight.color = Color.Lerp(morningColor, dayColor, timeOfDay * 4f);
            globalLight.intensity = Mathf.Lerp(morningIntensity, dayIntensity, timeOfDay * 4f);
        }
        else if (timeOfDay <= 0.5f)
        {
            globalLight.color = Color.Lerp(dayColor, eveningColor, (timeOfDay - 0.25f) * 4f);
            globalLight.intensity = Mathf.Lerp(dayIntensity, eveningIntensity, (timeOfDay - 0.25f) * 4f);
        }
        else if (timeOfDay <= 0.75f)
        {
            dayTime = TimeType.Evening;
            globalLight.color = Color.Lerp(eveningColor, nightColor, (timeOfDay - 0.5f) * 4f);
            globalLight.intensity = Mathf.Lerp(eveningIntensity, nightIntensity, (timeOfDay - 0.5f) * 4f);
        }
        else
        {
            globalLight.color = Color.Lerp(nightColor, morningColor, (timeOfDay - 0.75f) * 4f);
            globalLight.intensity = Mathf.Lerp(nightIntensity, morningIntensity, (timeOfDay - 0.75f) * 4f);
        }
    }

    private TimeType GetCurrentTime()
    {
        if (timeOfDay <= 0.25f)
        {
            return TimeType.Morning;
        }
        else if (timeOfDay <= 0.5f)
        {
            return TimeType.Day;
        }
        else if (timeOfDay <= 0.75f)
        {
            return TimeType.Evening;
        }
        else
        {
            return TimeType.Night;
        }
    }
}

public struct TimeEvent
{
    public TimeType eventType;

    public TimeEvent(TimeType eventType)
    {
        this.eventType = eventType;
    }

    public static void Trigger(TimeType eventType)
    {
        TimeEvent eventToCall = new TimeEvent(eventType);
        EventBus.TriggerEvent(eventToCall);
    }
}

public enum TimeType { Morning, Day,Evening, Night }



