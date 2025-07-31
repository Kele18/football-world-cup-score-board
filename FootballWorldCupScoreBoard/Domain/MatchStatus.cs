namespace FootballWorldCupScoreBoard.Domain
{
    public enum MatchStatus
    {
        None = 0,
        Scheduled,
        InProgress,
        Finished,
        Cancelled
    }

    public enum TeamSide
    {
        Home,
        Away
    }
}