namespace FootballWorldCupScoreBoard.Domain
{
    public enum MatchStatus
    {
        None = 0,
        Scheduled,
        InProgress,
        Finished,
        Cancelled,
        Abandoned
    }

    public enum TeamSide
    {
        Home,
        Away
    }
}