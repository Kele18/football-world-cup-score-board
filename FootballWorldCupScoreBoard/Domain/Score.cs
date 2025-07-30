namespace FootballWorldCupScoreBoard.Domain
{
    public sealed class Score
    {
        public int Home { get; private set; }
        public int Away { get; private set; }

        public Score(int home = 0, int away = 0)
        {
            if (home < 0 || away < 0) throw new ArgumentOutOfRangeException("Score cannot be negative.");
            Home = home;
            Away = away;
        }

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