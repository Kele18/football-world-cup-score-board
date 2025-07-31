using FootballWorldCupScoreBoard.Domain;

namespace FootballWorldCupScoreBoard.Interface
{
    public interface IScoreboard
    {
        Match ScheduleMatch(Team home, Team away, DateTime scheduledTime);

        Match StartMatch(Guid matchId);

        void UpdateScore(Guid matchId, int homeScore, int awayScore);

        void FinishMatch(Guid matchId, int minutePlayed = 90);

        public List<Match> MatchesSummary();
    }
}