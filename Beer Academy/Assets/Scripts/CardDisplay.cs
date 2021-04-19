using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardDisplay : MonoBehaviour
{
    
    private Suit _suit;
    private int _currentSuit = 0;
    
    //Card Rotation
    public GameObject cardBack;
    [HideInInspector] public bool isCardFacingDown;
    
    public List<Image> suits;
    public List<TextMeshProUGUI> ranks;
    
    private void Awake()
    {
        FindCardElements(transform);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Suit s = (Suit) _currentSuit;
            SetSuit(s);

            _currentSuit++;

            _currentSuit = _currentSuit == Enum.GetValues(typeof(Suit)).Length ? 0 : _currentSuit;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TurnCard();
        }*/
    }
    
    /// <summary>
    /// Animated the card rotating 180 around it's y-axis
    /// </summary>
    public void TurnCard()
    {
        isCardFacingDown = !isCardFacingDown;
        LeanTween.rotateLocal(gameObject, new Vector3(0, 90, 0), 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
        {
            Debug.Log("Half way!");
            cardBack.SetActive(isCardFacingDown);
            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                Debug.Log("Done!");
            });
        });
    }

    /// <summary>
    /// Instantly flips the card in the desired orientation.
    /// </summary>
    /// <param name="cardActive"></param>
    public void SetCardActive(bool cardActive)
    {
        isCardFacingDown = !cardActive;
        cardBack.SetActive(!cardActive);
    }
    private void SetSuit(Suit s)
    {
        foreach (Image image in suits)
        {
            image.sprite = MyResources.current.GetSprite(s);
        }
        
        foreach (TextMeshProUGUI rank in ranks)
        {
            rank.color = MyResources.current.GetSuitColor(s);
        }
        
    }
    
    public void FindCardElements(Transform t)
    {
        if (t.CompareTag("Suit"))
        {
            suits.Add(t.gameObject.GetComponent<Image>());
        }

        if (t.CompareTag("Rank"))
        {
            ranks.Add(t.gameObject.GetComponent<TextMeshProUGUI>());
        }
        
        if (t.CompareTag("CardBack"))
        {
            cardBack = t.gameObject;
        }
        
        

        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                FindCardElements(child);
            }
        }
    }

    public void CleanList()
    {
        suits.Clear();
        ranks.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CardDisplay))]
public class CardEditor : Editor
{
    private SerializedProperty _value;
    
    private void OnEnable()
    {
        // Link the SerializedProperty to the variable 
        _value = serializedObject.FindProperty("isCardFacingDown");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        base.DrawDefaultInspector();
        
        CardDisplay cardDisplay = (CardDisplay) target;

        if (GUILayout.Button("Get Card Elements"))
        {
            cardDisplay.CleanList();
            cardDisplay.FindCardElements(cardDisplay.gameObject.transform);
        }
        
        if (GUILayout.Button("Flip Card"))
        {
            cardDisplay.CleanList();
            cardDisplay.FindCardElements(cardDisplay.gameObject.transform);
            _value.boolValue = !_value.boolValue;
            cardDisplay.cardBack.SetActive(_value.boolValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif


