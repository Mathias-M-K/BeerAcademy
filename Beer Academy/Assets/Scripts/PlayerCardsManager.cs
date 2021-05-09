using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerCardsManager : MonoBehaviour
{
    public static PlayerCardsManager current;
    private void Awake()
    {
        if (current == null) current = this;
    }

    [SerializeField]private GameObject playerCardTemplate;

    private Dictionary<Player, PlayerCard> _playerCards = new Dictionary<Player, PlayerCard>();
    
    private GridLayoutGroup _gridLayout;
    private RectTransform _gridContainer;

    private float _checkSize;

    private void Start()
    {
        //Getting grid layout element
        _gridLayout = transform.GetChild(0).GetComponent<GridLayoutGroup>();
        _gridContainer = transform.GetChild(0).GetComponent<RectTransform>();
        
        //Running setup for player cards
        StartCoroutine(SetGridDimensions());
        
    }

    private void SetupPlayerCards()
    {
        //Getting all players
        List<Player> players = GameController.current.GetAllPlayers();
        
        //editing preexisting player-card to match first player
        PlayerCard playerCard = playerCardTemplate.GetComponent<PlayerCard>();
        playerCard.SetInfo(players[0]);
        
        _playerCards.Add(players[0],playerCard);

        //Adding a player-card for each player
        for (int i = 1; i < players.Count; i++)
        {
            GameObject newPlayerCard = Instantiate(playerCardTemplate, transform.GetChild(0));
            
            PlayerCard pc = newPlayerCard.GetComponent<PlayerCard>();
            pc.SetInfo(players[i]);
            
            _playerCards.Add(players[i],pc);
        }
    }

    private IEnumerator SetGridDimensions()
    {
        yield return new WaitUntil(()=>_gridContainer.sizeDelta.x > 0);

        Vector2 gridSize = _gridContainer.sizeDelta;
        
        float nrOfPlayers = GameController.current.GetAllPlayers().Count;

        if(nrOfPlayers > 3 && nrOfPlayers < 7) OneRow();
        if(nrOfPlayers > 6) TwoRows();
        
        SetupPlayerCards();


        void OneRow()
        {
            float spacing = 10;

            float spacingTotal = spacing * (nrOfPlayers-1);

            float cardWidth = (gridSize.x - spacingTotal) / nrOfPlayers;
            

            _gridLayout.cellSize = new Vector2(cardWidth, gridSize.y);
            _gridLayout.spacing = new Vector2(10, 10);
        }

        void TwoRows()
        {
            float horizontalSpacing = 10;

            float horizontalSpacingTotal = horizontalSpacing * (nrOfPlayers - 1);

            float cardWidth = (gridSize.x - horizontalSpacingTotal) / Mathf.Ceil(nrOfPlayers / 2);

            float verticalSpacing = 10;

            float cardHeight = (gridSize.y - verticalSpacing) / 2;
            
            _gridLayout.cellSize = new Vector2(cardWidth, cardHeight);
            _gridLayout.spacing = new Vector2(horizontalSpacing, verticalSpacing);
        }
    }
    
    public void UpdatePlayer(Player player)
    {
        _playerCards[player].SetInfo(player);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerCardsManager))]
public class PlayerCardsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerCardsManager pcm = (PlayerCardsManager) target;
    }
}
#endif