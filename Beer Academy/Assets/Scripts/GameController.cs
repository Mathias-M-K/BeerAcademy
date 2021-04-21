using System;
using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameController : MonoBehaviour
{
    [Header("Game Settings")] 
    public int numberOfPlayers;
    
    [Header("Card Flipping Elements")]
    public GameObject currentCardPos;
    public GameObject nextCardPos;
    public GameObject nextCard;

    //UI
    private Dictionary<int,TextMeshProUGUI> _cardCounters;
    private readonly Dictionary<int, int> _cardCounterValues = new Dictionary<int, int>();
    
    //Game Functionality
    private readonly List<Card> _cards = new List<Card>();
    private bool _gameOver;
    
    private void Start()
    {
        nextCardPos.SetActive(false);
        currentCardPos.SetActive(false);
        
        _cardCounters = GetCardCounters();
        
        SetUp();

        nextCard = SpawnCard(GetRandomCard());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_gameOver)
        {
            ShowNextCard();
        }
    }

    private void SetUp()
    {
        for (int i = 2; i <= 14; i++)
        {
            for (int y = 0; y < numberOfPlayers; y++)
            {
                _cards.Add(new Card(i, (Suit)y));
            }
        }

        SetAllCounters(numberOfPlayers);
        
        foreach (Card card in _cards)
        {
            Debug.Log($"Rank: {card.rank} | Suit: {card.suit}");
        }
    }

    private void ShowNextCard()
    {
        GameObject newNextCard = null;
        if (_cards.Count > 0)
        {
            newNextCard = SpawnCard(GetRandomCard());
        }
        else
        {
            _gameOver = true;
        }
        

        LeanTween.move(nextCard, currentCardPos.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);


        CardDisplay cd = nextCard.GetComponent<CardDisplay>();
        cd.TurnCard(0.5f);

        SubtractOneFromCounter(cd.GetRank());
        
        
        /*
        LeanTween.value(nextCard, a =>
        {
            nextCardRect.sizeDelta = new Vector2(a, a*aspect);

        }, xyStart.x, xyEnd.x, 1f).setEase(LeanTweenType.easeInOutQuad);
        */
        
        nextCard.transform.SetAsLastSibling();
        nextCard = newNextCard;
    }

    private GameObject SpawnCard(GameObject cardToSpawn)
    {
        GameObject newNextCard = Instantiate(cardToSpawn, 
            nextCardPos.transform.position, 
            nextCardPos.transform.rotation,
            nextCardPos.transform.parent);

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
        
        return newNextCard;
    }

    private GameObject GetRandomCard()
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
