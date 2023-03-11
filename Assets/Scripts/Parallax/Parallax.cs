using System;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region References
    public List<ParallaxElement> layers;
    private Transform cameraTransform;
    #endregion

    #region Private Variables
    private Vector3 speed;
    private Vector3 deltaCameraPosition;
    private Vector3 cameraDistanceDifference;
    #endregion

    public void Awake()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            deltaCameraPosition = cameraTransform.position;
        }
    }

    public void FixedUpdate()
    {
        ApplyParallax();
    }

    private void ApplyParallax()
    {
        cameraDistanceDifference = cameraTransform.position - deltaCameraPosition;

        for (int i = 0; i < layers.Count; i++)
        {
            speed.x = layers[i].horizontalSpeed;
            speed.y = layers[i].verticalSpeed;
            layers[i].parallaxObject.transform.position += Vector3.Scale(speed, cameraDistanceDifference);
        }

        deltaCameraPosition = cameraTransform.position;
    }
}

[Serializable]
public struct ParallaxElement
{
    public GameObject parallaxObject;
    [Range(0f, 0.5f)]
    public float horizontalSpeed;
    [Range(0f, 0.5f)]
    public float verticalSpeed;
}