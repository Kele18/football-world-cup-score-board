using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class ScheduleMatchTests : ScheduleMatchDriver
    {
        [Fact]
        public void ScheduleMatch_ValidTeams_SavesAndReturnsMatch()
        {
            var result = Sut.ScheduleMatch(Home, Away, ScheduledTime);

            result.Should().NotBeNull();
            result.HomeTeam.Should().Be(Home);
            result.AwayTeam.Should().Be(Away);
            result.Status.Should().Be(MatchStatus.Scheduled);
            result.StartTime.Should().BeCloseTo(ScheduledTime, TimeSpan.FromSeconds(1));
            VerifyScheduledDataSourceAddCalled();
        }

        [Fact]
        public void ScheduleMatch_AlreadyScheduled_ThrowsException()
        {
            SetupMatchAlreadyScheduled();

            Action act = () => Sut.ScheduleMatch(Home, Away, ScheduledTime);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Match already scheduled.");
        }
    }

    public class ScheduleMatchDriver : ScoreBoardServiceDriver
    {
        public Team Home { get; }
        public Team Away { get; }
        public DateTime ScheduledTime { get; }

        public ScheduleMatchDriver()
        {
            Home = new Team("Brazil");
            Away = new Team("Germany");
            ScheduledTime = DateTime.UtcNow.AddHours(2);

            _scheduledMatchDataSource
                .Setup(i => i.Add(It.Is<Match>(m =>
                    m.HomeTeam == Home &&
                    m.AwayTeam == Away &&
                    m.Status == MatchStatus.Scheduled &&
                    m.StartTime == ScheduledTime)))
                .Returns(true);
        }

        public void SetupMatchAlreadyScheduled()
        {
            _scheduledMatchDataSource
                .Setup(i => i.Add(It.IsAny<Match>()))
                .Returns(false);
        }

        public void VerifyScheduledDataSourceAddCalled()
        {
            _scheduledMatchDataSource.Verify(i => i.Add(It.Is<Match>(m =>
                m.HomeTeam == Home &&
                m.AwayTeam == Away &&
                m.Status == MatchStatus.Scheduled &&
                m.StartTime == ScheduledTime)), Times.Once());
        }
    }
}