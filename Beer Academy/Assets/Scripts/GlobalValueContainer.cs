using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using UnityEngine;

public class GlobalValueContainer : MonoBehaviour
{
    public static GlobalValueContainer Container;
    public List<string> players;
    public List<int> playerSips;
    public List<bool> playerProtectedStatus;
    public List<Player> PlayerResults;

    private void Awake()
    {
        if (Container == null) Container = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
