using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class UpdateScoreTests : UpdateScoreDriver
    {
        [Fact]
        public void UpdateScore_ValidMatch_UpdatesScoreCorrectly()
        {
            const int homeScore = 2;
            const int awayScore = 1;

            Sut.UpdateScore(Match.Id, homeScore, awayScore);

            Match.Score.Home.Should().Be(2);
            Match.Score.Away.Should().Be(1);

            VerifyGetMatchCalledOnce();
        }

        [Fact]
        public void UpdateScore_MatchNotFound_ThrowsKeyNotFoundException()
        {
            SetupMatchNotFound();
            const int homeScore = 1;
            const int awayScore = 0;

            Action act = () => Sut.UpdateScore(Guid.NewGuid(), homeScore, awayScore);

            act.Should().Throw<KeyNotFoundException>()
               .WithMessage("No match in progress found.");
        }

        [Fact]
        public void UpdateScore_SameScore_NoGoalEventsAdded()
        {
            int existingEventCount = Match.GoalEvents.Count;
            const int homeScore = 0;
            const int awayScore = 0;

            Sut.UpdateScore(Match.Id, homeScore, awayScore);

            Match.GoalEvents.Count.Should().Be(existingEventCount);
        }

        [Fact]
        public void UpdateScore_Success_UpdatesScoreProperties()
        {
            Match.UpdateScore(2, 1);

            Match.Score.Home.Should().Be(2);
            Match.Score.Away.Should().Be(1);
        }

        [Fact]
        public void UpdateScore_SameScore_DoesNotAddEvents()
        {
            Match.UpdateScore(0, 0);

            Match.GoalEvents.Should().BeEmpty();
        }
    }

    public class UpdateScoreDriver : ScoreBoardServiceDriver
    {
        public Team Home { get; }
        public Team Away { get; }
        public Match Match { get; }

        public UpdateScoreDriver()
        {
            Home = new Team("Brazil");
            Away = new Team("Argentina");

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
    }
}