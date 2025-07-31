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
            Match match = dataSource.GetMatch(matchId) ?? throw new KeyNotFoundException("No match in progress found.");

            match.UpdateScore(homeScore, awayScore);

            logger.LogInformation("Updated score: {Home}-{HomeScore} vs {Away}-{AwayScore}", match.HomeTeam, homeScore, match.AwayTeam, awayScore);
        }

        public void FinishMatch(Guid matchId, int minutePlayed = 90)
        {
            var match = dataSource.GetMatch(matchId) ?? throw new KeyNotFoundException("Match not found.");

            match.Finish(minutePlayed);
            dataSource.Remove(matchId);

            logger.LogInformation("Finished match {Id}", matchId);
        }

        public List<Match> MatchesSummary()
        {
            return dataSource.GetAllMatches()
                             .Where(m => m.Status == MatchStatus.InProgress)
                             .OrderByDescending(m => m.Score.Home + m.Score.Away)
                             .ThenByDescending(m => m.StartTime)
                             .ToList();
        }
    }
}