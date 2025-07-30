using FootballWorldCupScoreBoard.Domain;

namespace FootballWorldCupScoreBoard.Interface
{
    public interface IScoreboard
    {
        Match StartMatch(Team home, Team away);

        void UpdateScore(Guid matchId, int homeScore, int awayScore);

        void FinishMatch(Guid matchId);

        public List<Match> MatchesSummary();
    }
}