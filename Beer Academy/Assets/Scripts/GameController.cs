﻿using System;
using System.Collections.Generic;
using Data_Types;
using Debugging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameController : MonoBehaviour
{
    public static GameController current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        
        CreatePlayerList();
    }

    [Header("Game Settings")] 
    public List<string> playerNames;
    private readonly List<Player> _players = new List<Player>();
    private int _currentPlayer = 0;
    
    [Header("Card Flipping Elements")]
    public GameObject currentCardPos;
    public GameObject nextCardPos;
    public Card nextCard;

    [Header("Other")] 
    public DebugPanelController debugPanelController;

    //UI
    private Dictionary<int,TextMeshProUGUI> _cardCounters;
    private readonly Dictionary<int, int> _cardCounterValues = new Dictionary<int, int>();
    private TextMeshProUGUI _roundCounterText;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _currentPlayerName;

    //Game Functionality
    private readonly List<Card> _cards = new List<Card>();
    private bool _gameOver;
    
    //Timer
    private bool _timerStarted = false;
    private DateTime _startTime;
    
    //Counter 
    private int _roundCounter = 1;

    private void Start()
    {
        //nextCardPos.SetActive(false);
        currentCardPos.SetActive(false);
        
        _cardCounters = GetCardCounters();
        _roundCounterText = GameObject.FindGameObjectWithTag("RoundCounter").GetComponent<TextMeshProUGUI>();
        _timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
        _currentPlayerName = GameObject.FindGameObjectWithTag("CurrentPlayerName").GetComponent<TextMeshProUGUI>();
        
        SetUp();
        
        nextCard = SpawnCard(GetRandomCard());
        nextCardPos.SetActive(false);
    }

    private void CreatePlayerList()
    {
        //TODO This method should be deleted at some point
        int id = 0;
        foreach (string playerName in playerNames)
        {
            _players.Add(new Player(id,playerName,MyResources.current.GetColor(id)));
            id++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            debugPanelController.Toggle();
        }
        
        
        if(_timerStarted)
        {
            TimeSpan we = DateTime.Now - _startTime;
            
            _timerText.text = $"{we.Hours:00}:{we.Minutes:00}:{we.Seconds:00}";
        }
        
        
        if (Input.GetKeyDown(KeyCode.Space) && !_gameOver)
        {
            if (!_timerStarted)
            {
                _startTime = DateTime.Now;
                _timerStarted = true;
            }

            ShowNextCard();
        }
    }

    private void SetUp()
    {
        for (int i = 2; i <= 14; i++)
        {
            for (int y = 0; y < _players.Count; y++)
            {
                _cards.Add(new Card(i, (Suit)y,MyResources.current.GetPlayingCard(i)));
            }
        }
        
        SetAllCounters(_players.Count); 
    }

    /// <summary>
    /// Returns next player in line, and progress the round if all players have drawn a card
    /// </summary>
    /// <returns>Next Player</returns>
    private Player GetNextPlayer()
    {
        if (_currentPlayer >= _players.Count)
        {
            _currentPlayer = 0;
            _roundCounter++;
            DataGrid.current.AddRow(_roundCounter);
        }
        _roundCounterText.text = $"Round:{_roundCounter}";
        
        Player currentPlayer = _players[_currentPlayer];
        _currentPlayer++;

        return currentPlayer;
    }

    public List<Player> GetAllPlayers()
    {
        return _players;
    }

    private void ShowNextCard()
    {
        if(_gameOver) return;

        Player currentPlayer = GetNextPlayer();

        Card newNextCard = null;
        if (_cards.Count > 0)
        {
            newNextCard = SpawnCard(GetRandomCard());
        }
        else
        {
            _gameOver = true;
        }

        currentPlayer.sips += nextCard.rank;
            
        Graph.current.AddDataPoint(new DataPoint(_roundCounter,currentPlayer.sips,currentPlayer.sips/_roundCounter),currentPlayer);
        DataGrid.current.SetFieldValue(currentPlayer,nextCard.rank);

        _currentPlayerName.text = currentPlayer.name;
        _currentPlayerName.color = currentPlayer.color;

        LeanTween.move(nextCard.cardObj, currentCardPos.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);

        CardDisplay cd = nextCard.cardObj.GetComponent<CardDisplay>();
        cd.TurnCard(0.5f);
        

        SubtractOneFromCounter(cd.GetRank());

        nextCard.cardObj.transform.SetAsLastSibling();
        nextCard = newNextCard;
    }

    private Card SpawnCard(Card cardToSpawn)
    {
        GameObject newNextCard = Instantiate(cardToSpawn.cardObj, 
            nextCardPos.transform.position, 
            nextCardPos.transform.rotation,
            nextCardPos.transform.parent);

        AspectRatioFitter arf = nextCardPos.GetComponent<AspectRatioFitter>();
        AspectRatioFitter arf1 = newNextCard.AddComponent<AspectRatioFitter>();

        arf1.aspectMode = arf.aspectMode;
        arf1.aspectRatio = arf.aspectRatio;
        
        CardDisplay cd = newNextCard.GetComponent<CardDisplay>();
        cd.SetCardActive(false);
        cd.Initialize();
            
        RectTransform nCardRect = newNextCard.GetComponent<RectTransform>();
        RectTransform currentCardRect = nextCardPos.GetComponent<RectTransform>();
            
        nCardRect.anchorMin = currentCardRect.anchorMin;
        nCardRect.anchorMax = currentCardRect.anchorMax;
        nCardRect.anchoredPosition = currentCardRect.anchoredPosition;
        nCardRect.sizeDelta = currentCardRect.sizeDelta;
            
        newNextCard.transform.SetAsFirstSibling();

        cardToSpawn.cardObj = newNextCard;
        
        return cardToSpawn;
    }

    /*private GameObject GetRandomCard()
    {
        int randomCardNumber = Random.Range(0, _cards.Count);

        Card randomCard = _cards[randomCardNumber];
        _cards.RemoveAt(randomCardNumber);

        GameObject randomCardObj = MyResources.current.GetPlayingCard(randomCard.rank);

        CardDisplay randomCardDisplay = randomCardObj.GetComponent<CardDisplay>();
        randomCardDisplay.SetSuit(randomCard.suit);
        randomCardDisplay.SetRank(randomCard.rank);
        //randomCardObj.GetComponent<CardDisplay>().suit = randomCard.suit;
        
        Debug.Log($"Returning {randomCard.suit} {randomCard.rank}");
        return randomCardObj;
    }*/
    private Card GetRandomCard()
    {
        int randomCardNumber = Random.Range(0, _cards.Count);

        Card randomCard = _cards[randomCardNumber];
        _cards.RemoveAt(randomCardNumber);

        CardDisplay randomCardDisplay = randomCard.cardObj.GetComponent<CardDisplay>();
        randomCardDisplay.SetSuit(randomCard.suit);
        randomCardDisplay.SetRank(randomCard.rank);

        return randomCard;
    }
    

    private Dictionary<int,TextMeshProUGUI> GetCardCounters()
    {
        var cardCounterTextObjs = GameObject.FindGameObjectsWithTag("CardCounter");

        Dictionary<int, TextMeshProUGUI> cardCounters = new Dictionary<int, TextMeshProUGUI>();

        foreach (GameObject cardCounter in cardCounterTextObjs)
        {
            //Card name identifies which card the counter is for
            string cardCounterName = cardCounter.transform.parent.name;
            
            //Getting the card number or letter
            string card = cardCounterName.Split('-')[1].Trim();
            
            //If its a letter (A,K,Q or J) we convert it to a number
            int cardNumber = Int32.Parse(card);;
            
            //Adding it to our dictionary
            cardCounters.Add(cardNumber,cardCounter.GetComponent<TextMeshProUGUI>());
        }

        return cardCounters;
    }

    /// <summary>
    /// Sets the counter for a specific card (2,3,4,5,6,7,8,9,10,J,Q,K,A)
    /// </summary>
    /// <param name="cardCounter"></param>
    /// <param name="newValue"></param>
    private void SetCounter(int cardCounter,int newValue)
    {
        _cardCounters[cardCounter].text = newValue.ToString();
    }

    private void SubtractOneFromCounter(int rank)
    {
        _cardCounterValues[rank]--;
        
        SetCounter(rank, _cardCounterValues[rank]);
        
    }

    /// <summary>
    /// Sets the counter for all cards. I imagine this will be mostly used to start the game
    /// </summary>
    /// <param name="newValue"></param>
    private void SetAllCounters(int newValue)
    {
        _cardCounterValues.Clear();
        foreach (KeyValuePair<int,TextMeshProUGUI> counter in _cardCounters)
        {
            counter.Value.text = newValue.ToString();
            
            _cardCounterValues.Add(counter.Key,newValue);
        }

        
    }
}
