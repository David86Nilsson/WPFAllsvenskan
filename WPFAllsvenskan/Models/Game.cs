namespace WPFAllsvenskan.Models
{
    public class Game
    {
        public Team winner { get; set; }
        public int homeGoals { get; set; }
        public int awayGoals { get; set; }
        public int round { get; set; }
        public bool played { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }
        public string Pitch { get; set; }
        public Game(Team hTeam, Team aTeam, int aRound)
        {
            played = false;
            round = aRound;
            homeTeam = hTeam;
            awayTeam = aTeam;
            Pitch = homeTeam.Pitch;
            homeTeam.AddGameToTeam(this);
            awayTeam.AddGameToTeam(this);
        }
        public Game(Team hTeam, Team aTeam, int hGoals, int aGoals, int aRound)
        {
            round = aRound;
            homeTeam = hTeam;
            awayTeam = aTeam;
            Pitch = homeTeam.Pitch;
            SetResult(hGoals, aGoals);
            homeTeam.AddGameToTeam(this);
            awayTeam.AddGameToTeam(this);
        }
        public void SetResult(int hGoals, int aGoals)
        {
            played = true;
            homeGoals = hGoals;
            awayGoals = aGoals;
            if (homeGoals > awayGoals)
            {
                winner = homeTeam;
            }
            else if (homeGoals < awayGoals)
            {
                winner = awayTeam;
            }
            else
            {
                winner = null;
            }
        }
        public string PrintGame()
        {
            if (played)
            {
                return "\n" + round + ":\t" + homeTeam.Name + "\t" + homeGoals + "-" + awayGoals + "\t" + awayTeam.Name;

            }
            else
            {

                return $"Omg:{round} | {homeTeam.Name.Trim()} - {awayTeam.Name.Trim()}";
            }
        }
        public void GuessTheGame(string input)
        {
            if (input == homeTeam.Name)
            {
                homeTeam.AddToEndPoints(3);
            }
            else if (input == awayTeam.Name)
            {
                awayTeam.AddToEndPoints(3);

            }
            else
            {
                homeTeam.AddToEndPoints(1);
                awayTeam.AddToEndPoints(1);
            }
        }

        public int GetTeamPointsFromGame(Team team)
        {
            if (team == winner)
            {
                return 3;
            }
            else if (homeGoals == awayGoals)
            {
                return 1;
            }
            return 0;
        }
    }
}