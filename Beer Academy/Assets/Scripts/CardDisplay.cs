using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardDisplay : MonoBehaviour
{
    [SerializeField][HideInInspector] private Suit suit;
    [SerializeField][HideInInspector] private int rank;

    //Card Rotation
    public GameObject cardBack;
    [HideInInspector] public bool isCardFacingDown;
    
    //Card Elements
    public List<Image> suits;
    public List<TextMeshProUGUI> ranks;
    
    private void Awake()
    {
        FindCardElements(transform);
    }
    
    
    /// <summary>
    /// Animated the card rotating 180 around it's y-axis
    /// </summary>
    public void TurnCard(float turnTime)
    {
        isCardFacingDown = !isCardFacingDown;
        LeanTween.rotateLocal(gameObject, new Vector3(0, 90, 0), turnTime/2).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
        {
            cardBack.SetActive(isCardFacingDown);
            LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), turnTime / 2).setEase(LeanTweenType.easeOutQuad);
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
    
    /// <summary>
    /// Changes the suit of the card and in turn, changes the rank color to match the suit
    /// </summary>
    /// <param name="s"></param>
    public void Initialize()
    {
        foreach (Image image in suits)
        {
            image.sprite = MyResources.current.GetSuitSprite(suit);
        }
        
        foreach (TextMeshProUGUI rank in ranks)
        {
            rank.color = MyResources.current.GetSuitColor(suit);
        }
    }

    public void SetSuit(Suit newSuit)
    {
        this.suit = newSuit;
    }

    public Suit GetSuit()
    {
        return suit;
    }

    public void SetRank(int newRank)
    {
        this.rank = newRank;
    }

    public int GetRank()
    {
        return rank;
    }
    
    /// <summary>
    /// Iterates though the elements making up the card and saves elements marked with certain tags
    /// </summary>
    /// <param name="t"></param>
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

    /// <summary>
    /// Empties the list of suits and ranks
    /// </summary>
    public void CleanList()
    {
        suits.Clear();
        ranks.Clear();
    }
}

#region Custom Card Editor
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
#endregion


