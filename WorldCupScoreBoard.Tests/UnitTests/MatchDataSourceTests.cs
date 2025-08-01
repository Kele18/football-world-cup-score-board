using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using FootballWorldCupScoreBoard.Interface;
using FootballWorldCupScoreBoard.Repository;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class MatchDataSourceTests : MatchDataSourceDriver
    {
        [Fact]
        public void Add_SuccessfullyAddedMatch_RetrunTrue()
        {
            var result = Sut.Add(Match);

            result.Should().BeTrue();
        }

        [Fact]
        public void Add_DuplicateMatch_ReturnsFalse()
        {
            SetupDuplicateMatch();

            var result = Sut.Add(Match);

            result.Should().BeFalse();
        }

        [Fact]
        public void Remove_Success_ReturnTrue()
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
        public void GetAllMatches_ReturnsAllMatches()
        {
            SetupMatches();

            var result = Sut.GetAllMatches();

            result.Should().HaveCount(2);
            result.Should().ContainEquivalentOf(Match1);
            result.Should().ContainEquivalentOf(Match2);
        }
    }

    public class MatchDataSourceDriver
    {
        public Match Match { get; }
        public IMatchDataSource Sut { get; }
        public Match Match1 { get; }
        public Match Match2 { get; }

        public MatchDataSourceDriver()
        {
            Match = new Match(new Team("Argentina"), new Team("France"));
            Match1 = new Match(new Team("Portugal"), new Team("Uruguay"));
            Match2 = new Match(new Team("USA"), new Team("Mexico"));
            Sut = new MatchDataSource();
        }

        public void SetupDuplicateMatch()
        {
            Sut.Add(Match);
        }

        public void SetupMatches()
        {
            Sut.Add(Match1);
            Sut.Add(Match2);
        }
    }
}