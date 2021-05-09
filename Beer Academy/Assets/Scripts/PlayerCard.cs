using System;
using System.Net.Mime;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

#endif
public class PlayerCard : MonoBehaviour
{
    public Image currentBeerProgress;

    [Header("Card Elements")]
    [SerializeField]private TextMeshProUGUI playerName;
    [SerializeField]private TextMeshProUGUI nrOfSips;
    [SerializeField]private TextMeshProUGUI avgSips;
    [SerializeField]private TextMeshProUGUI beers;
    [SerializeField]private TextMeshProUGUI lastTime;
    [SerializeField]private TextMeshProUGUI avgTime;
    
    [Header("Templates")] 
    public GameObject beerCounterElement;
    public GameObject beerCounterContainer;

    [Header("Animation")] 
    public float animationSpeed = 0.5f;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;
    
    private int _beersAdded = 0;
    
    private void AddBeer()
    {
        GameObject go = Instantiate(beerCounterElement, beerCounterContainer.transform);
        
        RectTransform beerRect = go.transform.GetChild(0).GetComponent<RectTransform>();

        beerRect.localPosition = new Vector2(beerRect.localPosition.x, -30);
        
        LeanTween.moveLocalY(beerRect.gameObject, 0, animationSpeed).setEase(easeType);

        _beersAdded++;
    }

    public void SetInfo(Player player)
    {
        playerName.text = player.name;
        nrOfSips.text = player.sips.ToString("00");
        avgSips.text = player.avgSips.ToString("F");
        beers.text = player.beers.ToString("F1");
        lastTime.text = player.lastTime.ToString();
        avgTime.text = player.avgTime.ToString();
        
        SetColor(player.color);
        
        //Adding beers if the player have finished a beer
        if (Mathf.Floor(player.beers) > _beersAdded)
        {
            int beerDeficit = (int)(Mathf.Floor(player.beers) - _beersAdded);

            for (int i = 0; i < beerDeficit; i++)
            {
                AddBeer();
            }
        }
        
        //Updating the beerSipGraphic
        float sipsInABeer = GameController.current.sipsInABeer;
        
        float remainingSips = player.sips % sipsInABeer;
        remainingSips = sipsInABeer - remainingSips;
        remainingSips = Mathf.Ceil(remainingSips);

        UpdateRemainingSipsDisplay(remainingSips, 0.5f);
    }

    private void UpdateRemainingSipsDisplay(float remainingSips, float animationTime)
    {
        float newFillAmount = MyResources.current.Remap(remainingSips, 0, GameController.current.sipsInABeer, 0, 1);
        LeanTween.value(currentBeerProgress.fillAmount, newFillAmount, animationTime).setOnUpdate((f, o) =>
        {
            currentBeerProgress.fillAmount = f;
        }).setEase(LeanTweenType.easeInOutQuad);
        
        currentBeerProgress.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = remainingSips.ToString("0");
    }
    
    private void SetColor(Color32 color)
    {
        GetComponent<Image>().color = color;
        playerName.transform.parent.GetComponent<Image>().color = color;
        currentBeerProgress.transform.parent.GetComponent<Image>().color = color;
    }
}

