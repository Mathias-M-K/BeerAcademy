using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Graph : MonoBehaviour
{
    public static Graph current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
    }
    
    [Header("Template Objects")]    //Default objects and templates
    public GameObject graphPoint;
    public LineRendererController lineRenderer;
    public List<GameObject> horizontalSeparators;
    public List<GameObject> verticalSeparators;
    public List<TextMeshProUGUI> horizontalSeparatorText;
    public List<TextMeshProUGUI> verticalSeparatorText;
        
    [Header("Graph Overlay")]   //We need height and width from this
    public RectTransform graphOverlay;
    
    [Header("Separator Settings")]
    public float horizontalSeparatorWidth;
    public float verticalSeparatorWidth;
    public int verticalSeparatorStepIncrement;

    [Header("Animation Settings")] 
    public float animationSpeed = 0.75f;
    private int _verticalSeparatorStepIncrementSaved;    //Saved value

    private float _widthOfGraph;
    private float _heightOfGraph;
    private int _maxVerticalValue;      //The current maximum vertical value on the y-axis
    private int _maxHorizontalValue;    //The current maximum horizontal value on x-axis    
    
    //Other
    private int _dataPointFetchIndex = 1;    //if 1, the graph will display nrOfBeers, of 2, it will display avg beers

    private readonly Dictionary<Player, LineRendererController> _lineRenderers = new Dictionary<Player, LineRendererController>();
    
    private float _highestAvgSip = 0.01f;
    private float _highestSipCount = 0.01f;
    
    
    public void Start()
    {
        //Because i need to do a lot of animations, i have to initiate it
        LeanTween.init(1600);
        
        StartCoroutine(InitiateGraph());
        _verticalSeparatorStepIncrementSaved = verticalSeparatorStepIncrement;
    }
    
    
    /// <summary>
    /// Initiates the graph with the default separators and the height and width of the graph in the current screen aspect ratio.
    /// The method waits until it can fetch the dimensions of the graph box, since these dimensions are critical for the rest of the code.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitiateGraph()
    {
        yield return new WaitUntil(() => graphOverlay.sizeDelta.x > 0);

        //Getting height and width from graph rect
        Rect graphXY = graphOverlay.rect;
        _widthOfGraph = graphXY.width;
        _heightOfGraph = graphXY.height;
        
        //Updates all separators to their correct position and spawns a point at 0,0
        UpdateHorizontalSeparatorPositions();
        UpdateVerticalSeparatorPositions();
        

        
        //Getting players from the game controller
        foreach (Player player in GameController.current.GetAllPlayers())
        {
            AddNewPlayer(player);
        }
        
        Destroy(lineRenderer.gameObject);
    }

    /// <summary>
    /// Adds a new line-renderer for each player to show their individual stats.
    /// </summary>
    /// <param name="player"></param>
    public void AddNewPlayer(Player player)
    {
        GameObject newLineRendererObj = Instantiate(lineRenderer.gameObject, 
            new Vector3(0, 0, 0), 
            Quaternion.Euler(0, 0, 0),
            lineRenderer.transform.parent);

        RectTransform newLineRendererRect = newLineRendererObj.GetComponent<RectTransform>();
        RectTransform originalLineRendererRect = lineRenderer.GetComponent<RectTransform>();

        newLineRendererRect.sizeDelta = originalLineRendererRect.sizeDelta;
        newLineRendererRect.anchoredPosition = originalLineRendererRect.anchoredPosition;

        //If there is any points in the lineRenderer, they are removed
        UILineRenderer newLineRenderer = newLineRendererObj.GetComponent<UILineRenderer>();
        newLineRenderer.Points = new List<Vector2>().ToArray();
        
        //Setting color of the line renderer
        LineRendererController newLineRendererController = newLineRenderer.GetComponent<LineRendererController>();
        newLineRendererController.SetColor(player.color);
        
        _lineRenderers.Add(player,newLineRendererController);
        
        AddDataPoint(new DataPoint(0,0,0),player);
    }

    /// <summary>
    /// Adds a new point on the graph using x and y. Coordinates should correspond to the graph
    /// </summary>
    /// <param name="point"></param>
    /// <param name="player"></param>
    public void AddDataPoint(DataPoint point, Player player)
    {
        //Checking if new point is setting any new max values
        if (point.avgSip > _highestAvgSip) _highestAvgSip = point.avgSip;
        if (point.numberOfSips > _highestSipCount) _highestSipCount = point.numberOfSips;
        
        //The spawn position is not it's final position. This is merely the position it waits, before being animated to its final position.
        Vector2 pointSpawnPos = GetGraphPosition(new Vector2(_lineRenderers[player].lastPoint[0],_lineRenderers[player].lastPoint[_dataPointFetchIndex]));
        
        //Creating the new point and placing it at the spawn position
        GameObject go = Instantiate(graphPoint, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0), horizontalSeparators[0].transform.parent);
        go.GetComponent<RectTransform>().localPosition = pointSpawnPos;
        go.GetComponent<Image>().color = player.color;
        
        //Adding the new point to our the required list and dictionaries 
        _lineRenderers[player].graphPoints.Add(go,point);
        AddPointToLineRenderer(new Vector2(pointSpawnPos.x,pointSpawnPos.y),_lineRenderers[player].lineRenderer);

        //Updating _lastPoint with our new point
        _lineRenderers[player].lastPoint = point;
        
        UpdatePointPositions(_lineRenderers[player]);
        
        _lineRenderers[player].transform.SetAsLastSibling();
        
        //If the graph it to limited to show the new point, we expand it till it fits 
        CheckAndExpandAxis(point);
    }
    
    /// <summary>
    /// Checks if the given datapoint fits in the graph. If not, the graph will be expanded
    /// </summary>
    /// <param name="point"></param>
    private void CheckAndExpandAxis(DataPoint point)
    {
        //Checks if the round number is represented on the horizontal axis. If not, the graph will be expanded horizontally
        if (point.round > _maxHorizontalValue)
        {
            float x = (point.round - _maxHorizontalValue);

            for (int i = 0; i < x; i++)
            {
                AddHorizontalSeparator();
            }
        }
        
        //Checks if the value is represented on the vertical axis. If now, the graph will be expanded vertically
        if (point[_dataPointFetchIndex] > _maxVerticalValue)
        {
            float x = Mathf.Ceil((point[_dataPointFetchIndex]-_maxVerticalValue)/verticalSeparatorStepIncrement);

            for (int i = 0; i < x; i++)
            {
                AddVerticalSeparator();
            }
        }
    }

    /// <summary>
    /// Can both add and remove separators to ensure that the graph fits the data
    /// </summary>
    /// <param name="point"></param>
    /// <param name="removeRedundantSeparators"></param>
    private void CheckAndExpandAxis(DataPoint point, bool removeRedundantSeparators)
    {
        //Expands the graph if necessary
        CheckAndExpandAxis(point);
        
        //Shrinks the graph as far as possible if requested
        if(removeRedundantSeparators)
        {
            float x = Mathf.Floor((_maxVerticalValue-point[_dataPointFetchIndex])/verticalSeparatorStepIncrement);

            if (x >= 1)
            {
                for (int i = 0; i < x; i++)
                {
                    RemoveVerticalSeparator();
                } 
            }
        }

    }
    
    /// <summary>
    /// Iterates through every point in the graph and animates them to their correct position
    /// </summary>
    private void UpdatePointPositions(LineRendererController lineRendererController)
    {
        int i = 0;
        foreach (KeyValuePair<GameObject,DataPoint> dataPoint in lineRendererController.graphPoints)
        {
            int y = i;
            LeanTween.moveLocal(dataPoint.Key, GetGraphPosition(new Vector2(dataPoint.Value[0],dataPoint.Value[_dataPointFetchIndex])), animationSpeed).setEase(LeanTweenType.easeInOutQuad).setOnUpdateVector3(vector3 =>
            {
                lineRendererController.lineRenderer.Points[y] = new Vector2(vector3.x, vector3.y);
                lineRendererController.lineRenderer.SetAllDirty();  //Don't know what this does, but it makes my code work
            });
            
            i++;
        }
    }

    /// <summary>
    /// Returns the localPosition in the graph that corresponds to the vector2 input
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Vector2 GetGraphPosition(Vector2 point)
    {
        float xPos = MyResources.current.Remap(point.x, 0, _maxHorizontalValue, (_widthOfGraph/2)*-1, _widthOfGraph/2);
        float yPos = MyResources.current.Remap(point.y, 0, _maxVerticalValue, (_heightOfGraph/2)*-1, _heightOfGraph/2);

        return new Vector2(xPos, yPos);
    }

    /// <summary>
    /// Adds a new point to the line-renderer. The line-renderer is responsible for rendering the lines, but not the points.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lr"></param>
    private void AddPointToLineRenderer(Vector2 point,UILineRenderer lr)
    {
        var pointList = new List<Vector2>(lr.Points);
                
        pointList.Add(point);
        
        lr.Points = pointList.ToArray();
    }
    
    /// <summary>
    /// Adds a new vertical separator from the top
    /// </summary>
    public void AddVerticalSeparator()
    {
        //The position where the new separator will be spawned before being animated to it's destination.
        float yPos = (_heightOfGraph / 2)+200;

        //Creating new separator
        GameObject newVerticalSeparator = Instantiate(verticalSeparators[0], 
            new Vector3(0, yPos, 0), 
            Quaternion.Euler(0, 0, 0),
            verticalSeparators[0].transform.parent);
        
        RectTransform newVerticalSeparatorRect = newVerticalSeparator.GetComponent<RectTransform>();
        newVerticalSeparatorRect.anchoredPosition = new Vector2(0, yPos);
        newVerticalSeparatorRect.sizeDelta = new Vector2(newVerticalSeparatorRect.sizeDelta.x, verticalSeparatorWidth);

        //Creates new text field to follow the new separator
        GameObject newVerticalSeparatorText = Instantiate(verticalSeparatorText[0].gameObject, 
            new Vector3(0, yPos, 0), 
            Quaternion.Euler(0, 0, 0),
            verticalSeparatorText[0].transform.parent);

        newVerticalSeparatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
        
        //Adding separator and text to list, so we can update them at a later time
        verticalSeparators.Add(newVerticalSeparator);
        verticalSeparatorText.Add(newVerticalSeparatorText.GetComponent<TextMeshProUGUI>());
        
        UpdateVerticalSeparatorPositions();
    }

    /// <summary>
    /// Removes a line separator from the top
    /// </summary>
    public void RemoveVerticalSeparator()
    {
        int lastSeparatorIndex = verticalSeparators.Count - 1;
        float deletePos = (_heightOfGraph / 2)+50;
        
        LeanTween.moveLocalY(verticalSeparators[lastSeparatorIndex], deletePos, animationSpeed).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveLocalY(verticalSeparatorText[lastSeparatorIndex].gameObject, deletePos, animationSpeed).setEase(LeanTweenType.easeInOutQuad);
        
        Destroy(verticalSeparators[lastSeparatorIndex],2);
        verticalSeparators.RemoveAt(verticalSeparators.Count-1);
        
        Destroy(verticalSeparatorText[lastSeparatorIndex].gameObject,2);
        verticalSeparatorText.RemoveAt(lastSeparatorIndex);
        
        UpdateVerticalSeparatorPositions();
    }
    
    /// <summary>
    /// Updates all vertical separators and animates them to their correct position. Often called when adding new point
    /// </summary>
    public void UpdateVerticalSeparatorPositions()
    {
        //The space between each separator
        float increment = _heightOfGraph / (verticalSeparators.Count - 1);
        
        //the position to place the separator when iterating though them
        float yPos = (_heightOfGraph / 2) * -1;

        int i = 0;
        foreach (GameObject separator in verticalSeparators)
        {
            LeanTween.moveLocalY(separator, yPos, animationSpeed).setEase(LeanTweenType.easeInOutQuad);

            //Moves the text so it follows it's separator, and the text itself to reflect the graph correctly
            verticalSeparatorText[i].text = (i*verticalSeparatorStepIncrement).ToString();
            LeanTween.moveLocalY(verticalSeparatorText[i].gameObject, yPos, animationSpeed).setEase(LeanTweenType.easeInOutQuad);
            
            //Updates yPos to reflect where the next separator should be
            yPos += increment;

            //Recalculates the maximum vertical value, since another vertical separator have been added
            _maxVerticalValue = i*verticalSeparatorStepIncrement;
            
            i++;
        }

        foreach (KeyValuePair<Player,LineRendererController> uiLineRenderer in _lineRenderers)
        {
            UpdatePointPositions(uiLineRenderer.Value);
        }
    }
    
    /// <summary>
    /// Adds a horizontal separator from left
    /// </summary>
    public void AddHorizontalSeparator()
    {
        //The position where the new separator will be spawned before being animated to it's destination.
        float xPos = (_widthOfGraph / 2) + 250;
        
        //Creating new separator
        GameObject newHorizontalSeparator = Instantiate(horizontalSeparators[0],
            new Vector3(xPos,0,0),
            Quaternion.Euler(0,0,0),
            horizontalSeparators[0].transform.parent);
        
        RectTransform newHorizontalSeperatorRect = newHorizontalSeparator.GetComponent<RectTransform>();
        newHorizontalSeperatorRect.anchoredPosition = new Vector2(xPos,0);
        newHorizontalSeperatorRect.sizeDelta = new Vector2(horizontalSeparatorWidth, newHorizontalSeperatorRect.sizeDelta.y);
        
        //Creates new text field to follow the new separator
        GameObject newHorizontalSeparatorText = Instantiate(horizontalSeparatorText[0].gameObject,
            new Vector3(xPos,0,0),
            Quaternion.Euler(0,0,0),
            horizontalSeparatorText[0].transform.parent);
        
        newHorizontalSeparatorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos,0);

        //Adding separator and text to list, so we can update them at a later time
        horizontalSeparators.Add(newHorizontalSeparator);
        horizontalSeparatorText.Add(newHorizontalSeparatorText.GetComponent<TextMeshProUGUI>());
        
        /*foreach (UILineRenderer uiLineRenderer in lineRenderers)
        {
            UpdatePointPositions(uiLineRenderer.GetComponent<LineRendererController>());
        }*/
        
        UpdateHorizontalSeparatorPositions();
    }
    
    /// <summary>
    /// Updates all horizontal separators and animates them to their correct position. Often called when adding new point to the graph
    /// </summary>
    public void UpdateHorizontalSeparatorPositions()
    {
        //The space between each separator
        float increment = _widthOfGraph / (horizontalSeparators.Count-1);
        
        //the position to place the separator when iterating though them
        float xPos = ((_widthOfGraph / 2) * -1);

        int i = 0;
        foreach (GameObject separator in horizontalSeparators)
        {
            LeanTween.moveLocalX(separator, xPos, animationSpeed).setEase(LeanTweenType.easeInOutQuad);

            //Moves the text so it follows it's separator, and the text itself to reflect the graph correctly
            horizontalSeparatorText[i].text = i.ToString();
            LeanTween.moveLocalX(horizontalSeparatorText[i].gameObject, xPos,  animationSpeed).setEase(LeanTweenType.easeInOutQuad);
            
            //Updates yPos to reflect where the next separator should be
            xPos += increment;
            
            //Recalculates the maximum vertical value, since another vertical separator have been added
            _maxHorizontalValue = i;
            
            i++;
        }
        
        foreach (KeyValuePair<Player,LineRendererController> uiLineRenderer in _lineRenderers)
        {
            UpdatePointPositions(uiLineRenderer.Value);
        }
    }

    public void ShowAvgSipGraph()
    {
        verticalSeparatorStepIncrement = 1;
        _maxVerticalValue = verticalSeparatorStepIncrement * (verticalSeparators.Count-1);

        _dataPointFetchIndex = 2;

        CheckAndExpandAxis(new DataPoint(0,0,_highestAvgSip),true);

        UpdateVerticalSeparatorPositions();

        foreach (KeyValuePair<Player,LineRendererController> lr in _lineRenderers)
        {
            UpdatePointPositions(lr.Value);
        }
    }

    public void ShowBeerCountGraph()
    {
        verticalSeparatorStepIncrement = _verticalSeparatorStepIncrementSaved;
        _maxVerticalValue = verticalSeparatorStepIncrement * (verticalSeparators.Count-1);
        
        _dataPointFetchIndex = 1;
        
        CheckAndExpandAxis(new DataPoint(0,_highestSipCount,0),true);
        
        UpdateVerticalSeparatorPositions();
        
        foreach (KeyValuePair<Player,LineRendererController> lr in _lineRenderers)
        {
            UpdatePointPositions(lr.Value);
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

        if (GUILayout.Button("Update"))
        {
            graph.UpdateHorizontalSeparatorPositions();
            graph.UpdateVerticalSeparatorPositions();
        }
        if (GUILayout.Button("Add Player"))
        {
            graph.AddNewPlayer(new Player(1,"Mathias",MyResources.current.GetColor(1)));
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Add Horizontal Separator"))
        {
            graph.AddHorizontalSeparator();
        }
        if (GUILayout.Button("Add Vertical Separator"))
        {
            graph.AddVerticalSeparator();
        }
        if (GUILayout.Button("Remove Vertical Separator"))
        {
            graph.RemoveVerticalSeparator();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Show Avg Sip Graph"))
        {
            graph.ShowAvgSipGraph();
        }
        if (GUILayout.Button("Show Beer Count Graph"))
        {
            graph.ShowBeerCountGraph();
        }
        
    }
}
#endif
#endregion
