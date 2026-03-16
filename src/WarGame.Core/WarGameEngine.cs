using System;
using System.Collections.Generic;
using System.Linq;

namespace WarGame.Core
{
    /// <summary>
    /// Core War game engine: dealing, rounds, ties, and winner determination.
    /// </summary>
    public class WarGameEngine
    {
        private const int RoundLimit = 10_000;

        public PlayerHands PlayerHands { get; } = new PlayerHands();
        private readonly List<Card> _pot = new List<Card>();

        private int _roundNumber = 0;

        // Events for console reporting (Task 3)
        public event Action<int, Dictionary<string, Card>>? RoundStarted;
        public event Action<List<string>>? TieDetected;
        public event Action<string>? RoundWinner;
        public event Action<Dictionary<string, int>>? CardCountsUpdated;

        public WarGameEngine(IEnumerable<string> playerNames, Deck deck)
        {
            InitializeHands(playerNames);
            DealCardsRoundRobin(deck);
        }

        private void InitializeHands(IEnumerable<string> playerNames)
        {
            foreach (var name in playerNames)
            {
                PlayerHands.AddPlayer(name, new Hand());
            }
        }

        /// <summary>
        /// Deal all cards from the deck in round-robin order.
        /// </summary>
        private void DealCardsRoundRobin(Deck deck)
        {
            var names = PlayerHands.Hands.Keys.ToList();
            int index = 0;

            while (deck.Count > 0)
            {
                var playerName = names[index];
                PlayerHands[playerName].Enqueue(deck.Draw());

                index = (index + 1) % names.Count;
            }
        }

        /// <summary>
        /// Runs the game until one player has all cards or the round limit is reached.
        /// </summary>
        public WarGameResult PlayGame()
        {
            while (_roundNumber < RoundLimit && MoreThanOnePlayerHasCards())
            {
                _roundNumber++;
                PlayRound();
            }

            return DetermineResult();
        }

        private bool MoreThanOnePlayerHasCards()
        {
            return PlayerHands.Hands.Values.Count(h => h.HasCards) > 1;
        }

        private WarGameResult DetermineResult()
        {
            var result = new WarGameResult { RoundsPlayed = _roundNumber };

            var alive = PlayerHands.Hands
                .Where(kvp => kvp.Value.HasCards)
                .OrderByDescending(kvp => kvp.Value.Count)
                .ToList();

            if (alive.Count == 0)
            {
                result.IsDraw = true;
                return result;
            }

            if (alive.Count == 1)
            {
                result.WinnerName = alive[0].Key;
                return result;
            }

            // Round limit reached → highest card count wins
            int maxCount = alive[0].Value.Count;
            var top = alive.Where(kvp => kvp.Value.Count == maxCount).ToList();

            if (top.Count == 1)
            {
                result.WinnerName = top[0].Key;
            }
            else
            {
                result.IsDraw = true;
            }

            return result;
        }

        /// <summary>
        /// Plays a single round, including any tiebreakers.
        /// </summary>
        private void PlayRound()
        {
            _pot.Clear();

            var activePlayers = PlayerHands.Hands
                .Where(kvp => kvp.Value.HasCards)
                .Select(kvp => kvp.Key)
                .ToList();

            if (activePlayers.Count <= 1)
                return;

            ResolveBattle(activePlayers);
        }

        /// <summary>
        /// Resolves a battle (normal round or tiebreaker) among the given players.
        /// </summary>
        private void ResolveBattle(List<string> playersInBattle)
        {
            var playedCards = new Dictionary<string, Card>();

            // Each player plays one card
            foreach (var name in playersInBattle.ToList())
            {
                var hand = PlayerHands[name];

                if (!hand.HasCards)
                {
                    playersInBattle.Remove(name);
                    continue;
                }

                var card = hand.Dequeue();
                playedCards[name] = card;
                _pot.Add(card);
            }

            if (playedCards.Count == 0)
                return;

            // Notify console
            RoundStarted?.Invoke(_roundNumber, playedCards);

            // Determine highest rank
            var maxRank = playedCards.Max(kvp => kvp.Value.Rank);
            var highestPlayers = playedCards
                .Where(kvp => kvp.Value.Rank == maxRank)
                .Select(kvp => kvp.Key)
                .ToList();

            if (highestPlayers.Count == 1)
            {
                AwardPotToWinner(highestPlayers[0]);
                RoundWinner?.Invoke(highestPlayers[0]);
                return;
            }

            // Tie detected
            TieDetected?.Invoke(highestPlayers);

            // Only tied players continue
            var tiedPlayers = highestPlayers
                .Where(name => PlayerHands[name].HasCards)
                .ToList();

            if (tiedPlayers.Count == 0)
                return;

            ResolveBattle(tiedPlayers);
        }

        private void AwardPotToWinner(string winnerName)
        {
            var winnerHand = PlayerHands[winnerName];

            foreach (var card in _pot)
            {
                winnerHand.Enqueue(card);
            }

            _pot.Clear();

            // Update card counts
            var counts = PlayerHands.Hands.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
            CardCountsUpdated?.Invoke(counts);
        }
    }
}