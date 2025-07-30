namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class Score(int home = 0, int away = 0)
    {
        public int Home { get; private set; } = home;
        public int Away { get; private set; } = away;

        public int Total => Home + Away;

        public void Update(int newHome, int newAway)
        {
            if (newHome < Home || newAway < Away)
                throw new InvalidOperationException("Cannot decrease score.");
            Home = newHome;
            Away = newAway;
        }
    }
}