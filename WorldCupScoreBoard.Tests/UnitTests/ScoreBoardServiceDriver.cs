using FootballWorldCupScoreBoard.Interface;
using FootballWorldCupScoreBoard.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class ScoreBoardServiceDriver
    {
        internal IScoreboard Sut { get; }
        internal readonly Mock<IMatchDataSource> _dataSource;
        internal readonly Mock<IScheduledMatchDataSource> _scheduledMatchDataSource;
        internal readonly Mock<ILogger<IScoreboard>> _logger;

        public ScoreBoardServiceDriver()
        {
            _dataSource = new Mock<IMatchDataSource>();
            _scheduledMatchDataSource = new Mock<IScheduledMatchDataSource>();
            _logger = new Mock<ILogger<IScoreboard>>();

            Sut = new Scoreboard(_dataSource.Object, _scheduledMatchDataSource.Object, _logger.Object);
        }
    }
}