namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class GoalEvent(TeamSide side, DateTime timestamp, string? reason = null)
    {
        public TeamSide Side { get; } = side;
        public DateTime Timestamp { get; } = timestamp;
        public string? Reason { get; } = reason;
    }
}