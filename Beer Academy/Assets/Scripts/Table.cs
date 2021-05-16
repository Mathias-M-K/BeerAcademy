using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using TableHelpClasses;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
#if UNITY_EDITOR
using UnityEditor;

#endif


public class Table : MonoBehaviour
{
    public static Table current;

    private void Awake()
    {
        if (current == null) current = this;
    }

    public RectTransform gridContainer;
    public RectTransform row;
    public RectTransform field;

    public RectTransform extRoundPanel;
    public RectTransform extRoundPanelField;

    private RowController _currentRow;

    private readonly Dictionary<Player, int> _playerIndexDictionary = new Dictionary<Player, int>();
    private List<RowController> _rowControllers = new List<RowController>();

    private float _imageWidth;
    private float _rowHeight;
    private float _rowWidth;
    private float _extRoundPanelHidePos;
    private float _extRoundPanelShowPos;

    private void Start()
    {
        GUIEvents.current.MouseHover += OnMouseHover;
        GUIEvents.current.MouseExit += OnMouseExit;

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

    private void OnMouseExit(Player player)
    {
        int playerIndex = _playerIndexDictionary[player];
        
        foreach (RowController rowController in _rowControllers)
        {
            if (rowController == null) continue;
            rowController.SetFrameColor(playerIndex, Color.white, false);
            rowController.SetTextColor(playerIndex, Color.white);
        }
    }

    private void OnMouseHover(Player player)
    {
        int playerIndex = _playerIndexDictionary[player];
        
        foreach (RowController rowController in _rowControllers)
        {
            if (rowController == null) continue;
            rowController.SetFrameColor(playerIndex, player.color, true);
            rowController.SetTextColor(playerIndex, player.color);
        }
        
        FocusOnPlayer(player);
    }

    private IEnumerator Setup()
    {
        yield return new WaitUntil(() => row.sizeDelta.x > 0);

        //Setting the name on the preexisting field
        field.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameController.current.GetAllPlayers()[0].name;

        //Adding a field to the row for every player, and adjusting the scrolling rect
        for (int i = 1; i < GameController.current.GetAllPlayers().Count; i++)
        {
            AddFieldToRow(GameController.current.GetAllPlayers()[i].name);
        }

        gridContainer.sizeDelta = new Vector2(
            row.sizeDelta.x + ((GameController.current.GetAllPlayers().Count - 1) * _imageWidth),
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

        //Hiding the external round panel
        _extRoundPanelShowPos = extRoundPanel.localPosition.x;
        _extRoundPanelHidePos = _extRoundPanelShowPos - extRoundPanelField.sizeDelta.x;
        HideExternalRoundPanel();

        _rowControllers.Add(_currentRow);

        //Adding first row
        AddRow(1);
    }

    public void SetFieldValue(Player player, float value)
    {
        _currentRow.SetValue(_playerIndexDictionary[player], value);

        FocusOnPlayer(player);
    }

    private void AddFieldToRow(string playerName)
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

        _rowControllers.Add(_currentRow);

        //Adding a row on the external round panel
        RectTransform roundRect = Instantiate(extRoundPanelField, extRoundPanel);
        roundRect.GetChild(0).GetComponent<TextMeshProUGUI>().text = round.ToString();
    }

    private void FocusOnPlayer(Player player)
    {
        float viewableAreaWidth = GetComponent<RectTransform>().sizeDelta.x;
        float contentWidth = gridContainer.sizeDelta.x;
        float diff = contentWidth - viewableAreaWidth;
        float incrementPrPlayer = diff / _playerIndexDictionary.Count;

        //gridContainer.anchoredPosition = new Vector3(-20, 2, 0);
        LeanTween.value(gridContainer.gameObject, gridContainer.anchoredPosition.x,
                -incrementPrPlayer * _playerIndexDictionary[player], 0.75f).setEase(LeanTweenType.easeInOutQuad)
            .setOnUpdate(
                (float f) => { gridContainer.anchoredPosition = new Vector3(f, 0, 0); });
    }

    public void ShowExternalRoundPanel()
    {
        LeanTween.moveLocalX(extRoundPanel.gameObject, _extRoundPanelShowPos, 0.75f)
            .setEase(LeanTweenType.easeInOutQuad);
    }

    public void HideExternalRoundPanel()
    {
        LeanTween.moveLocalX(extRoundPanel.gameObject, _extRoundPanelHidePos, 0.75f)
            .setEase(LeanTweenType.easeInOutQuad);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Table))]
public class TableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        Table table = (Table) target;

        if (GUILayout.Button("Hide Round Panel"))
        {
            table.HideExternalRoundPanel();
        }

        if (GUILayout.Button("Show Round Panel"))
        {
            table.ShowExternalRoundPanel();
        }
    }
}
#endif