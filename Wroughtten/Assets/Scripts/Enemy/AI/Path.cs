using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Transform transform;
    public float waitTime;
}

public class Path : MonoBehaviour
{
    [SerializeField] private List<Waypoint> waypoints;

    public Waypoint GetWaypoint(int wp){
        return waypoints[wp];
    }

    public Tuple<Waypoint,int> GetNextWaypoint(int wp){
        
        return (wp+1 >= waypoints.Count) ? 
        new Tuple<Waypoint,int>(waypoints[0],0) : 
        new Tuple<Waypoint,int>(waypoints[wp+1],wp+1);
    }

    private void OnDrawGizmosSelected(){
        
        for(int i = 0; i < waypoints.Count; i++){
            Gizmos.DrawSphere(waypoints[i].transform.position,0.25f);
            Waypoint nextwp = GetNextWaypoint(i).Item1;
            Gizmos.DrawLine(waypoints[i].transform.position,nextwp.transform.position);
           
        }
    }
}
