using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data_Types;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatTracker : MonoBehaviour
{
    public static StatTracker statTracker;

    private void Awake()
    {
        if (statTracker == null) statTracker = this;
    }

    //private string _directoryPath = "../GameLogs/";
    private string _directoryPath = "C:/GameLogs/";
    private TextWriter _textWriter;
    private bool _fileOpen;
     
    
    private void CreateFile()
    {
        if (Application.platform == RuntimePlatform.Android) return;
        
        Debug.Log("Creating new file!");
        if (!Directory.Exists(_directoryPath))
        {
            Debug.Log($"{_directoryPath} does not exist, creating directory..");
            Directory.CreateDirectory(_directoryPath);
        }

        string filePath = _directoryPath + "Log" + Random.Range(0, 100000) + ".csv";

        _textWriter = new StreamWriter(filePath);
        _fileOpen = true;
        
        _textWriter.WriteLine("{0},{1}", "Game Started:",DateTime.Now);
    }

    public void Log(TimeSpan gameTime,Card card, Player player)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        
        //Game time | Player | Card
        _textWriter.WriteLine("{0},{1},{2}",$"{gameTime.Hours:00}:{gameTime.Minutes:00}:{gameTime.Seconds:00}",player.Name,$"{card.suit}, {card.rank}");
    }

    public void LogChuckTime(TimeSpan chuckTime, Player player)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        
        _textWriter.WriteLine("{0},{1},{2}","Chuck!",player.Name,$"{chuckTime.Minutes:00}:{chuckTime.Seconds:00}:{chuckTime.Milliseconds:000}");
    }

    public void EndLog(TimeSpan gameTime, List<Player> players)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        
        if (!_fileOpen) return;
        
        _textWriter.WriteLine("{0},{1}","Game Ended",$"{gameTime.Hours:00}:{gameTime.Minutes:00}:{gameTime.Seconds:00}");
        _textWriter.WriteLine("{0},{1},{2},{3},{4},{5}","name","nr of beers","nr of sips","avg Sips","time Total","avg Time");
        foreach (Player player in players)
        {
            _textWriter.WriteLine("{0},{1},{2},{3},{4},{5}",player.Name,player.Beers,player.Sips,player.AvgSips,player.TimeTotal,player.AvgTime);
        }
        
        _textWriter.WriteLine("{0}", "File End");
        
        _fileOpen = false;
        _textWriter.Flush();
        _textWriter.Close();
    }

    public void StartTracking()
    {
        if (Application.platform == RuntimePlatform.Android) return;
        
        Debug.Log("Starting Tracker");
        CreateFile();
    }
    
    
}
