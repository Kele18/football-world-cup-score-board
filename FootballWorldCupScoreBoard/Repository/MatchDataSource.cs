using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using System.Collections.Concurrent;

namespace FootballWorldCupScoreBoard.Repository
{
    public sealed class MatchDataSource : IMatchDataSource
    {
        private readonly ConcurrentDictionary<Guid, Match> _matches;

        public MatchDataSource()
        {
            _matches = new ConcurrentDictionary<Guid, Match>();
        }

        public bool Add(Match match)
        {
            return _matches.TryAdd(match.Id, match);
        }

        public IEnumerable<Match> GetAllMatches()
        {
            return _matches.Values;
        }

        public Match? GetMatch(Guid matchId)
        {
            return _matches.TryGetValue(matchId, out var match) ? match : null;
        }

        public bool Remove(Guid matchId)
        {
            return _matches.TryRemove(matchId, out _);
        }
    }
}