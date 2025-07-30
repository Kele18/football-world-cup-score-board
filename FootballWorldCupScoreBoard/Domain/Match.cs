namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class Match
    {
        public Guid Id { get; }
        public Team HomeTeam { get; }
        public Team AwayTeam { get; }
        public Score Score { get; }
        public DateTime StartTime { get; }
        public MatchStatus Status { get; }
        public int? FinishedAtMinute { get; private set; }

        public Match(Team home, Team away)
        {
            if (home.Equals(away)) throw new ArgumentException("Teams must be different.");
            Id = Guid.NewGuid();
            HomeTeam = home;
            AwayTeam = away;
            Score = new Score();
            Status = MatchStatus.InProgress;
            StartTime = DateTime.UtcNow;
        }
    }
}