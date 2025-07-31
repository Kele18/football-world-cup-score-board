namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class Match
    {
        private readonly List<GoalEvent> _goalEvents = new();
        private readonly object _lock = new();

        public Guid Id { get; }
        public Team HomeTeam { get; }
        public Team AwayTeam { get; }
        public Score Score { get; }
        public DateTime StartTime { get; }
        public MatchStatus Status { get; private set; }

        public DateTime? EndTime { get; private set; }

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

        public IReadOnlyList<GoalEvent> GoalEvents => _goalEvents.AsReadOnly();

        public void UpdateScore(int home, int away)
        {
            lock (_lock)
            {
                int currentMinute = (int)(DateTime.UtcNow - StartTime).TotalMinutes;

                int homeDelta = home - Score.Home;
                int awayDelta = away - Score.Away;
                Score.Update(home, away);

                for (int i = 0; i < homeDelta; i++)
                    _goalEvents.Add(new GoalEvent(TeamSide.Home, DateTime.UtcNow, currentMinute));

                for (int i = 0; i < awayDelta; i++)
                    _goalEvents.Add(new GoalEvent(TeamSide.Away, DateTime.UtcNow, currentMinute));
            }
        }

        public void Finish(int minutePlayed = 90)
        {
            Status = MatchStatus.Finished;
            EndTime = DateTime.UtcNow.AddMinutes(minutePlayed - 90);
        }
    }
}