using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using FootballWorldCupScoreBoard.Repository;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class ArchivedMatchDataSourceTests : ArchivedMatchDataSourceDriver
    {
        [Fact]
        public void Add_ValidMatchToArchieve_ReturnsTrue()
        {
            var result = Sut.Add(Match);

            result.Should().BeTrue();
        }

        [Fact]
        public void Add_MatchAlreadyArhieve_ReturnFalse()
        {
            SetupMatchAlreadyArchieved();

            var result = Sut.Add(Match);

            result.Should().BeFalse();
        }

        [Fact]
        public void Add_InvalidStatusMatch_ThrowsException()
        {
            var act = () => Sut.Add(InvalidMatch);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Only non-active matches can be archived.");
        }

        [Fact]
        public void GetMatch_ExistingMatch_ReturnsMatch()
        {
            Sut.Add(Match);

            var result = Sut.GetMatch(Match.Id);

            result.Should().Be(Match);
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
            Sut.Add(Match);

            var result = Sut.Remove(Match.Id);

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
            SetupMultipleArchievedMatches();

            var result = Sut.GetAllMatches();

            result.Should().HaveCount(2);
            result.Should().Contain(ArchivedMatch1);
            result.Should().Contain(ArchivedMatch2);
        }
    }

    public class ArchivedMatchDataSourceDriver
    {
        public IArchiveMatchDataSource Sut { get; }
        public Match Match { get; }
        public Match InvalidMatch { get; }
        public Match ArchivedMatch1 { get; }
        public Match ArchivedMatch2 { get; }

        public ArchivedMatchDataSourceDriver()
        {
            Sut = new ArchivedMatchDataSource();

            Match = new Match(new Team("Brazil"), new Team("Argentina"), status: MatchStatus.Finished);

            InvalidMatch = new Match(new Team("Brazil"), new Team("Argentina"));

            ArchivedMatch1 = new Match(new Team("Slovenia"), new Team("England"), status: MatchStatus.Finished);
            ArchivedMatch2 = new Match(new Team("Spain"), new Team("Italy"), status: MatchStatus.Finished);
        }

        public void SetupMatchAlreadyArchieved()
        {
            Sut.Add(Match);
        }

        public void SetupMultipleArchievedMatches()
        {
            Sut.Add(ArchivedMatch1);
            Sut.Add(ArchivedMatch2);
        }
    }
}