using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;
using Cinemachine;
using System;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;

public interface ILevelExtender
{
    ExtensionDirection direction { get; set; }

    void Extend();

    float ExtensionPointX();
}


/// <summary>
/// Extends the Level Collider - Must Be Attached to The GameObject Containing The Level Collider
/// </summary>
public class LevelBounder : SingletonMonoBehaviour<LevelBounder>
{
    [FoldoutGroup("Extension Settings"), Range(0.000005f, 0.01f), SerializeField] private float cameraPanSpeed;

    private float currentCoordinateX;

    public PolygonCollider2D levelBoundsCollider { get; private set; }

    public CinemachineConfiner2D cameraConfiner;

    public MMF_Player worldExtendedFeedback;

    public override void Awake()
    {
        base.Awake();
        levelBoundsCollider = GetComponent<PolygonCollider2D>();
    }

    public void SmoothlyIncreaseWorldBounds(ILevelExtender extender)
    {
        Vector2[] newPoints = new Vector2[levelBoundsCollider.points.Length];
        Array.Copy(levelBoundsCollider.points, newPoints, levelBoundsCollider.points.Length);
        float pointX = extender.ExtensionPointX();

        if (pointX < 0)
        {
            StartCoroutine(TransitionCameraToPointLeft(pointX, newPoints));
        }
        else
        {
            StartCoroutine(TransitionCameraToPointRight(pointX, newPoints));
        }
        worldExtendedFeedback?.PlayFeedbacks();
    }

    private IEnumerator TransitionCameraToPointLeft(float pointX, Vector2[] newPoints)
    {
        float transitionTime = 2.5f;
        while (transitionTime > 0)
        {
            newPoints[0].x = Mathf.Lerp(levelBoundsCollider.points[0].x, pointX, cameraPanSpeed);
            newPoints[1].x = Mathf.Lerp(levelBoundsCollider.points[1].x, pointX, cameraPanSpeed);
            levelBoundsCollider.SetPath(0, newPoints);
            cameraConfiner.InvalidateCache();
            WorldEvent.Trigger(WorldEventType.BoundsChanged);
            transitionTime -= Time.deltaTime;
            yield return null;
        }
        newPoints[0].x = pointX;
        newPoints[1].x = pointX;
        levelBoundsCollider.SetPath(0, newPoints);
        cameraConfiner.InvalidateCache();
        WorldEvent.Trigger(WorldEventType.BoundsChanged);
    }

    private IEnumerator TransitionCameraToPointRight(float pointX, Vector2[] newPoints)
    {
        float transitionTime = 2.5f;
        while (transitionTime > 0)
        {
            newPoints[2].x = Mathf.Lerp(levelBoundsCollider.points[2].x, pointX, cameraPanSpeed);
            newPoints[3].x = Mathf.Lerp(levelBoundsCollider.points[3].x, pointX, cameraPanSpeed);
            levelBoundsCollider.SetPath(0, newPoints);
            cameraConfiner.InvalidateCache();
            WorldEvent.Trigger(WorldEventType.BoundsChanged);
            transitionTime -= Time.deltaTime;
            yield return null;
        }
        newPoints[2].x = pointX;
        newPoints[3].x = pointX;
        levelBoundsCollider.SetPath(0, newPoints);
        cameraConfiner.InvalidateCache();
        WorldEvent.Trigger(WorldEventType.BoundsChanged);
    }
}

public static class LevelExtensions
{
    public static bool WithinLevel(this Transform transform) => LevelBounder.Instance.levelBoundsCollider.OverlapPoint(transform.position);
}
