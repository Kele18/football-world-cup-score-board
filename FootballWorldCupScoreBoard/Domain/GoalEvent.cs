namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class GoalEvent(TeamSide side, DateTime time, int minuteOfPlay)
    {
        public TeamSide Side { get; } = side;
        public DateTime Time { get; } = time;
        public int MinuteOfPlay { get; } = minuteOfPlay;
        public string? Reason { get; private set; }
        public bool Undone { get; private set; }

        public void MarkAsUndone(string? reason)
        {
            Undone = true;
            Reason = reason;
        }
    }
}