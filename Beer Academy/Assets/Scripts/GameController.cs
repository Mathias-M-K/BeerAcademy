using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public GameObject showCard;
    public GameObject otherCard;


    private Dictionary<int,TextMeshProUGUI> _cardCounters;


    private void Start()
    {
        _cardCounters = GetCardCounters();

        SetCounter(2,6);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            /*
            GameObject go = Instantiate(otherCard, showCard.transform.position, showCard.transform.rotation,showCard.transform.parent);
            go.GetComponent<RectTransform>().sizeDelta = showCard.GetComponent<RectTransform>().sizeDelta;
            Destroy(showCard);
            */
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

    private void SetAllCounters(int newValue)
    {
        foreach (KeyValuePair<int,TextMeshProUGUI> counter in _cardCounters)
        {
            counter.Value.text = newValue.ToString();
        }
    }
}
