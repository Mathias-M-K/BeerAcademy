using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResultCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerSips;
    [SerializeField] private TextMeshProUGUI playerAvgSips;
    [SerializeField] private TextMeshProUGUI playerBeers;
    [SerializeField] private TextMeshProUGUI playerTime;
    [SerializeField] private TextMeshProUGUI playerAvgTime;
    [SerializeField] private Image background;

    public void SetInfo(Player player)
    {
        playerName.text = player.Name;
        playerSips.text = $"Sips : {player.Sips}";
        playerAvgSips.text = $"avg sips : {player.AvgSips:.00}";        
        playerBeers.text = $"Beers : {player.Beers:.00}";
        playerTime.text = $"Total Time: {player.TimeTotal.Hours:00}:{player.TimeTotal.Minutes:00}:{player.TimeTotal.Seconds:00}:{player.TimeTotal.Milliseconds:000}";
        playerAvgTime.text = $"Avg Time: {player.AvgTime.Hours:00}:{player.AvgTime.Minutes:00}:{player.AvgTime.Seconds:00}:{player.AvgTime.Milliseconds:000}";

        background.color = player.Color;
    }
}
