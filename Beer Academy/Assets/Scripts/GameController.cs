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


    public GameObject[] _cardCounters;


    private void Start()
    {
        _cardCounters = GetCardCounters();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = Instantiate(otherCard, showCard.transform.position, showCard.transform.rotation,showCard.transform.parent);
            go.GetComponent<RectTransform>().sizeDelta = showCard.GetComponent<RectTransform>().sizeDelta;
            Destroy(showCard);
        }
    }

    private GameObject[] GetCardCounters()
    {
        return GameObject.FindGameObjectsWithTag("CardCounter");
    }
}
