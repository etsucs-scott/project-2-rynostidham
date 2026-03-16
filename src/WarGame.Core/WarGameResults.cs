using System;

namespace WarGame.Core
{
    /// <summary>
    /// Final results of war game
    /// </summary>
    public class WarGameResult
    {
        public string? WinnerName { get; set; }
        public bool IsDraw { get; set; }
        public int RoundsPlayed { get; set; }
    }
}
