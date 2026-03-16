using System;
using System.Collections.Generic;
using WarGame.Core;

namespace WarGame.ConsoleApp
{
    /// <summary>
    /// Console runner for the War card game
    /// Handles user input, subscribes to engine events,
    /// and prints readable round-by-round output
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== WAR (Card Game) ===");

            // Determine number of players (2–4) from args or prompt
            int playerCount = GetPlayerCount(args);

            // Create default player names: Player 1, Player 2, etc.
            var playerNames = CreatePlayerNames(playerCount);

            // Create and shuffle a new deck
            var deck = new Deck();

            // Create the game engine with players + deck
            var engine = new WarGameEngine(playerNames, deck);

            // Subscribe to engine events for reporting
            engine.RoundStarted += OnRoundStarted;
            engine.TieDetected += OnTieDetected;
            engine.RoundWinner += OnRoundWinner;
            engine.CardCountsUpdated += OnCardCountsUpdated;

            // Run the full game until a winner or round limit
            var result = engine.PlayGame();

            // Print final results
            Console.WriteLine("\n=== GAME OVER ===");

            if (result.IsDraw)
            {
                Console.WriteLine("Result: Draw after 10,000 rounds.");
            }
            else
            {
                Console.WriteLine($"Winner: {result.WinnerName}");
            }

            Console.WriteLine($"Rounds Played: {result.RoundsPlayed}");
        }

        // Player Setup

        /// <summary>
        /// Gets the number of players either from command-line args
        /// or by prompting the user. Must be between 2 and 4
        /// </summary>
        private static int GetPlayerCount(string[] args)
        {
            // If argument provided and valid, use it
            if (args.Length > 0 &&
                int.TryParse(args[0], out int n) &&
                n >= 2 && n <= 4)
            {
                return n;
            }

            // Otherwise prompt until valid input is received
            while (true)
            {
                Console.Write("Enter number of players (2–4): ");
                if (int.TryParse(Console.ReadLine(), out n) &&
                    n >= 2 && n <= 4)
                {
                    return n;
                }
            }
        }

        /// <summary>
        /// Creates default player names: "Player 1", "Player 2", etc
        /// </summary>
        private static List<string> CreatePlayerNames(int count)
        {
            var names = new List<string>();
            for (int i = 1; i <= count; i++)
                names.Add($"Player {i}");
            return names;
        }

        // --------------------------------------------------------------------
        // Event Handlers (Reporting)
        // --------------------------------------------------------------------

        /// <summary>
        /// Start of each round
        /// Prints the round number and each player's revealed card
        /// </summary>
        private static void OnRoundStarted(int round, Dictionary<string, Card> played)
        {
            Console.WriteLine($"\nRound {round}");

            foreach (var kvp in played)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Rank}");
            }
        }

        /// <summary>
        /// Starts when two or more players tie for highest rank
        /// Indicates a tiebreaker round is starting
        /// </summary>
        private static void OnTieDetected(List<string> tiedPlayers)
        {
            Console.WriteLine($"Tie between: {string.Join(", ", tiedPlayers)}");
            Console.WriteLine("Tiebreaker round!");
        }

        /// <summary>
        /// Starts when a round (or tiebreaker chain) produces a winner
        /// </summary>
        private static void OnRoundWinner(string winner)
        {
            Console.WriteLine($"Winner: {winner}");
        }

        /// <summary>
        /// Starts after the pot is awarded
        /// Prints updated card counts for all players
        /// </summary>
        private static void OnCardCountsUpdated(Dictionary<string, int> counts)
        {
            Console.Write("Card counts: ");
            foreach (var kvp in counts)
            {
                Console.Write($"{kvp.Key}={kvp.Value}  ");
            }
            Console.WriteLine();
        }
    }
}