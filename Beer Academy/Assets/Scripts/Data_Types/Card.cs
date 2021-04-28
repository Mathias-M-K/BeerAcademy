
using UnityEngine;

namespace Data_Types
{
    public class Card
    {
        public readonly int rank;
        public readonly Suit suit;
        public GameObject cardObj;


        public Card(int rank, Suit suit, GameObject cardObj)
        {
            this.rank = rank;
            this.suit = suit;
            this.cardObj = cardObj;
        }
    }
}
