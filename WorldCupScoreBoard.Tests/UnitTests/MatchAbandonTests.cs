using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class MatchAbandonTests : MatchAbandonDriver
    {
        [Fact]
        public void AbandonMatch_InProgress_SetsStatusToAbandoned()
        {
            Sut.Abandon(ActiveMatch.Id);

            ActiveMatch.Status.Should().Be(MatchStatus.Abandoned);
            ActiveMatch.EndTime.Should().NotBeNull();
            VerifyRemoveCalled();
        }

        [Fact]
        public void AbandonMatch_NotInProgress_Throws()
        {
            ActiveMatch.Finish();

            Action act = () => Sut.Abandon(ActiveMatch.Id);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Only matches in progress can be abandoned.");
        }

        [Fact]
        public void Abandon_InProgressMatch_ShouldUpdateStatus()
        {
            ActiveMatch.Abandon();

            ActiveMatch.Status.Should().Be(MatchStatus.Abandoned);
            ActiveMatch.EndTime.Should().NotBeNull();
        }

        [Fact]
        public void Abandon_NotInProgressMatch_ShouldThrow()
        {
            var match = new Match(new Team("Home"), new Team("Away"), status: MatchStatus.Scheduled);

            Action act = () => match.Abandon();

            act.Should().Throw<InvalidOperationException>().WithMessage("Only matches in progress can be abandoned.");
        }
    }

    public class MatchAbandonDriver : ScoreBoardServiceDriver
    {
        public Match ActiveMatch { get; }

        public MatchAbandonDriver()
        {
            var home = new Team("Germany");
            var away = new Team("Italy");

            ActiveMatch = new Match(home, away);

            _dataSource.Setup(x => x.GetMatch(ActiveMatch.Id)).Returns(ActiveMatch);
            _dataSource.Setup(x => x.Remove(ActiveMatch.Id)).Returns(true);
        }

        public void VerifyRemoveCalled()
        {
            _dataSource.Verify(x => x.Remove(ActiveMatch.Id), Times.Once);
        }
    }
}