using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    private Suit _suit;
    private int _currentSuit = 0;
    
    [HideInInspector] public List<Image> suits;
    [HideInInspector] public List<TextMeshProUGUI> ranks;

    private void Start()
    {
        FindCardElements(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Suit s = (Suit) _currentSuit;
            SetSuit(s);

            _currentSuit++;

            _currentSuit = _currentSuit == Enum.GetValues(typeof(Suit)).Length ? 0 : _currentSuit;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            
        }
    }

    private void SetSuit(Suit s)
    {
        foreach (Image image in suits)
        {
            image.sprite = MyResources.Current.GetSprite(s);
        }
        
        foreach (TextMeshProUGUI rank in ranks)
        {
            rank.color = MyResources.Current.GetSuitColor(s);
        }
        
    }
    
    private void FindCardElements(Transform t)
    {
        if (t.CompareTag("Suit"))
        {
            suits.Add(t.gameObject.GetComponent<Image>());
        }

        if (t.CompareTag("Rank"))
        {
            ranks.Add(t.gameObject.GetComponent<TextMeshProUGUI>());
        }

        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                FindCardElements(child);
            }
        }
    }
}


