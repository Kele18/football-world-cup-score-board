using FluentAssertions;
using FootballWorldCupScoreBoard.Domain;
using Match = FootballWorldCupScoreBoard.Domain.Match;

namespace WorldCupScoreBoard.Tests.UnitTests
{
    public class MatchUndoGoalTests : MatchUndoGoalDriver
    {
        [Fact]
        public void UndoGoal_RemovesLastHomeGoal_ShouldRemoveOneGoal()
        {
            Sut.UndoGoal(ActiveMatch.Id, TeamSide.Home, "VAR decision");

            ActiveMatch.Score.Home.Should().Be(1);
            ActiveMatch.GoalEvents.Count(g => g.Side == TeamSide.Home && !g.Undone).Should().Be(1);
        }

        [Fact]
        public void UndoGoal_NoGoalsToUndo_Throws()
        {
            SetupNoGoalsToUndo();

            Action act = () => Sut.UndoGoal(ActiveMatch.Id, TeamSide.Away, "Mistake");

            act.Should().Throw<InvalidOperationException>().WithMessage("No goal to undo.");
        }

        [Fact]
        public void UndoLastGoal_WithExistingHomeGoal_ShouldUndoSuccessfully()
        {
            ActiveMatch.UndoLastGoal(TeamSide.Home, "VAR");

            ActiveMatch.Score.Home.Should().Be(1);
            ActiveMatch.GoalEvents.Count(g => g.Side == TeamSide.Home && !g.Undone).Should().Be(1);
            ActiveMatch.GoalEvents.Count(g => g.Side == TeamSide.Home && g.Undone).Should().Be(1);
        }

        [Fact]
        public void UndoLastGoal_WithNoGoals_ShouldThrow()
        {
            var home = new Team("France");
            var away = new Team("Spain");
            var match = new Match(home, away);
            Action act = () => match.UndoLastGoal(TeamSide.Away);

            act.Should().Throw<InvalidOperationException>().WithMessage("No goal to undo.");
        }

        [Fact]
        public void UndoLastGoal_WithMatchNotInProgress_ShouldThrow()
        {
            ActiveMatch.Finish();

            Action act = () => ActiveMatch.UndoLastGoal(TeamSide.Home);

            act.Should().Throw<InvalidOperationException>().WithMessage("Can only undo goals in an active match.");
        }
    }

    public class MatchUndoGoalDriver : ScoreBoardServiceDriver
    {
        public Match ActiveMatch { get; set; }

        public MatchUndoGoalDriver()
        {
            var home = new Team("France");
            var away = new Team("Spain");

            ActiveMatch = new FootballWorldCupScoreBoard.Domain.Match(home, away);
            ActiveMatch.UpdateScore(2, 1);

            _dataSource.Setup(x => x.GetMatch(ActiveMatch.Id)).Returns(ActiveMatch);
        }

        public void SetupNoGoalsToUndo()
        {
            ActiveMatch = new Match(new Team("Spain"), new Team("France"));
            _dataSource.Setup(x => x.GetMatch(ActiveMatch.Id)).Returns(ActiveMatch);
        }
    }
}