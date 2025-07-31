namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class GoalEvent(TeamSide side, DateTime time, int minuteOfPlay, string? reason = null)
    {
        public TeamSide Side { get; } = side;
        public DateTime Time { get; } = time;
        public int MinuteOfPlay { get; } = minuteOfPlay;
        public string? Reason { get; } = reason;
    }
}