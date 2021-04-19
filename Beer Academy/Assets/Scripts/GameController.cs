using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{ 
    public GameObject currentCardPos;
    public GameObject nextCardPos;
    public GameObject newCard;
    
    private Dictionary<int,TextMeshProUGUI> _cardCounters;
    
    private void Start()
    {
        //usedCard.SetActive(false);
        //currentCard.SetActive(false);
        
        _cardCounters = GetCardCounters();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject nCard = Instantiate(newCard, 
                nextCardPos.transform.position, 
                nextCardPos.transform.rotation,
                nextCardPos.transform.parent);
            
            nCard.GetComponent<CardDisplay>().SetCardActive(false);
            
            RectTransform nCardRect = nCard.GetComponent<RectTransform>();
            RectTransform currentCardRect = nextCardPos.GetComponent<RectTransform>();
            
            nCardRect.anchorMin = currentCardRect.anchorMin;
            nCardRect.anchorMax = currentCardRect.anchorMax;
            nCardRect.anchoredPosition = currentCardRect.anchoredPosition;
            nCardRect.sizeDelta = currentCardRect.sizeDelta;


            nCard.transform.SetAsFirstSibling();
            
            LeanTween.move(nextCardPos, currentCardPos.transform.position, 1f).setEase(LeanTweenType.easeInOutQuad);
            
            nextCardPos.GetComponent<CardDisplay>().TurnCard();
            
            Vector2 xyStart = currentCardRect.sizeDelta;
            Vector2 xyEnd = currentCardPos.GetComponent<RectTransform>().sizeDelta;

            float aspect = xyStart.y / xyStart.x;

            LeanTween.value(nextCardPos, a =>
            {
                currentCardRect.sizeDelta = new Vector2(a, a*aspect);

            }, xyStart.x, xyEnd.x, 1f).setEase(LeanTweenType.easeInOutQuad);
        }
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

    /// <summary>
    /// Sets the counter for all cards. I imagine this will be mostly used to start the game
    /// </summary>
    /// <param name="newValue"></param>
    private void SetAllCounters(int newValue)
    {
        foreach (KeyValuePair<int,TextMeshProUGUI> counter in _cardCounters)
        {
            counter.Value.text = newValue.ToString();
        }
    }
}
