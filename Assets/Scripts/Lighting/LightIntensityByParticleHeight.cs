using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityByParticleHeight : MonoBehaviour
{
    [SerializeField] private float activeHeight;
    private float startY = 0f; // Starting Y-axis value
    private float endY = 5f; // Ending Y-axis value
    [SerializeField] private float startIntensity = 1f; // Starting light intensity
    [SerializeField] private float endIntensity = 0f; // Ending light intensity
    [SerializeField] private float startRadius = 2f; // Starting light radius
    [SerializeField] private float endRadius = 10f; // Ending light radius
    [SerializeField] private ParticleSystem particles; // Reference to particle system
    [SerializeField] private Light2D light2d; // Reference to 2D light
    public Color startColor = Color.white;
    public Color endColor = Color.red;

    public void Awake()
    {
        light2d = GetComponent<Light2D>();
        startY = transform.position.y;

    }

    private void Update()
    {
        if (DayNightCycle.Instance.timeOfDay < 0.25f)
        {
            light2d.intensity = 0.01f;
            return;
        }

        endY = transform.position.y + activeHeight;
        // Get the height of the particles
        //float particleHeight = particles.transform.position.y - startY;

        // Calculate the light intensity based on the particle height
        float intensity = Mathf.Lerp(endIntensity, startIntensity, transform.position.y / endY) + Random.Range(0, 0.025f);

        // Calculate the light radius based on the particle height
        float radius = Mathf.Lerp(startRadius, endRadius, transform.position.y / endY);

        Color color = Color.Lerp(startColor, endColor, Mathf.InverseLerp(startY, endY, transform.position.y));
        light2d.color = color;

        // Set the light intensity and radius
        light2d.intensity = intensity;
        light2d.pointLightOuterRadius = radius;
    }
}
