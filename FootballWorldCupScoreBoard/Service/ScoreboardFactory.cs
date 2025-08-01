using FootballWorldCupScoreBoard.Interface;
using FootballWorldCupScoreBoard.Repository;
using Microsoft.Extensions.Logging.Abstractions;

namespace FootballWorldCupScoreBoard.Service
{
    public class ScoreboardFactory : IScoreBoardFactory
    {
        public IScoreboard CreateDefault()
        {
            return new Scoreboard(
                new MatchDataSource(),
                new ScheduledMatchDataSource(),
                new ArchivedMatchDataSource(),
                NullLogger<IScoreboard>.Instance);
        }
    }
}