using System
    /// <summary>
    /// Represents a singe playing card with suits and ranks.
    /// </summary>
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

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
//Just a test to see if this works
