using FootballWorldCupScoreBoard.Domain;

namespace FootballWorldCupScoreBoard.Interface
{
    public interface IMatchDataSource
    {
        bool Add(Match match);

        Match? GetMatch(Guid matchId);

        bool Remove(Guid matchId);

        IEnumerable<Match> GetAllMatches();
    }
}