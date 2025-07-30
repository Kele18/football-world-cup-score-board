namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class Team(string name)
    {
        public string Name { get; } = name;
    }
}