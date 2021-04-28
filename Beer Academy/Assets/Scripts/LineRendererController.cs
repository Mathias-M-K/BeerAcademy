using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LineRendererController : MonoBehaviour
{
    public UILineRenderer lineRenderer;
    public GameObject graphPoint;
    public DataPoint lastPoint= new DataPoint(0,0,0);
    public Dictionary<GameObject, DataPoint> _graphPoints = new Dictionary<GameObject, DataPoint>();

    private void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }
}