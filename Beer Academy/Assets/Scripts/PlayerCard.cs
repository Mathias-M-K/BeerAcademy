using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image currentBeerProgress;

    [Header("Card Elements")] 
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI nrOfSips;
    [SerializeField] private TextMeshProUGUI avgSips;
    [SerializeField] private TextMeshProUGUI beers;
    [SerializeField] private TextMeshProUGUI lastTime;
    [SerializeField] private TextMeshProUGUI avgTime;

    private Image _background;
    private Image _selector;
    private RectTransform _selectorContainer;

    [Header("Templates")] 
    public GameObject beerCounterElement;
    public GameObject beerCounterContainer;

    [Header("Animation")] 
    public float animationSpeed = 0.5f;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    private int _beersAdded = 0;
    private Player _player;
    
    //Selected
    private bool _isSelected;

    private void Awake()
    {
        _background = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        LeanTween.init( 800 );
        _selectorContainer = playerName.transform.parent.GetChild(1).GetComponent<RectTransform>();
        _selector = _selectorContainer.GetChild(0).GetComponent<Image>();

        HideSelectorButton();

        GUIEvents.current.SetSelected += OnSetSelected;
        GUIEvents.current.SetDeselected += OnSetDeselected;
        GUIEvents.current.ClearSelection += OnSetSelected;
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

        playerName.text = _player.Name;
        nrOfSips.text = _player.Sips.ToString("00");
        avgSips.text = _player.AvgSips.ToString("F");
        beers.text = _player.Beers.ToString("F1");
        lastTime.text = $"{_player.LastTime.Minutes:00}:{_player.LastTime.Seconds:00}";
        avgTime.text = $"{_player.AvgTime.Minutes:00}:{_player.AvgTime.Seconds:00}";

        //SetColor(_player.color);

        //Adding beers if the player have finished a beer
        if (Mathf.Floor(_player.Beers) > _beersAdded)
        {
            int beerDeficit = (int) (Mathf.Floor(_player.Beers) - _beersAdded);

            for (int i = 0; i < beerDeficit; i++)
            {
                AddBeer();
            }
        }

        //Updating the beerSipGraphic
        float sipsInABeer = player.SipsInABeer;

        float remainingSips = _player.Sips % sipsInABeer;
        remainingSips = sipsInABeer - remainingSips;
        remainingSips = Mathf.Ceil(remainingSips);

        UpdateRemainingSipsDisplay(sipsInABeer, remainingSips, 0.5f);
    }

    private void UpdateRemainingSipsDisplay(float sipsInABeer,float remainingSips, float animationTime)
    {
        float newFillAmount = MyResources.current.Remap(remainingSips, 0, sipsInABeer, 0, 1);
        LeanTween.value(currentBeerProgress.fillAmount, newFillAmount, animationTime).setOnUpdate((f, o) =>
        {
            currentBeerProgress.fillAmount = f;
        }).setEase(LeanTweenType.easeInOutQuad);

        currentBeerProgress.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            remainingSips.ToString("0");
    }

    public void SetColor(Color32 color)
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

    private void ShowSelectorButton()
    {
        LeanTween.moveLocalY(_selectorContainer.gameObject, 0, 0.25f).setEase(LeanTweenType.easeInOutQuad);
    }

    private void HideSelectorButton()
    {
        _selector.color = new Color32(255,255,255,51);
        LeanTween.moveLocalY(_selectorContainer.gameObject, 30, 0.25f).setEase(LeanTweenType.easeInOutQuad);
    }


    private void OnSetDeselected(List<Player> players)
    {
        if (players.Contains(_player))
        {
            //SetColor(Color.gray, 0.25f);

            Color32 mutedColor = _player.Color;
            mutedColor.a = 60;
            
            SetColor(mutedColor,0.25f);

            _isSelected = false;
            HideSelectorButton();
        }
    }

    private void OnSetSelected(List<Player> players)
    {
        if (players.Contains(_player))
        {
            SetColor(_player.Color, 0.25f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GUIEvents.current.OnMouseHover(_player);
        
        ShowSelectorButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GUIEvents.current.OnMouseExit();
        
        if (!_isSelected) HideSelectorButton();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            GUIEvents.current.OnMouseClick(_player,true);
            
            if (_isSelected)
            {
                HideSelectorButton();
                _isSelected = false;
            }
            else
            {
                ShowSelectorButton();
                _isSelected = true;
                _selector.color = new Color32(49, 119, 152, 255);
            }
            
        }
        else
        {
            GUIEvents.current.OnMouseClick(_player,false);
            HideSelectorButton();
            _isSelected = false;
        }
        

    }

    private void OnSelectorButton()
    {
        if (_isSelected)
        {
            HideSelectorButton();
            _isSelected = false;
        }
        else
        {
            ShowSelectorButton();
            _isSelected = true;
            _selector.color = new Color32(49, 119, 152, 255);
        }
        
        GUIEvents.current.OnMouseClick(_player,true);
    }
}