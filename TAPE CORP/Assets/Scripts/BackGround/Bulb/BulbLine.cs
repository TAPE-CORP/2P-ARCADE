using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform startPoint;
    public Transform endPoint;
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
