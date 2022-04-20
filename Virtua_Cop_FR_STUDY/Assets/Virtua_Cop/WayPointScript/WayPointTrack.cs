using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointTrack : MonoBehaviour
{
    public Color lineColor = Color.yellow;
    private Transform[] points;

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        points = GetComponentsInChildren<Transform>();

        

        



        for(int i = 1; i <= points.Length-2; i++)
        {

            Vector3 currPos = points[i].position;
            Vector3 nextPos = points[i+1].position;

            Gizmos.DrawLine(currPos, nextPos);

            currPos = nextPos;
        }
    }
}
