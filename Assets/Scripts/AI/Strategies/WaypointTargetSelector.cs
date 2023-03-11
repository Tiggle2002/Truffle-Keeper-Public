using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTargetSelector : AITargetSelector
{
    [SerializeField] private AttractantID[] attractants;
    private List<WayPoint> waypoints = new List<WayPoint>();
    private WayPoint currentWayPoint;

    private void Awake()
    {
        AddWayPoints();
    }

    public override bool TargetAvailable() => base.TargetAvailable() && currentWayPoint != null;

    public override void SelectTarget()
    {
        if (waypoints.Count == 0) return ;
        currentWayPoint = SelectWayPoint();
        target = currentWayPoint.transform.position;
    }

    private WayPoint SelectWayPoint()
    {
        WayPoint newWaypoint = SelectValidWayPoint();
        while (newWaypoint == null)
        {
            waypoints.Remove(newWaypoint);
            newWaypoint = SelectValidWayPoint();
        }
        return newWaypoint;
    }

    private WayPoint SelectValidWayPoint()
    {
        WayPoint newWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        return (newWaypoint != null && currentWayPoint != newWaypoint && newWaypoint.transform.WithinLevel()) ? newWaypoint : null;
    }

    private void AddWayPoints()
    {
        WayPoint[] potentialWaypoints = FindObjectsOfType<WayPoint>();

        foreach (var waypoint in potentialWaypoints)
        {
            if (waypoint.IsAttractant(attractants))
            {
                waypoints.Add(waypoint);
            }
        }
    }
}

