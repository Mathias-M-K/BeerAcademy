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
    
    /// <summary>
    /// Method for fething resources from the resources folder and making them accessible globally.
    /// </summary>
    private void InitializeResources()
    {
        var sprites = Resources.LoadAll("Suits", typeof(Sprite));

        
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
    }

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

            if (c.a == 1)
            {
                return c;
            }
        }

        throw new Exception("Couldn't find color");
    }

    public Sprite GetSprite(Suit s)
    {
        return _suits[s];
    }

    public Color GetSuitColor(Suit s)
    {
        return _suitColors[s];
    }
}

public enum Suit
{
    Club,
    Heart,
    Spade,
    Diamond,
    Penis
}
