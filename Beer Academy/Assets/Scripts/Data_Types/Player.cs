using System.Collections.Generic;
using UnityEngine;

namespace Data_Types
{
    public class Player
    {
        private int _id;
        public readonly string name;
        public Color32 color;
    
        //Stats
        public float sips = 0;
        public float avgSips = 0;
        public float beers = 0;
        public float lastTime = 0;
        public float avgTime = 0;

        private readonly List<float> _chuckTimes = new List<float>();


        public Player(int id, string name, Color32 color)
        {
            this._id = id;
            this.name = name;
            this.color = color;
        }

        public void AddNewChuckTime(float newTime)
        {
            _chuckTimes.Add(newTime);
        }

        public List<float> GetChuckTimes()
        {
            return _chuckTimes;
        }
        
        
    }
}
