using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class NightLight : MonoBehaviour
{
    [FoldoutGroup("Light Settings")]
    public float morningIntensity = 1f;
    [FoldoutGroup("Light Settings")]
    public float dayIntensity = 1f;
    [FoldoutGroup("Light Settings")]
    public float eveningIntensity = 0.5f;
    [FoldoutGroup("Light Settings")]
    public float nightIntensity = 0.2f;

    private Light2D light2D;

    public void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    public void Update()
    {
        UpdateLight();
    }

    private void UpdateLight()
    {
        if (DayNightCycle.Instance.timeOfDay <= 0.25f)
        {
            light2D.intensity = Mathf.Lerp(morningIntensity, dayIntensity, DayNightCycle.Instance.timeOfDay * 4f);
        }
        else if (DayNightCycle.Instance.timeOfDay <= 0.5f)
        {
            light2D.intensity = Mathf.Lerp(dayIntensity, eveningIntensity, (DayNightCycle.Instance.timeOfDay - 0.25f) * 4f);
        }
        else if (DayNightCycle.Instance.timeOfDay <= 0.75f)
        {
            light2D.intensity = Mathf.Lerp(eveningIntensity, nightIntensity, (DayNightCycle.Instance.timeOfDay - 0.5f) * 4f);
        }
        else
        {
            light2D.intensity = Mathf.Lerp(nightIntensity, morningIntensity, (DayNightCycle.Instance.timeOfDay - 0.75f) * 4f);
        }
    }
}