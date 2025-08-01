using FootballWorldCupScoreBoard.Domain;

namespace FootballWorldCupScoreBoard.Interface
{
    public interface IScoreboard
    {
        Match ScheduleMatch(Team home, Team away, DateTime scheduledTime);

        Match StartMatch(Guid matchId);

        void UpdateScore(Guid matchId, int homeScore, int awayScore);

        void FinishMatch(Guid matchId, int minutePlayed = 90);

        void UndoGoal(Guid matchId, TeamSide side, string? reason = null);

        void CancelMatch(Guid matchId);

        void Abandon(Guid matchId);

        IList<Match> GetLiveSummary();

        IList<Match> GetScheduled();

        IList<Match> GetArchived();
    }
}