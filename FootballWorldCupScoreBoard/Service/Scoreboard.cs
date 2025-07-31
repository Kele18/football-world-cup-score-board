using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using Microsoft.Extensions.Logging;

namespace FootballWorldCupScoreBoard.Service
{
    public class Scoreboard(
        IMatchDataSource dataSource,
        IScheduledMatchDataSource scheduledMatchDataSource,
        ILogger<IScoreboard> logger) : IScoreboard
    {
        public Match ScheduleMatch(Team home, Team away, DateTime scheduledTime)
        {
            var match = Match.CreateScheduled(home, away, scheduledTime);
            if (!scheduledMatchDataSource.Add(match))
                throw new InvalidOperationException("Match already scheduled.");

            logger.LogInformation("Scheduled match: {Home} vs {Away} at {Time}", home.Name, away.Name, scheduledTime);
            return match;
        }

        public Match StartMatch(Guid matchId)
        {
            var match = scheduledMatchDataSource.GetMatch(matchId)
             ?? throw new KeyNotFoundException("Scheduled match not found.");

            match.Start();

            if (!scheduledMatchDataSource.Remove(matchId))
                throw new InvalidOperationException("Could not remove match from scheduled list.");

            if (!dataSource.Add(match))
                throw new InvalidOperationException("Failed to start match. It may already be active.");

            logger.LogInformation("Started match: {Home} vs {Away}", match.HomeTeam.Name, match.AwayTeam.Name);

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