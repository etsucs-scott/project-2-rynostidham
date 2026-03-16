using System;
using System.Collections.Generic;

namespace WarGame.Core
{
    /// <summary>
    /// Represents a player's hand as a queue of cards
    /// </summary>
    public class Hand
    {
        private readonly Queue<Card> _cards = new Queue<Card>();
        
        public int Count => _cards.Count;

        public bool HasCards => _cards.Count > 0;

        public void Enqueue(Card card)
        {
            _cards.Enqueue(card);
        }

        public Card Dequeue()
        {
            return _cards.Dequeue();
        }

        public IEnumerable<Card> Cards => _cards;
    }
}