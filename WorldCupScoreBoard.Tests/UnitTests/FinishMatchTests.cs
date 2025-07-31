using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class FinishMatchTests : FinishMatchDriver
    {
        [Fact]
        public void FinishMatch_ValidMatch_SetsStatusAndEndTime()
        {
            var before = DateTime.UtcNow;

            Sut.FinishMatch(Match.Id, 95);

            Match.Status.Should().Be(MatchStatus.Finished);
            Match.EndTime.Should().NotBeNull();
            Match.EndTime.Value.Should().BeOnOrAfter(before);
            Match.EndTime.Value.Should().BeCloseTo(before.AddMinutes(5), TimeSpan.FromSeconds(2));

            VerifyGetMatchCalledOnce();
            VerifyRemoveCalledOnce();
        }

        [Fact]
        public void FinishMatch_DefaultMinute_SetsEndTimeAt90()
        {
            var before = DateTime.UtcNow;

            Sut.FinishMatch(Match.Id);

            Match.EndTime.Should().BeCloseTo(before, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void FinishMatch_MatchNotFound_ThrowsKeyNotFoundException()
        {
            SetupMatchNotFound();

            Action act = () => Sut.FinishMatch(Guid.NewGuid());

            act.Should().Throw<KeyNotFoundException>()
               .WithMessage("Match not found.");
        }

        [Fact]
        public void Finish_SetsStatusToFinished_AndSetsEndTime()
        {
            var before = DateTime.UtcNow;

            Match.Finish(95);

            Match.Status.Should().Be(MatchStatus.Finished);
            Match.EndTime.Should().NotBeNull();
            var expectedEnd = before.AddMinutes(5);
            Match.EndTime.Value.Should().BeCloseTo(expectedEnd, TimeSpan.FromSeconds(2));
        }
    }

    public class FinishMatchDriver : ScoreBoardServiceDriver
    {
        public Team Home { get; }
        public Team Away { get; }
        public Match Match { get; }

        public FinishMatchDriver()
        {
            Home = new Team("Germany");
            Away = new Team("Netherlands");

            Match = new Match(Home, Away);
            _dataSource.Setup(i => i.GetMatch(Match.Id)).Returns(Match);
        }

        public void SetupMatchNotFound()
        {
            _dataSource.Setup(i => i.GetMatch(It.IsAny<Guid>())).Returns(() => null);
        }

        public void VerifyGetMatchCalledOnce()
        {
            _dataSource.Verify(i => i.GetMatch(Match.Id), Times.Once);
        }

        public void VerifyRemoveCalledOnce()
        {
            _dataSource.Verify(i => i.Remove(Match.Id), Times.Once);
        }
    }
}