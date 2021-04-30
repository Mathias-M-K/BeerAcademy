using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DataGrid : MonoBehaviour
{
    public static DataGrid current;

    private void Awake()
    {
        if (current == null) current = this;
    }

    public RectTransform gridContainer;
    public RectTransform row;
    public RectTransform field;

    private RowController _currentRow;

    private readonly Dictionary<Player, int> _playerIndexDictionary = new Dictionary<Player, int>();

    private float _imageWidth;
    private float _rowHeight;
    private float _rowWidth;

    private void Start()
    {
        _imageWidth = field.sizeDelta.x;
        _rowHeight = row.sizeDelta.y;

        //Creating player index dictionary, where i can get their index by using their name as key
        int i = 0;
        foreach (Player player in GameController.current.GetAllPlayers())
        {
            _playerIndexDictionary.Add(player, i);
            i++;
        }

        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        yield return new WaitUntil(() => row.sizeDelta.x > 0);

        //Setting the name on the preexisting field
        field.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameController.current.GetAllPlayers()[0].name;
        
        //Editing the preexisting row
        //row.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);

        //Adding a field to the row for every player, and adjusting the scrolling rect
        for (int i = 1; i < GameController.current.GetAllPlayers().Count; i++)
        {
            AddFieldToRow(GameController.current.GetAllPlayers()[i].name);
        }

        gridContainer.sizeDelta = new Vector2(row.sizeDelta.x + ((GameController.current.GetAllPlayers().Count - 1) * _imageWidth),
            gridContainer.sizeDelta.y);


        //Setting width of fields
        foreach (Transform child in gridContainer.transform)
        {
            float width;

            if (GameController.current.GetAllPlayers().Count <= 4)
            {
                gridContainer.sizeDelta = GetComponent<RectTransform>().sizeDelta;
                width = (gridContainer.sizeDelta.x - 25) / GameController.current.GetAllPlayers().Count;
            }
            else
            {
                width = field.sizeDelta.x;
            }

            child.GetComponent<RowController>().SetImageWidth(width);
        }

        AddRow(1);
    }

    public void SetFieldValue(Player player, float value)
    {
        _currentRow.SetValue(_playerIndexDictionary[player], value);
    }

    public void AddFieldToRow(string playerName)
    {
        RectTransform rect = Instantiate(field, row);

        rect.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerName;
    }

    public void AddRow(int round)
    {
        //TODO can i do it like this in the graph class maybe?
        RectTransform rect = Instantiate(row, row.parent);

        _currentRow = rect.GetComponent<RowController>();
        _currentRow.ClearAllFields();
        _currentRow.SetRound(round);

        rect.localPosition = new Vector3(row.localPosition.x, row.localPosition.y - _rowHeight);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataGrid))]
public class DataGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        DataGrid dataGrid = (DataGrid) target;

        if (GUILayout.Button("Add Image to Row"))
        {
            dataGrid.AddFieldToRow("New Field");
        }

        if (GUILayout.Button("New Row"))
        {
            dataGrid.AddRow(2);
        }
    }
}
#endif