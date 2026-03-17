using System;
///<summary>
///Creates all cards with suit and rank
/// </summary>
namespace WarGame.Core
{
    public class Card : IComparable<Card>
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public int CompareTo(Card? other)
        {
            if (other == null) return 1;
            return Rank.CompareTo(other.Rank);
        }

        public override string ToString() => $"{Rank} of {Suit}";
    }
}
