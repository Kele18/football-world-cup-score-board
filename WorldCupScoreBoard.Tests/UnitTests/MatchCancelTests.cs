using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class MatchCancelTests : MatchCancelDriver
    {
        [Fact]
        public void CancelMatch_ScheduledMatch_SetsStatusToCancelled()
        {
            Sut.CancelMatch(ScheduledMatch.Id);

            ScheduledMatch.Status.Should().Be(MatchStatus.Cancelled);
            ScheduledMatch.EndTime.Should().NotBeNull();
            VerifyRemoveFromScheduledDataSourceCalled();
            VerifyArchievedCalledOnce();
        }

        [Fact]
        public void CancelMatch_NotScheduled_Throws()
        {
            ScheduledMatch.Start();

            Action act = () => Sut.CancelMatch(ScheduledMatch.Id);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Only scheduled matches can be canceled.");

            VerifyRemoveAndArchivedNotCalled();
        }

        [Fact]
        public void Cancel_ScheduledMatch_ShouldUpdateStatus()
        {
            ScheduledMatch = Match.CreateScheduled(Home, Away, DateTime.UtcNow);

            ScheduledMatch.Cancel();

            ScheduledMatch.Status.Should().Be(MatchStatus.Cancelled);
            ScheduledMatch.EndTime.Should().NotBeNull();
        }

        [Fact]
        public void Cancel_NonScheduledMatch_ShouldThrow()
        {
            var match = new Match(new Team("Home"), new Team("Away"), status: MatchStatus.InProgress);

            Action act = () => match.Cancel();

            act.Should().Throw<InvalidOperationException>().WithMessage("Only scheduled matches can be canceled.");
        }
    }

    public class MatchCancelDriver : ScoreBoardServiceDriver
    {
        public Match ScheduledMatch { get; set; }
        public Team Home { get; }
        public Team Away { get; }

        public MatchCancelDriver()
        {
            Home = new Team("Brazil");
            Away = new Team("Argentina");

            ScheduledMatch = Match.CreateScheduled(Home, Away, DateTime.UtcNow.AddMinutes(30));

            _scheduledMatchDataSource.Setup(x => x.GetMatch(ScheduledMatch.Id)).Returns(ScheduledMatch);
            _scheduledMatchDataSource.Setup(x => x.Remove(ScheduledMatch.Id)).Returns(true);
        }

        public void VerifyRemoveFromScheduledDataSourceCalled()
        {
            _scheduledMatchDataSource.Verify(x => x.Remove(ScheduledMatch.Id), Times.Once);
        }

        public void VerifyArchievedCalledOnce()
        {
            _archiveMatchDataSource.Verify(i => i.Add(ScheduledMatch), Times.Once);
        }

        public void VerifyRemoveAndArchivedNotCalled()
        {
            _scheduledMatchDataSource.Verify(i => i.Remove(It.IsAny<Guid>()), Times.Never);
            _archiveMatchDataSource.Verify(i => i.Add(It.IsAny<Match>()), Times.Never);
        }
    }
}