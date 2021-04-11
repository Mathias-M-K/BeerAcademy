using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class MyResources : MonoBehaviour
{
    public static MyResources Current;
    private void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        
        InitializeResources();
    }

    private readonly Dictionary<Suit, Sprite> _suits = new Dictionary<Suit, Sprite>();
    
    private void InitializeResources()
    {
        var sprites = Resources.LoadAll("Suits", typeof(Sprite));

        foreach (Object sprite in sprites)
        {
            Debug.Log(sprite.name);

            string enumString = sprite.name.Split('_')[1];
            
            Suit parsedEnum = (Suit) Enum.Parse( typeof(Suit), enumString );
            
            _suits.Add(parsedEnum,(Sprite) sprite);
        }
    }

    public Sprite GetSprite(Suit s)
    {
        return _suits[s];
    }
}
