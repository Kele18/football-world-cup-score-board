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
        public DateTime StartTime { get; private set; }
        public MatchStatus Status { get; private set; } = MatchStatus.None;

        public DateTime? EndTime { get; private set; }

        public Match(Team home, Team away, DateTime? scheduledTime = null, MatchStatus status = MatchStatus.InProgress)
        {
            if (home.Equals(away)) throw new ArgumentException("Teams must be different.");
            Id = Guid.NewGuid();
            HomeTeam = home;
            AwayTeam = away;
            Score = new Score();
            Status = status;
            StartTime = scheduledTime ?? DateTime.UtcNow;
        }

        public IReadOnlyList<GoalEvent> GoalEvents
        {
            get
            {
                lock (_lock)
                    return _goalEvents.AsReadOnly();
            }
        }

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
            if (Status != MatchStatus.InProgress)
                throw new InvalidOperationException("Only matches in progress can be finished.");

            Status = MatchStatus.Finished;
            EndTime = DateTime.UtcNow.AddMinutes(minutePlayed - 90);
        }

        public static Match CreateScheduled(Team home, Team away, DateTime scheduledTime)
        {
            if (home.Equals(away)) throw new ArgumentException("Teams must be different.");
            return new Match(home, away, scheduledTime, MatchStatus.Scheduled);
        }

        public void Start()
        {
            if (Status != MatchStatus.Scheduled && Status != MatchStatus.None)
                throw new InvalidOperationException("Only scheduled matches can be started.");

            Status = MatchStatus.InProgress;
            StartTime = DateTime.UtcNow;
        }
    }
}