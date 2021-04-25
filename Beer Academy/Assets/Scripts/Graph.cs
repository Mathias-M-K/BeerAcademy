using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Graph : MonoBehaviour
{
    public UILineRenderer lineRenderer;
    public RectTransform point1;
    public GameObject columnsSeparator;
    public float x1, y1;

    private List<GameObject> _seperators = new List<GameObject>();

    private float _widthOfGraph;

    public void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        
        _seperators.Add(columnsSeparator);
        MoveSeparator(columnsSeparator,0);

        _widthOfGraph = GetComponent<RectTransform>().sizeDelta.x;
    }


    public void AddPoint(Vector2 point)
    {
        var pointList = new List<Vector2>(lineRenderer.Points);
        pointList.Add(point);

        lineRenderer.Points = pointList.ToArray();
    }

    public void MoveSeparator(GameObject separator, float xPos)
    {
        LeanTween.moveLocal(separator, new Vector3(xPos, 0, 0), 0.75f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void AddSeparator()
    {

        float xPos = ((_widthOfGraph / 2) * -1)-20;
        GameObject separator = Instantiate(columnsSeparator,new Vector3(xPos,0,0),Quaternion.Euler(0,0,0),columnsSeparator.transform.parent);
        
        _seperators.Add(separator);
        
        UpdateSeparators();
    }

    public void UpdateSeparators()
    {
        float increment = _widthOfGraph / (_seperators.Count+1);
        float xPos = (_widthOfGraph/2) - increment;

        foreach (GameObject separator in _seperators)
        {
            
            LeanTween.moveLocal(separator, new Vector3(xPos, 0, 0), 0.75f).setEase(LeanTweenType.easeInOutQuad);
            xPos -= increment;
        }
        
    }
    

    
}

#region Custom Card Editor
#if UNITY_EDITOR
[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{ public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        
        Graph graph = (Graph) target;

        if (GUILayout.Button("Add Point"))
        {
            graph.AddPoint(new Vector2(graph.x1,graph.y1));
        }

        if (GUILayout.Button("To Point"))
        {
            graph.AddPoint(graph.point1.localPosition);
        }
        
        if (GUILayout.Button("Add Separator"))
        {
            graph.AddSeparator();
        }
        
        
        
    }
}
#endif
#endregion
