using UnityEngine;

public class Player
{
    public int id;
    public string name;
    public Color32 color;
    public int sips = 0;


    public Player(int id, string name, Color32 color)
    {
        this.id = id;
        this.name = name;
        this.color = color;
    }
}
