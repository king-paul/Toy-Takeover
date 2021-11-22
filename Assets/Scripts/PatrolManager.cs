using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolRoute
{
    public PatrolRoute(Transform[] waypoints, bool twoWay)
    {
        this.waypoints = waypoints;
        this.twoWay = twoWay;
    }

    public Transform[] waypoints;
    public bool twoWay;
    public bool randomizeOrder;
}

public class PatrolManager : MonoBehaviour
{
    public PatrolRoute[] patrolRoutes;

    public PatrolRoute GetPatrolRoute()
    {
        //return patrolRoutes[1];
        /*
        int random = Random.Range(0, patrolRoutes.Length);
        if (random == patrolRoutes.Length)
            random = patrolRoutes.Length - 1;*/

        int random = 2;

        if (patrolRoutes[random].randomizeOrder)
        {
            return new PatrolRoute(GetRandomWaypoints(patrolRoutes[random].waypoints),
                                   patrolRoutes[random].twoWay);
        }

        return patrolRoutes[random];
    }

    // Shuffle the waypoints so that the order is random
    private Transform[] GetRandomWaypoints(Transform[] waypoints)
    {
        List<Transform> waypointList = new List<Transform>();
        Transform[] newRoute = new Transform[waypoints.Length];
        int waypointsAdded = 0;
        bool alreadyInRoute = false;

        foreach(Transform waypoint in waypoints)
        {
            waypointList.Add(waypoint);
        }

        // selects random waypoints until the route is full
        while (waypointsAdded < waypoints.Length)
        {
            int chosenNumber = Random.Range(0, waypointList.Count-1);

            // break out of loop if null is detected
            if (waypoints[chosenNumber] == null)
                return newRoute;

            newRoute[waypointsAdded] = waypointList[chosenNumber];

            waypointList.RemoveAt(chosenNumber);
            waypointsAdded++;
        }

        return newRoute;
    }

    // Check if a random waypoint has already been selected
    private bool AlreadyInRoute(Transform[] patrolRoute, Transform chosenWaypoint)
    {
        foreach (Transform waypoint in patrolRoute)
        {
            if (waypoint == chosenWaypoint)
                return true;
        }

        return false;
    }

}
