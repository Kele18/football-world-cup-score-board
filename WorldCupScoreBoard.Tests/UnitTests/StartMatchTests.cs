using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Moq;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class StartMatchTests : StartMatchDriver
    {
        [Fact]
        public void StartMatch_ValidScheduledMatch_StartsSuccessfully()
        {
            var before = DateTime.UtcNow;

            var result = Sut.StartMatch(ScheduledMatch.Id);

            var after = DateTime.UtcNow;
            result.Id.Should().Be(ScheduledMatch.Id);
            result.Status.Should().Be(MatchStatus.InProgress);
            result.StartTime.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);

            VerifyScheduledMatchRemoved();
            VerifyMatchAddedToActive();
        }

        [Fact]
        public void StartMatch_MatchNotScheduled_ThrowsKeyNotFound()
        {
            SetupMatchNotScheduled();

            var act = () => Sut.StartMatch(ScheduledMatch.Id);

            act.Should().Throw<KeyNotFoundException>().WithMessage("Scheduled match not found.");
        }

        [Fact]
        public void StartMatch_RemoveFromScheduledFails_Throws()
        {
            SetupRemovefromScheduleDataSourceFails();

            var act = () => Sut.StartMatch(ScheduledMatch.Id);

            act.Should().Throw<InvalidOperationException>().WithMessage("Could not remove match from scheduled list.");
        }

        [Fact]
        public void StartMatch_AddToDataSourceFails_Throws()
        {
            SetupAddStartedGameFails();

            var act = () => Sut.StartMatch(ScheduledMatch.Id);

            act.Should().Throw<InvalidOperationException>().WithMessage("Failed to start match. It may already be active.");
        }

        [Fact]
        public void Match_Start_WhenAlreadyInProgressOrFinished_Throws()
        {
            var match = new Match(new Team("A"), new Team("B"), DateTime.UtcNow);
            match.GetType().GetProperty(nameof(Match.Status))!.SetValue(match, MatchStatus.Finished);

            var act = () => match.Start();

            act.Should().Throw<InvalidOperationException>().WithMessage("Only scheduled matches can be started.");
        }

        [Fact]
        public void Match_Start_WhenScheduled_TransitionsToInProgress()
        {
            var match = new Match(new Team("X"), new Team("Y"), DateTime.UtcNow);
            match.GetType().GetProperty(nameof(Match.Status))!.SetValue(match, MatchStatus.Scheduled);

            match.Start();

            match.Status.Should().Be(MatchStatus.InProgress);
            match.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }

    public class StartMatchDriver : ScoreBoardServiceDriver
    {
        public Match ScheduledMatch { get; }

        public StartMatchDriver()
        {
            var home = new Team("Italy");
            var away = new Team("Spain");

            ScheduledMatch = new Match(home, away, DateTime.UtcNow.AddMinutes(10));
            typeof(Match)
                .GetProperty(nameof(Match.Status))!
                .SetValue(ScheduledMatch, MatchStatus.Scheduled);

            _scheduledMatchDataSource.Setup(i => i.GetMatch(ScheduledMatch.Id)).Returns(ScheduledMatch);
            _scheduledMatchDataSource.Setup(i => i.Remove(ScheduledMatch.Id)).Returns(true);
            _dataSource.Setup(i => i.Add(ScheduledMatch)).Returns(true);
        }

        public void SetupAddStartedGameFails()
        {
            _dataSource.Setup(i => i.Add(ScheduledMatch)).Returns(false);
        }

        public void SetupRemovefromScheduleDataSourceFails()
        {
            _scheduledMatchDataSource.Setup(i => i.Remove(ScheduledMatch.Id)).Returns(false);
        }

        public void SetupMatchNotScheduled()
        {
            _scheduledMatchDataSource.Setup(i => i.GetMatch(ScheduledMatch.Id)).Returns(() => null);
        }

        public void VerifyScheduledMatchRemoved()
        {
            _scheduledMatchDataSource.Verify(i => i.Remove(ScheduledMatch.Id), Times.Once);
        }

        public void VerifyMatchAddedToActive()
        {
            _dataSource.Verify(i => i.Add(ScheduledMatch), Times.Once);
        }
    }
}