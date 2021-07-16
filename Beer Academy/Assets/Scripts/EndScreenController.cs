using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data_Types;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    public Transform playerResultCardContainer;
    public GameObject playerResultCard;
    public GridLayoutGroup grid;

    [Header("Elements")] 
    public GameObject playerResultCards;
    public GameObject playerTopScores;

    [Header("Beer Scoreboard")] 
    public TextMeshProUGUI beerFirstPlaceName;
    public TextMeshProUGUI beerFirstPlaceCount;
    public TextMeshProUGUI beerSecondPlace;
    public TextMeshProUGUI beerThirdPlace;
    
    [Header("Avg Time Scoreboard")] 
    public TextMeshProUGUI avgTimeFirstPlaceName;
    public TextMeshProUGUI avgTimeFirstPlaceTime;
    public TextMeshProUGUI avgTimeSecondPlace;
    public TextMeshProUGUI avgTimeThirdPlace;
    
    [Header("Chuck Time Scoreboard")] 
    public TextMeshProUGUI chuckTimeFirstPlaceName;
    public TextMeshProUGUI chuckTimeFirstPlaceTime;
    public TextMeshProUGUI chuckTimeSecondPlace;
    public TextMeshProUGUI chuckTimeThirdPlace;

    [Header("Buttons")] 
    public ButtonManager nextAndBackButton;
    public GameObject quitBtn;

    
    //Values
    private bool _nextPressed = false;
    private void Start()
    {
        List<Player> playerResults = GlobalValueContainer.Container.PlayerResults;

        if (playerResults.Count > 5)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 2;
        }
        else
        {
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
        }
        
        foreach (Player playerResult in playerResults)
        {
            GameObject go = Instantiate(playerResultCard, playerResultCardContainer);
            go.GetComponent<PlayerResultCard>().SetInfo(playerResult);
        }
        
        SetTopBeerScores();
        SetAvgTimeTopScores();
        SetChuckTimeTopScores();

    }


    private void SetTopBeerScores()
    {
        List<Player> players = GlobalValueContainer.Container.PlayerResults;
        
        players = players.OrderByDescending(p => p.Beers).ToList();

        beerFirstPlaceName.text = players[0].Name;
        beerFirstPlaceName.color = players[0].Color;
        
        beerFirstPlaceCount.text = $"{players[0].Beers:.000}";
        beerFirstPlaceCount.color = players[0].Color;
        
        beerSecondPlace.text = $"{players[1].Name} : {players[1].Beers:.000}";
        beerSecondPlace.color = players[1].Color;

        if (players.Count > 2)
        {
            beerThirdPlace.text = $"{players[2].Name} : {players[2].Beers:.000}";
            beerThirdPlace.color = players[2].Color;
        }
        else
        {
            beerThirdPlace.text = "";
        }
    }

    private void SetAvgTimeTopScores()
    {
        List<Player> players = GlobalValueContainer.Container.PlayerResults;
        
        players = players.OrderBy(p => p.AvgTime.TotalMilliseconds).ToList();
        
        avgTimeFirstPlaceName.text = players[0].Name;
        avgTimeFirstPlaceName.color = players[0].Color;
        
        avgTimeFirstPlaceTime.text = $"{players[0].AvgTime.Minutes:00}:{players[0].AvgTime.Seconds:00}:{players[0].AvgTime.Milliseconds:000}";
        avgTimeFirstPlaceTime.color = players[0].Color;
        
        avgTimeSecondPlace.text = $"{players[1].Name} : {players[1].AvgTime.Minutes:00}:{players[1].AvgTime.Seconds:00}:{players[1].AvgTime.Milliseconds:000}";
        avgTimeSecondPlace.color = players[1].Color;

        if (players.Count > 2)
        {
            avgTimeThirdPlace.text = $"{players[2].Name} : {players[2].AvgTime.Minutes:00}:{players[2].AvgTime.Seconds:00}:{players[2].AvgTime.Milliseconds:000}";
            avgTimeThirdPlace.color = players[2].Color;
        }
        else
        {
            avgTimeThirdPlace.text = "";
        }
    }
    
    private void SetChuckTimeTopScores()
    {
        List<Player> players = GlobalValueContainer.Container.PlayerResults;
        
        players = players.OrderBy(p => p.GetFastestChuckTime()).ToList();

        TimeSpan fastestChuckTime = TimeSpan.FromMilliseconds(players[0].GetFastestChuckTime());
        chuckTimeFirstPlaceName.text = players[0].Name;
        chuckTimeFirstPlaceTime.text = $"{fastestChuckTime.Minutes:00}:{fastestChuckTime.Seconds:00}:{fastestChuckTime.Milliseconds:000}";
        
        chuckTimeFirstPlaceName.color =players[0].Color;
        chuckTimeFirstPlaceTime.color =players[0].Color;
        
        TimeSpan secondFastestChuckTime = TimeSpan.FromMilliseconds(players[1].GetFastestChuckTime());
        chuckTimeSecondPlace.text = $"{players[1].Name} : {secondFastestChuckTime.Minutes:00}:{secondFastestChuckTime.Seconds:00}:{secondFastestChuckTime.Milliseconds:000}";
        chuckTimeSecondPlace.color =players[1].Color;


        if (players.Count > 2)
        {
            TimeSpan thirdFastestChuckTime = TimeSpan.FromMilliseconds(players[2].GetFastestChuckTime());
            
            chuckTimeThirdPlace.text =
                $"{players[2].Name} : {thirdFastestChuckTime.Minutes:00}:{thirdFastestChuckTime.Seconds:00}:{thirdFastestChuckTime.Milliseconds:000}";

            chuckTimeThirdPlace.color = players[2].Color;
        }
        else
        {
            chuckTimeThirdPlace.text = "";
        }
    }

    public void NextBtnPressed()
    {
        if (!_nextPressed)
        {
            LeanTween.moveLocalX(playerTopScores, 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalX(playerResultCards, -2500, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            nextAndBackButton.normalText.text = "Back";
            nextAndBackButton.highlightedText.text = "Back";
            LeanTween.moveLocalY(quitBtn, -660, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            _nextPressed = true;
        }
        else
        {
            LeanTween.moveLocalX(playerTopScores, 2500, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalX(playerResultCards, 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            nextAndBackButton.normalText.text = "Next";
            nextAndBackButton.highlightedText.text = "Next";
            LeanTween.moveLocalY(quitBtn, -800, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            _nextPressed = false;
        }
    }

    public void QuitBtn()
    {
        Application.Quit();
    }
    
}
