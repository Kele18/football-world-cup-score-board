using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using System.Reflection;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class MatchesSummaryTests : MatchesSummaryDriver
    {
        [Fact]
        public void MatchesSummary_FiltersOnlyInProgressMatches()
        {
            var result = Sut.GetLiveSummary();

            result.Should().OnlyContain(m => m.Status == MatchStatus.InProgress);
        }

        [Fact]
        public void MatchesSummary_OrdersByTotalScoreDescending_ThenByStartTimeDescending()
        {
            var result = Sut.GetLiveSummary();

            result.Should().HaveCount(3);
            result[0].Should().Be(MatchWithHighestScore);
            result[1].Should().Be(MatchWithLowerScoreNewer);
            result[2].Should().Be(MatchWithLowerScoreOlder);
        }

        [Fact]
        public void MatchesSummary_EmptyDataSource_ReturnsEmptyList()
        {
            SetupEmptyMatches();

            var result = Sut.GetLiveSummary();

            result.Should().BeEmpty();
        }

        [Fact]
        public void MatchesSummary_OnlyFinishedMatches_ReturnsEmptyList()
        {
            SetupOnlyFinishedMatches();

            var result = Sut.GetLiveSummary();

            result.Should().BeEmpty();
        }
    }

    public class MatchesSummaryDriver : ScoreBoardServiceDriver
    {
        private readonly Team teamA;
        private readonly Team teamD;

        public Match MatchWithHighestScore { get; }
        public Match MatchWithLowerScoreNewer { get; }
        public Match MatchWithLowerScoreOlder { get; }
        public Match FinishedMatch { get; }

        public MatchesSummaryDriver()
        {
            teamA = new Team("Team A");
            var teamB = new Team("Team B");
            var teamC = new Team("Team C");
            teamD = new Team("Team D");

            MatchWithHighestScore = new Match(teamA, teamB);
            MatchWithLowerScoreNewer = new Match(teamB, teamC);
            MatchWithLowerScoreOlder = new Match(teamC, teamD);
            FinishedMatch = new Match(teamA, teamD);

            MatchWithHighestScore.UpdateScore(3, 2);
            MatchWithLowerScoreNewer.UpdateScore(1, 1);
            MatchWithLowerScoreOlder.UpdateScore(1, 1);
            FinishedMatch.Finish(90);

            SetStartTime(MatchWithLowerScoreNewer, DateTime.UtcNow.AddMinutes(-5));
            SetStartTime(MatchWithLowerScoreOlder, DateTime.UtcNow.AddMinutes(-30));
            SetStartTime(MatchWithHighestScore, DateTime.UtcNow.AddMinutes(-10));

            _dataSource.Setup(i => i.GetAllMatches())
                .Returns(
                [
                    MatchWithHighestScore,
                    MatchWithLowerScoreNewer,
                    MatchWithLowerScoreOlder,
                    FinishedMatch
                ]);
        }

        public void SetupEmptyMatches()
        {
            _dataSource.Setup(i => i.GetAllMatches()).Returns([]);
        }

        public void SetupOnlyFinishedMatches()
        {
            typeof(Match)
                .GetProperty(nameof(Match.Status))!
                .SetValue(FinishedMatch, MatchStatus.Finished);

            typeof(Match)
                .GetProperty(nameof(Match.EndTime))!
                .SetValue(FinishedMatch, DateTime.UtcNow);

            _dataSource.Setup(i => i.GetAllMatches()).Returns([FinishedMatch]);
        }

        private static void SetStartTime(Match match, DateTime time) => typeof(Match)
                .GetField("<StartTime>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(match, time);
    }
}