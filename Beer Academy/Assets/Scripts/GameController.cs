using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using PauseScreenAndSettings;
using SupportClasses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Card Flipping Elements")]
    public GameObject currentCardPos;
    public GameObject nextCardPos;
    private Card _currentCard;
    private Card _nextCard;

    [Header("UI Elements")]
    public TextMeshProUGUI previusPlayerText;
    public TextMeshProUGUI waitingForPlayerText;

    public TextMeshProUGUI playerTimerText;
    
    private TextMeshProUGUI _roundCounterText;
    private TextMeshProUGUI _gameTimerText;
    private TextMeshProUGUI _currentPlayerNameText;

    
    //Game Functionality
    private Dictionary<int,TextMeshProUGUI> _cardCounters;
    private readonly Dictionary<int, int> _cardCounterValues = new Dictionary<int, int>();
    private Dictionary<Player, PlayerCard> _playerCards = new Dictionary<Player, PlayerCard>();
    
    private readonly List<Card> _cards = new List<Card>();
    private readonly List<Player> _players = new List<Player>();
    
    private bool _gameOver;
    private int _roundCounter = 1;
    private int _currentPlayerIndex = 1;

    private Player _currentPlayer;
    private Player _nextPlayer;
    private Player _previousPlayer;
    private bool _gameJustStarted = true;
    



    //Game Timer
    [HideInInspector] public TimeSpan ElapsedGameTime;
    
    private bool _timerStarted = false;
    private DateTime _startTime;
    
    public Timer gameTimer;
    public Timer playerTimer;
    
    //Player Timer
    [HideInInspector] public TimeSpan ElapsedPlayerTime;
    
    private DateTime _playerStartTime;

    //Pause
    private bool _gamePaused;
    private DateTime _pauseStartTime;
    private TimeSpan _totalTimeoutTime;

    private bool _playerTimePaused;
    private DateTime _playerPauseStartTime;
    private TimeSpan _totalTimeoutTimePlayer;
    
    
    //Events
    public event Action SpacePressed;
    
    
    private void Start()
    {
        StatTracker.statTracker.StartTracking();
        
        currentCardPos.SetActive(false);
        
        _cardCounters = GetCardCounters();
        _roundCounterText = GameObject.FindGameObjectWithTag("RoundCounter").GetComponent<TextMeshProUGUI>();
        _gameTimerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
        _currentPlayerNameText = GameObject.FindGameObjectWithTag("CurrentPlayerName").GetComponent<TextMeshProUGUI>();

        CardSetup();

        _currentPlayer = _players[0];
        _nextPlayer = _players[1];
        _previousPlayer = _players[0];
        UpdateCurrentAndNextPlayerTextFields();

        previusPlayerText.text = "";
        
        _nextCard = SpawnCard(GetRandomCard(_currentPlayer.IsProtected));
        //nextCardPos.SetActive(false);
        
        
        //Subscribing to pause events
        PausePanelController.current.Pause += OnPause;
        PausePanelController.current.Unpause += OnUnPause;
        ChuckPanelController.current.ChuckPanelClose += OnChuckPanelClose;
    }
    

    private void CardSetup()
    {
        //Creating a deck of cards for each player
        for (int i = 2; i <= 14; i++)
        {
            for (int y = 0; y < _players.Count; y++)
            {
                _cards.Add(new Card(i, (Suit)y,MyResources.current.GetPlayingCard(i)));
            }
        }
        SetAllCounters(_players.Count);
    }

    private void CreatePlayerList()
    {
        List<string> newPlayers;
        int id = 0;
        
        if (GlobalValueContainer.Container == null)
        {
            newPlayers = playerNames;
        }
        else
        {
            newPlayers = GlobalValueContainer.Container.players;
        }
        
        foreach (string playerName in newPlayers)
        {
            _players.Add(new Player(id,playerName,GlobalValueContainer.Container.playerSips[id],GlobalValueContainer.Container.playerProtectedStatus[id],MyResources.current.GetColor(id)));
            id++;
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanelController.current.TogglePanel();
        }

        if(_timerStarted && !_gamePaused)
        {
            UpdateGameTime();
            
            
            if(_playerTimePaused) return;
            UpdatePlayerTime();
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !_gameOver && !ChuckPanelController.current.timerActive && !PausePanelController.current.panelActive)
        {
            if (!_timerStarted)
            {
                _startTime = DateTime.Now;
                _timerStarted = true;
            }
            _playerStartTime = DateTime.Now;
            _totalTimeoutTimePlayer = TimeSpan.FromMinutes(0);

            //Notifying listeners
            OnSpacePressed();
            
            //progress game
            ShowNextCard();
        }
    }

    private void ShowNextCard()
    {
        Card newNextCard = null;
        
        if (_nextCard.rank == 14)
        {
            ChuckPanelController.current.ShowTimer(_currentPlayer,_nextCard.suit);
            PausePlayerTimer();

            _currentPlayer.Sips += _currentPlayer.SipsInABeer;
        }
        else
        {
            _currentPlayer.Sips += _nextCard.rank;
        }
        
        //Updating player with new info
        _currentPlayer.AvgSips = _currentPlayer.Sips / _roundCounter;
        _currentPlayer.Beers = _currentPlayer.Sips / _currentPlayer.SipsInABeer;

        _currentPlayer.LastTime = ElapsedPlayerTime;
        _currentPlayer.TimeTotal += ElapsedPlayerTime;
        _currentPlayer.AvgTime = TimeSpan.FromSeconds(_currentPlayer.TimeTotal.TotalSeconds / _roundCounter);

        //if (_nextCard.rank == 14) _currentPlayer.sips--;
        
        StatTracker.statTracker.Log(ElapsedGameTime,_nextCard,_currentPlayer);
        
        //Updating round counter
        _roundCounterText.text = $"Round:{_roundCounter}";

        //Moving card to the used-cards stack and flipping it so it faces up
        LeanTween.move(_nextCard.cardObj, currentCardPos.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        CardDisplay cd = _nextCard.cardObj.GetComponent<CardDisplay>();
        cd.TurnCard(0.5f);
        
        //Updating the card counter for the used card
        SubtractOneFromCounter(cd.GetRank());

        _nextCard.cardObj.transform.SetAsLastSibling();
        
        //Updating components
        Graph.current.AddDataPoint(new DataPoint(_roundCounter,_currentPlayer.Sips,_currentPlayer.AvgSips,(float)_currentPlayer.LastTime.TotalSeconds,(float)_currentPlayer.AvgTime.TotalSeconds),_currentPlayer);
        Table.current.SetFieldValue(_currentPlayer,_nextCard.rank);
        PlayerCardsManager.current.UpdatePlayer(_currentPlayer);
        
        //Getting next player
        _currentPlayer = GetNextPlayerInLine();
        
        //Updating text
        UpdateCurrentAndNextPlayerTextFields();
        
        if (_cards.Count > 0)
        {
            newNextCard = SpawnCard(GetRandomCard(_currentPlayer.IsProtected));
        }
        else
        {
            _gameOver = true;
            OnPause();

            if (!ChuckPanelController.current.timerActive)
            {
                StartCoroutine(EndGame(3));
            }
        }
        
        //Updating card
        _nextCard = newNextCard;
    }
    
    private void UpdateCurrentAndNextPlayerTextFields()
    {
        //Current Player
        waitingForPlayerText.text = $"<color=white>Now drawing:</color> {_currentPlayer.Name}";
        waitingForPlayerText.color = _currentPlayer.Color;
        
        _currentPlayerNameText.text = _currentPlayer.Name;
        _currentPlayerNameText.color = _currentPlayer.Color;


        if (_gameJustStarted)
        {
            _gameJustStarted = false;
        }
        else
        {
            previusPlayerText.text = $"{_previousPlayer.Name} <color=white>Got {_nextCard.rank} sips </color>";
        
            if(_nextCard.rank == 14) previusPlayerText.text = $"{_previousPlayer.Name} <color=white>Got an ACE! </color>";
            previusPlayerText.color = _previousPlayer.Color; 
        }
        
    }
    
    private void UpdateGameTime()
    {
        TimeSpan tempElapsedGameTime = DateTime.Now - _startTime;
        tempElapsedGameTime -= _totalTimeoutTime;
        ElapsedGameTime = tempElapsedGameTime;
        gameTimer.SetTimer(ElapsedGameTime);
    }

    private void UpdatePlayerTime()
    {
        ElapsedPlayerTime = DateTime.Now - _playerStartTime;
        ElapsedPlayerTime -= _totalTimeoutTimePlayer;
        playerTimer.SetTimer(ElapsedPlayerTime);
    }
    
    private void OnChuckPanelClose()
    {

        if (_gameOver)
        {
            StartCoroutine(EndGame(3));
        }
        else
        {
            UnpausePlayerTimer();  
        }
        
    }
    
    private void OnPause()
    {
        if (!_timerStarted) return;
        
        _pauseStartTime = DateTime.Now;
        _gamePaused = true;
        
        PausePlayerTimer();
    }

    private void OnUnPause()
    {
        if (!_timerStarted) return;

        _gamePaused = false;

        TimeSpan timeout = DateTime.Now - _pauseStartTime;

        _totalTimeoutTime += timeout;

        if (ChuckPanelController.current.timerActive) return;
        UnpausePlayerTimer();
    }
    
    private void PausePlayerTimer()
    {
        _playerPauseStartTime = DateTime.Now;
        _playerTimePaused = true;
    }

    private void UnpausePlayerTimer()
    {
        _playerTimePaused = false;
        TimeSpan ts = DateTime.Now - _playerPauseStartTime;
        _totalTimeoutTimePlayer += ts;
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
    
    /// <summary>
    /// Returns next player in line, and progress the round if all players have drawn a card
    /// </summary>
    /// <returns>Next Player</returns>
    private Player GetNextPlayerInLine()
    {
        if (_currentPlayerIndex >= _players.Count)
        {
            _currentPlayerIndex = 0;
            _roundCounter++;
            
            //Check if we need a new row
            Table.current.AddRow(_roundCounter);
        }

        _currentPlayer = _players[_currentPlayerIndex];
        
        //finding next player
        int nextPlayerIndex = _currentPlayerIndex + 1;
        if (nextPlayerIndex >= _players.Count) nextPlayerIndex = 0;
        _nextPlayer = _players[nextPlayerIndex];

        //Finding Previous Player
        int previousPlayerIndex = _currentPlayerIndex - 1;
        if (previousPlayerIndex < 0) previousPlayerIndex = _players.Count-1;
        _previousPlayer = _players[previousPlayerIndex];

        _currentPlayerIndex++;

        //Debug.Log($"Current: {_currentPlayer.name} | Next: {_nextPlayer.name} | Prev: {_previousPlayer.name}");
        
        return _currentPlayer;
    }

    public List<Player> GetAllPlayers()
    {
        return _players;
    }
    
    private Card GetRandomCard(bool playerProtected)
    {
        int randomCardNumber = Random.Range(0, _cards.Count);

        Card randomCard = _cards[randomCardNumber];

        if (playerProtected && randomCard.rank == 14 && AvoidAcePossible())
        {
            randomCardNumber = ReturnIndexOfNonAceCard();
            randomCard = _cards[randomCardNumber];
        }
        
        _cards.RemoveAt(randomCardNumber);

        CardDisplay randomCardDisplay = randomCard.cardObj.GetComponent<CardDisplay>();
        randomCardDisplay.SetSuit(randomCard.suit);
        randomCardDisplay.SetRank(randomCard.rank);

        return randomCard;
    }

    private bool AvoidAcePossible()
    {
        foreach (Card card in _cards)
        {
            if (card.rank != 14) return true;
        }
        return false;
    }

    private int ReturnIndexOfNonAceCard()
    {
        List<int> nonAceCardIndexes = new List<int>();

        int i = 0;
        foreach (Card card in _cards)
        {
            if (card.rank != 14)
            {
                nonAceCardIndexes.Add(i);
            }

            i++;
        }

        return Random.Range(0, nonAceCardIndexes.Count);
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

    public List<float> GetChuckTimes()
    {
        List<float> times = new List<float>();

        foreach (Player player in _players)
        {
            foreach (float time in player.GetChuckTimes())
            {
                times.Add(time);
            }
        }

        return times;
    }
    
    /*
     * Event Invokers
     */
    private void OnSpacePressed()
    {
        SpacePressed?.Invoke();
    }

    public void NextCardKeyPressed()
    {
        Debug.Log("Key pressed");
        
        if (_gameOver || ChuckPanelController.current.timerActive || PausePanelController.current.panelActive) return;
        
        if (!_timerStarted)
        {
            _startTime = DateTime.Now;
            _timerStarted = true;
        }
        _playerStartTime = DateTime.Now;
        _totalTimeoutTimePlayer = TimeSpan.FromMinutes(0);

        //Notifying listeners
        OnSpacePressed();
            
        //progress game
        ShowNextCard();
    }

    public IEnumerator EndGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        StatTracker.statTracker.EndLog(ElapsedGameTime,_players);
        GlobalValueContainer.Container.PlayerResults = _players;
        SceneManager.LoadScene(2);
    }


    private void OnApplicationQuit()
    {
        StatTracker.statTracker.EndLog(ElapsedGameTime,_players);
    }
}
