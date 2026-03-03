using System;
using System.Collections.Generic;

namespace WarGame.Core
{
    /// <summary>
    /// Represents 52 deck of cards stored as a stack
    /// </summary>
    public class Deck
    {
        private readonly Stack<Card> _cards = new Stack<Card>();
        private static readonly Random _rng = new Random();

        public Deck()
        {
            Initialize();
            Shuffle();
        }

        /// <summary>
        /// Returns the next card from the top of the deck
        /// </summary>
        public Card Draw()
        {
            return _cards.Pop();
        }

        /// <summary>
        /// Number of cards remaining
        /// </summary>
        public int Count => _cards.Count;

        private void Initialize()
        {
            var list = new List<Card>();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    list.Add(new Card(suit, rank));
                }
            }

            // Convert list → stack
            foreach (var card in list)
            {
                _cards.Push(card);
            }
        }

        private void Shuffle()
        {
            // Stack → list → shuffle → stack
            var list = new List<Card>(_cards);
            _cards.Clear();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            foreach (var card in list)
            {
                _cards.Push(card);
            }
        }
    }
}