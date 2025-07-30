using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using Microsoft.Extensions.Logging;

namespace FootballWorldCupScoreBoard.Service
{
    public class Scoreboard(IMatchDataSource dataSource, ILogger<IScoreboard> logger) : IScoreboard
    {
        public Match StartMatch(Team home, Team away)
        {
            logger.LogInformation("Starting match between: Home: {Home} and Away :{Away}", home.Name, away.Name);
            var match = new Match(home, away);

            if (!dataSource.Add(match)) throw new InvalidOperationException("Match already started.");

            logger.LogInformation("Match between: Home: {Home} and Away :{Away} started {Id}", home.Name, away.Name, match.Id);
            return match;
        }

        public void UpdateScore(Guid matchId, int homeScore, int awayScore)
        {
            throw new NotImplementedException();
        }

        public void FinishMatch(Guid matchId)
        {
            throw new NotImplementedException();
        }

        public List<Match> MatchesSummary()
        {
            throw new NotImplementedException();
        }
    }
}