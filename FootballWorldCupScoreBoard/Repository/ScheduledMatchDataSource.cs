using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using System.Collections.Concurrent;

namespace FootballWorldCupScoreBoard.Repository
{
    public sealed class ScheduledMatchDataSource : IScheduledMatchDataSource
    {
        private readonly ConcurrentDictionary<Guid, Match> _store = new();

        public bool Add(Match match)
        {
            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("The match cannot be scheduled");
            return _store.TryAdd(match.Id, match);
        }

        public IEnumerable<Match> GetAllMatches() => _store.Values;

        public Match? GetMatch(Guid matchId) => _store.TryGetValue(matchId, out var match) ? match : null;

        public bool Remove(Guid matchId) => _store.TryRemove(matchId, out _);
    }
}