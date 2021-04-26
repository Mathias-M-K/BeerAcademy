using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class MyResources : MonoBehaviour
{
    public static MyResources current;
    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        
        InitializeResources();
    }
    
    

    private readonly Dictionary<Suit, Sprite> _suits = new Dictionary<Suit, Sprite>();
    private readonly Dictionary<Suit, Color> _suitColors = new Dictionary<Suit, Color>();
    private Dictionary<int, GameObject> _cards = new Dictionary<int, GameObject>();
    
    /// <summary>
    /// Method for fetching resources from the resources folder and making them accessible globally.
    /// </summary>
    private void InitializeResources()
    {
        //Fetching all suit sprites
        var sprites = Resources.LoadAll("Suits", typeof(Sprite));
        
        //We need the sprite renderer to get the pixel data, so we can determine the color of the suit (because i dont wanna save the colors manually)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        
        foreach (Object sprite in sprites)
        {
            string enumString = sprite.name.Split('_')[1];
            
            //Parsing the string as an enum of type Suit
            Suit parsedEnum = (Suit) Enum.Parse( typeof(Suit), enumString );
            
            //Adding the sprite with it's enum counterpart in a key,pair set, so that it is accessible globally
            _suits.Add(parsedEnum,(Sprite) sprite);

            //Using to attached sprite renderer it to extract pixel-data from sprite
            sr.sprite = (Sprite) sprite;
            Color[] data = sr.sprite.texture.GetPixels();
            
            //Getting the suit color and adding it to the dictionary
            Color suitColor = FindSpriteColor(data);
            _suitColors.Add(parsedEnum,suitColor);
        }
        
        //Removing the sprite renderer, cuz we don't need that
        Destroy(sr);
        
        var cards = Resources.LoadAll("Cards_Big");

        foreach (Object card in cards)
        {
            _cards.Add(Int32.Parse(card.name.Split('_')[2]), (GameObject) card);
        }
    }

    #region Get Methods
    
    /// <summary>
    /// Returns the sprite associated with a given suit
    /// </summary>
    /// <param name="suit"></param>
    /// <returns></returns>
    public Sprite GetSuitSprite(Suit suit)
    {
        return _suits[suit];
    }

    /// <summary>
    /// Returns a gameobject of a card
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public GameObject GetPlayingCard(int card)
    {
        return _cards[card];
    }

    /// <summary>
    /// //Returns all playing cards in a dictionary, where the key is the card number (eg. 2 will return playing card two, and 11 will return the jack)
    /// </summary>
    /// <returns></returns>
    public Dictionary<int,GameObject> GetAllPlayingCards()
    {
        return _cards;
    }

    /// <summary>
    /// Returns the general color of the suit
    /// </summary>
    /// <param name="suit"></param>
    /// <returns></returns>
    public Color GetSuitColor(Suit suit)
    {
        return _suitColors[suit];
    }
    
    #endregion
    
    #region Support Methods
    
    /// <summary>
    /// Checks every 1000 pixel, and returns the first color it finds where the albedo value is one (meaning that it is not transparent)
    /// </summary>
    /// <param name="pixelData"></param>
    /// <returns>The Primary Color of the Sprite</returns>
    /// <exception cref="Exception"></exception>
    private Color FindSpriteColor(Color[] pixelData)
    {
        for (int i = 0; i < pixelData.Length; i += 1000)
        {
            Color c = pixelData[i];

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (c.a == 1)
            {
                return c;
            }
        }

        throw new Exception("Couldn't find color");
    }
    
    public float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #endregion
}

public enum Suit
{
    Club,
    Heart,
    Spade,
    Diamond,
    Penis
}
