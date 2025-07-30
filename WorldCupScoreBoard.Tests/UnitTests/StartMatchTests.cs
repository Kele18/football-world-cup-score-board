using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class StartMatchTests : StartMatchDriver
    {
        [Fact]
        public void StartMatch_SaveToRepository_ReturnMatch()
        {
            var beforeStart = DateTime.UtcNow;

            var result = Sut.StartMatch(Home, Away);

            var afterStart = DateTime.UtcNow;

            result.Should().BeEquivalentTo(Match, o => o.Excluding(i => i.Id)
                                                        .Excluding(o => o.StartTime));

            result.StartTime.Should().BeOnOrAfter(beforeStart).And.BeOnOrBefore(afterStart);
            VerifyDataSourcAddCalled();
        }

        [Fact]
        public void StartMatch_MatchAlreadyStarted_ThrowException()
        {
            SetupMatchAlreadyStarted();

            Action act = () => Sut.StartMatch(Home, Away);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Match already started.");
        }
    }

    public class StartMatchDriver : ScoreBoardServiceDriver
    {
        public Guid MatchId { get; set; }
        public Team Home { get; }
        public Team Away { get; }

        public Match Match { get; set; }

        public StartMatchDriver()
        {
            Home = new Team("Italy");
            Away = new Team("Spain");

            Match = new Match(Home, Away);
            _dataSource.Setup(i => i.Add(It.Is<Match>(j => j.AwayTeam == Away &&
                                                            j.HomeTeam == Home &&
                                                            j.Id != Guid.Empty &&
                                                            j.Status == MatchStatus.InProgress))).Returns(true);
        }

        public void SetupMatchAlreadyStarted()
        {
            _dataSource.Setup(i => i.Add(It.Is<Match>(j => j.HomeTeam == Home && j.AwayTeam == Away)))
                       .Returns(false);
        }

        public void VerifyDataSourcAddCalled()
        {
            _dataSource.Verify(i => i.Add(It.Is<Match>(j => j.AwayTeam == Away &&
                                                            j.HomeTeam == Home &&
                                                            j.Id != Guid.Empty &&
                                                            j.Status == MatchStatus.InProgress)), Times.Once());
        }
    }
}