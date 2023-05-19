using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Transform> nodes = new List<Transform>();
    public Color lineColor;
    private void OnDrawGizmos() {
        foreach (Transform child in transform) {
            if (!nodes.Contains(child)) {
                nodes.Add(child);
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (i + 1 >= nodes.Count) {
                Gizmos.DrawLine(nodes[i].position, nodes[0].position);
                Gizmos.color = lineColor;
            } else {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
                Gizmos.color = lineColor;
            }
        }
    }
}
