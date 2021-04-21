
using UnityEngine;

namespace Data_Types
{
    public class Card
    {
        public readonly int rank;
        public readonly Suit suit;


        public Card(int rank, Suit suit)
        {
            this.rank = rank;
            this.suit = suit;

        }
    }
}
