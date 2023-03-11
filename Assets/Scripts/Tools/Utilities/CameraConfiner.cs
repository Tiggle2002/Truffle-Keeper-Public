using System.Collections;
using UnityEngine;

public class CameraConfiner : Confiner
{
    private Camera cam;

    protected override void InitialiseComponents()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    protected override void UpdateTargetBounds()
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3 cameraPosition = cam.transform.position;
        Vector3 cameraSize = new Vector3(width, height, -2.200809f);

        boundsToConfine = new Bounds(cameraPosition, cameraSize);
    }
}