namespace WPFAllsvenskan.Models
{
    public class SerieMember
    {
        public Team Team { get; set; }
        public int xP { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDiff { get; set; }
        public int Rank { get; set; }
        public int GamesPlayed { get; set; }
        public int PointsOnGrass { get; internal set; }
        public int GamesPlayedOnGrass { get; internal set; }
        public int PointsOnPlastic { get; internal set; }
        public int GamesPlayedOnPlastic { get; internal set; }
        public double Difficulty { get; set; }
        public double SpecialAverage { get; internal set; }
        public int EndPoint { get; internal set; }
    }
}
