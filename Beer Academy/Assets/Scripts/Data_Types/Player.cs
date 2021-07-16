using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data_Types
{
    public class Player
    {
        private int _id;
        public readonly string Name;
        public Color32 Color;
        public float SipsInABeer;
        public readonly bool IsProtected;    //Protected against drawing acees
    
        //Stats
        public float Sips = 0;
        public float AvgSips = 0;
        public float Beers = 0;
        public TimeSpan LastTime = TimeSpan.Zero;
        public TimeSpan AvgTime = TimeSpan.Zero;
        public TimeSpan TimeTotal = TimeSpan.Zero;

        private readonly List<float> _chuckTimes = new List<float>();


        public Player(int id, string name,float sipsInABeer, bool isProtected, Color32 color)
        {
            _id = id;
            Name = name;
            SipsInABeer = sipsInABeer;
            IsProtected = isProtected;
            
            Color = color;
        }

        public void AddNewChuckTime(float newTime)
        {
            _chuckTimes.Add(newTime);
        }

        public List<float> GetChuckTimes()
        {
            return _chuckTimes;
        }

        public float GetFastestChuckTime()
        {
            float fastestTime = 10000000;
            foreach (float chuckTime in _chuckTimes)
            {
                if (chuckTime < fastestTime)
                {
                    fastestTime = chuckTime;
                }
            }
            
            Debug.Log($"returning {fastestTime}");

            return fastestTime;
        }
        
        
    }
}
