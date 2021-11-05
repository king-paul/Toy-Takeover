using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public Transform[] wayPoints;
    
    public Vector3 GetWaypointPosition(int index)
    {
        return wayPoints[index].position;
    }

}
