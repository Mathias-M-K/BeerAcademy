using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;


public class LineRendererController : MonoBehaviour
{
    public UILineRenderer lineRenderer;
    public DataPoint lastPoint= new DataPoint(0,0,0);
    public readonly Dictionary<GameObject, DataPoint> graphPoints = new Dictionary<GameObject, DataPoint>();

    private void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }

    public void SetColor(Color32 color)
    {
        lineRenderer.color = color;

        foreach (KeyValuePair<GameObject,DataPoint> dataPoint in graphPoints)
        {
            dataPoint.Key.GetComponent<Image>().color = color;
        }
    }
}