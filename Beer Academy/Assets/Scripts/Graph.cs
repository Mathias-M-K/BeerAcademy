using System.Collections;
using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Graph : MonoBehaviour
{
    [Header("Other")]
    public UILineRenderer lineRenderer;
    public RectTransform graphOverlay;
    public GameObject point1;
    
    public float x1, y1;

    public List<GameObject> horizontalSeparators;
    public List<GameObject> verticalSeparators;
    public List<TextMeshProUGUI> horizontalSeparatorText;
    public List<TextMeshProUGUI> verticalSeparatorText;
    private readonly Dictionary<GameObject, DataPoint> _graphPoints = new Dictionary<GameObject, DataPoint>();

    [Header("Separator Settings")]
    public float horizontalSeparatorWidth;
    public float verticalSeparatorWidth;
    public int verticalSeparatorStepIncrement;
    
    private float _widthOfGraph;
    private float _heightOfGraph;
    public int maxVerticalValue;
    public int maxHorizontalValue;
    public Vector2 _lastPoint = new Vector2(0,0);

    
    public void Start()
    {
        StartCoroutine(InitiateGraph());
    }
    
    private IEnumerator InitiateGraph()
    {
        yield return new WaitForSeconds(2);
        Debug.Log($"Height: {graphOverlay.rect.height} | Width: {graphOverlay.rect.width}");

        Rect xy = graphOverlay.rect;
        _widthOfGraph = xy.width;
        _heightOfGraph = xy.height;
        
        UpdateHorizontalSeparatorPositions();
        UpdateVerticalSeparatorPositions();
        AddPointImage(new Vector2(0,0));
    }

    public void AddPointImage(Vector2 point)
    {
        if (point.x > maxHorizontalValue)
        {
            float x = (point.x - maxHorizontalValue);

            for (int i = 0; i < x; i++)
            {
                AddHorizontalSeparator();
            }
        }

        if (point.y > maxVerticalValue)
        {
            float x = Mathf.Ceil((point.y-maxVerticalValue)/verticalSeparatorStepIncrement);

            for (int i = 0; i < x; i++)
            {
                AddVerticalSeparator();
            }
        }
        
        //Vector3 v3 = new Vector3(horizontalSeparators[(int) point.x].transform.localPosition.x, ((_heightOfGraph / 2) * -1) - 100, 0);
        Vector2 v3 = _lastPoint;
        GameObject go = Instantiate(point1, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0), horizontalSeparators[0].transform.parent);
        go.GetComponent<RectTransform>().localPosition = v3;
        
        _graphPoints.Add(go,new DataPoint(point.x,point.y));
        
        AddPointToLineRenderer(new Vector2(v3.x,v3.y));

        _lastPoint = GetGraphPosition(point);
        
        UpdatePointPositions();
    }
    private void UpdatePointPositions()
    {
        int i = 0;
        foreach (KeyValuePair<GameObject,DataPoint> dataPoint in _graphPoints)
        {
            int y = i;
            LeanTween.moveLocal(dataPoint.Key, GetGraphPosition(new Vector2(dataPoint.Value.x,dataPoint.Value.y)), 0.75f).setEase(LeanTweenType.easeInOutQuad).setOnUpdateVector3(vector3 =>
            {
                lineRenderer.Points[y] = new Vector2(vector3.x, vector3.y);
                Debug.Log($"LineRendererPoint: {y} | X: {vector3.x} | Y: {vector3.y}");
                lineRenderer.SetAllDirty();
            });
            
            i++;
        }
    }

    private Vector2 GetGraphPosition(Vector2 point)
    {
        float xPos = MyResources.current.Remap(point.x, 0, maxHorizontalValue, (_widthOfGraph/2)*-1, _widthOfGraph/2);
        float yPos = MyResources.current.Remap(point.y, 0, maxVerticalValue, (_heightOfGraph/2)*-1, _heightOfGraph/2);

        return new Vector2(xPos, yPos);
    }

    private void AddPointToLineRenderer(Vector2 point)
    {
        var pointList = new List<Vector2>(lineRenderer.Points);
                
        pointList.Add(point);
        
        lineRenderer.Points = pointList.ToArray();
    }
    
    public void AddVerticalSeparator()
    {
        float yPos = (_heightOfGraph / 2)+200;

        GameObject newVerticalSeparator = Instantiate(verticalSeparators[0], 
            new Vector3(0, yPos, 0), 
            Quaternion.Euler(0, 0, 0),
            verticalSeparators[0].transform.parent);

        RectTransform newVerticalSeparatorRect = newVerticalSeparator.GetComponent<RectTransform>();
        newVerticalSeparatorRect.anchoredPosition = new Vector2(0, yPos);
        newVerticalSeparatorRect.sizeDelta = new Vector2(newVerticalSeparatorRect.sizeDelta.x, verticalSeparatorWidth);

        GameObject newVerticalSeparatorText = Instantiate(verticalSeparatorText[0].gameObject, 
            new Vector3(0, yPos, 0), 
            Quaternion.Euler(0, 0, 0),
            verticalSeparatorText[0].transform.parent);

        newVerticalSeparatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
        

        verticalSeparators.Add(newVerticalSeparator);
        verticalSeparatorText.Add(newVerticalSeparatorText.GetComponent<TextMeshProUGUI>());
        
        UpdateVerticalSeparatorPositions();
    }
    public void UpdateVerticalSeparatorPositions()
    {
        float increment = _heightOfGraph / (verticalSeparators.Count - 1);
        float yPos = (_heightOfGraph / 2) * -1;

        int i = 0;
        foreach (GameObject separator in verticalSeparators)
        {
            LeanTween.moveLocalY(separator, yPos, 0.75f).setEase(LeanTweenType.easeInOutQuad);

            verticalSeparatorText[i].text = (i*verticalSeparatorStepIncrement).ToString();
            LeanTween.moveLocalY(verticalSeparatorText[i].gameObject, yPos, 0.75f).setEase(LeanTweenType.easeInOutQuad);
            
            yPos += increment;

            maxVerticalValue = i*verticalSeparatorStepIncrement;
            i++;
        }
        
        UpdatePointPositions();
    }
    public void AddHorizontalSeparator()
    {
        float xPos = (_widthOfGraph / 2) + 250;
        GameObject newHorizontalSeperator = Instantiate(horizontalSeparators[0],
            new Vector3(xPos,0,0),
            Quaternion.Euler(0,0,0),
            horizontalSeparators[0].transform.parent);
        
        RectTransform newHorizontalSeperatorRect = newHorizontalSeperator.GetComponent<RectTransform>();
        newHorizontalSeperatorRect.anchoredPosition = new Vector2(xPos,0);
        newHorizontalSeperatorRect.sizeDelta = new Vector2(horizontalSeparatorWidth, newHorizontalSeperatorRect.sizeDelta.y);
        
        GameObject newHorizontalSeparatorText = Instantiate(horizontalSeparatorText[0].gameObject,
            new Vector3(xPos,0,0),
            Quaternion.Euler(0,0,0),
            horizontalSeparatorText[0].transform.parent);
        
        newHorizontalSeparatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos,0);

        horizontalSeparators.Add(newHorizontalSeperator);
        horizontalSeparatorText.Add(newHorizontalSeparatorText.GetComponent<TextMeshProUGUI>());
        
        UpdateHorizontalSeparatorPositions();
    }
    public void UpdateHorizontalSeparatorPositions()
    {
        float increment = _widthOfGraph / (horizontalSeparators.Count-1);
        float xPos = ((_widthOfGraph / 2) * -1);

        int i = 0;
        foreach (GameObject separator in horizontalSeparators)
        {
            //TODO can i just use moveLocalX?
            //Vector3 newPos = new Vector3(xPos, 0, 0);
            LeanTween.moveLocalX(separator, xPos, 0.75f).setEase(LeanTweenType.easeInOutQuad);

            horizontalSeparatorText[i].text = i.ToString();
            LeanTween.moveLocalX(horizontalSeparatorText[i].gameObject, xPos,  0.75f).setEase(LeanTweenType.easeInOutQuad);
            
            xPos += increment;
            
            maxHorizontalValue = i;
            i++;
        }
        
        UpdatePointPositions();
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

        if (GUILayout.Button("Start"))
        {
            graph.UpdateHorizontalSeparatorPositions();
            graph.UpdateVerticalSeparatorPositions();
        }
        if (GUILayout.Button("Add Image"))
        {
            graph.AddPointImage(new Vector2(graph.x1,graph.y1));
        }
        
        if (GUILayout.Button("Add Horizontal Separator"))
        {
            graph.AddHorizontalSeparator();
        }
        if (GUILayout.Button("Add Vertical Separator"))
        {
            graph.AddVerticalSeparator();
        }
        
    }
}
#endif
#endregion
