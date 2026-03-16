using System;
using System.Collections.Generic;

namespace WarGame.Core
{
    /// <summary>
    /// Holds all players hands, keyed by player name
    /// </summary>
    public class PlayerHands
    {
        public Dictionary<string, Hand> Hands { get; } = new Dictionary<string, Hand>();

        public void AddPlayer(string name, Hand hand)
        {
            Hands[name] = hand;
        }

        public Hand this[string name] => Hands[name];
    }
}
