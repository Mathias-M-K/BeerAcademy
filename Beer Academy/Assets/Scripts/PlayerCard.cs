using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

#endif
public class PlayerCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image currentBeerProgress;

    [Header("Card Elements")] [SerializeField]
    private TextMeshProUGUI playerName;

    [SerializeField] private TextMeshProUGUI nrOfSips;
    [SerializeField] private TextMeshProUGUI avgSips;
    [SerializeField] private TextMeshProUGUI beers;
    [SerializeField] private TextMeshProUGUI lastTime;
    [SerializeField] private TextMeshProUGUI avgTime;

    private Image _background;

    [Header("Templates")] 
    public GameObject beerCounterElement;
    public GameObject beerCounterContainer;

    [Header("Animation")] 
    public float animationSpeed = 0.5f;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    private int _beersAdded = 0;
    private Player _player;

    private void Awake()
    {
        _background = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        GUIEvents.current.MouseHover += OnMouseHover;
        GUIEvents.current.MouseExit += OnMouseExit;
    }


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
        _player = player;

        playerName.text = _player.name;
        nrOfSips.text = _player.sips.ToString("00");
        avgSips.text = _player.avgSips.ToString("F");
        beers.text = _player.beers.ToString("F1");
        lastTime.text = _player.lastTime.ToString();
        avgTime.text = _player.avgTime.ToString();

        SetColor(_player.color);

        //Adding beers if the player have finished a beer
        if (Mathf.Floor(_player.beers) > _beersAdded)
        {
            int beerDeficit = (int) (Mathf.Floor(_player.beers) - _beersAdded);

            for (int i = 0; i < beerDeficit; i++)
            {
                AddBeer();
            }
        }

        //Updating the beerSipGraphic
        float sipsInABeer = GameController.current.sipsInABeer;

        float remainingSips = _player.sips % sipsInABeer;
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

        currentBeerProgress.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            remainingSips.ToString("0");
    }

    private void SetColor(Color32 color)
    {
        _background.color = color;
        currentBeerProgress.transform.parent.GetComponent<Image>().color = color;
    }

    private void SetColor(Color32 color, float time)
    {
        LeanTween.color(gameObject.GetComponent<Image>().rectTransform, color, time).setEase(LeanTweenType.easeInOutQuad).setRecursive(false);
        
        //Setting color of the beer-counter
        Image beerCounter = currentBeerProgress.transform.parent.GetComponent<Image>();
        LeanTween.color(beerCounter.rectTransform, color, time).setEase(LeanTweenType.easeInOutQuad).setRecursive(false);
    }


    private void OnMouseExit(Player player)
    {
        SetColor(_player.color, 0.25f);
    }

    private void OnMouseHover(Player obj)
    {
        if (obj != _player)
        {
            SetColor(Color.gray, 0.25f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GUIEvents.current.OnMouseHover(_player);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GUIEvents.current.OnMouseExit(_player);
    }
}