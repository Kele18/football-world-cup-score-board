using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using FootballWorldCupScoreBoard.Repository;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class ScheduledMatchDataSourceTests : ScheduledMatchDataSourceDriver
    {
        [Fact]
        public void Add_ValidScheduledMatch_ReturnsTrue()
        {
            var result = Sut.Add(ScheduledMatch);

            result.Should().BeTrue();
        }

        [Fact]
        public void Add_InvalidStatusMatch_ThrowsException()
        {
            var act = () => Sut.Add(InvalidMatch);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("The match cannot be scheduled");
        }

        [Fact]
        public void GetMatch_ExistingMatch_ReturnsMatch()
        {
            Sut.Add(ScheduledMatch);

            var result = Sut.GetMatch(ScheduledMatch.Id);

            result.Should().Be(ScheduledMatch);
        }

        [Fact]
        public void GetMatch_NonExistingMatch_ReturnsNull()
        {
            var result = Sut.GetMatch(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public void Remove_ExistingMatch_ReturnsTrue()
        {
            Sut.Add(ScheduledMatch);

            var result = Sut.Remove(ScheduledMatch.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public void Remove_NonExistingMatch_ReturnsFalse()
        {
            var result = Sut.Remove(Guid.NewGuid());

            result.Should().BeFalse();
        }

        [Fact]
        public void GetAllMatches_ReturnsAllScheduledMatches()
        {
            SetupMultipleScheduledMatches();

            var result = Sut.GetAllMatches();

            result.Should().HaveCount(2);
            result.Should().Contain(Scheduled1);
            result.Should().Contain(Scheduled2);
        }
    }

    public class ScheduledMatchDataSourceDriver
    {
        public IScheduledMatchDataSource Sut { get; }
        public Match ScheduledMatch { get; }
        public Match InvalidMatch { get; }
        public Match Scheduled1 { get; private set; }
        public Match Scheduled2 { get; private set; }

        public ScheduledMatchDataSourceDriver()
        {
            Sut = new ScheduledMatchDataSource();
            ScheduledMatch = Match.CreateScheduled(new Team("Germany"), new Team("Japan"), DateTime.UtcNow.AddHours(1));
            InvalidMatch = new Match(new Team("Brazil"), new Team("Argentina")); // Status: InProgress
        }

        public void SetupMultipleScheduledMatches()
        {
            Scheduled1 = Match.CreateScheduled(new Team("Belgium"), new Team("Canada"), DateTime.UtcNow.AddHours(2));
            Scheduled2 = Match.CreateScheduled(new Team("Croatia"), new Team("Morocco"), DateTime.UtcNow.AddHours(3));
            Sut.Add(Scheduled1);
            Sut.Add(Scheduled2);
        }
    }
}