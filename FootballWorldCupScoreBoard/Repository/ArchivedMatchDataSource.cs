using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;

namespace FootballWorldCupScoreBoard.Repository
{
    public class ArchivedMatchDataSource : IArchiveMatchDataSource
    {
        private readonly Dictionary<Guid, Match> _archivedMatches = [];

        public bool Add(Match match)
        {
            if (match.Status == MatchStatus.InProgress)
                throw new InvalidOperationException("Only non-active matches can be archived.");

            if (_archivedMatches.ContainsKey(match.Id))
                return false;

            _archivedMatches[match.Id] = match;
            return true;
        }

        public IEnumerable<Match> GetAllMatches()
        {
            return _archivedMatches.Values;
        }

        public Match? GetMatch(Guid matchId)
        {
            return _archivedMatches.TryGetValue(matchId, out var match) ? match : null;
        }

        public bool Remove(Guid matchId)
        {
            return _archivedMatches.Remove(matchId);
        }
    }
}