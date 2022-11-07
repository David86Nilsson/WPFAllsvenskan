namespace LeagueHandler
{
    public class Game
    {
        private int winner;

        public int homeGoals { get; set; }
        public int awayGoals { get; set; }
        public int round { get; set; }
        public bool played { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }

        public Game()
        {
            homeGoals = 0;
            awayGoals = 0;
            played = false;
            winner = 0;
            round = 0;
            homeTeam = new Team("temp");
            awayTeam = new Team("temp");
        }
        public Game(Team hTeam, Team aTeam, int aRound)
        {
            homeGoals = 0;
            awayGoals = 0;
            played = false;
            round = aRound;
            homeTeam = hTeam;
            awayTeam = aTeam;
            homeTeam.addGameToTeam(this);
            awayTeam.addGameToTeam(this);
        }
        public Game(Team hTeam, Team aTeam, int hGoals, int aGoals, int omg)
        {
            setResult(hGoals, aGoals);
            round = omg;
            homeTeam = hTeam;
            awayTeam = aTeam;
            homeTeam.addGameToTeam(this);
            awayTeam.addGameToTeam(this);
        }
        public void setResult(int hGoals, int aGoals)
        {
            homeGoals = hGoals;
            awayGoals = aGoals;
            played = true;
        }
        public string printGame()
        {
            if (played)
            {
                return ("\n" + round + ":\t" + homeTeam.name + "\t" + homeGoals + "-" + awayGoals + "\t" + awayTeam.name);

            }
            else
            {

                return $"Omg:{round} | {homeTeam.name.Trim()} - {awayTeam.name.Trim()}";
            }
        }
        public void GuessTheGame(string input)
        {
            if (input == homeTeam.name)
            {
                homeTeam.AddToEndPoints(3);
            }
            else if (input == awayTeam.name)
            {
                awayTeam.AddToEndPoints(3);

            }
            else
            {
                homeTeam.AddToEndPoints(1);
                awayTeam.AddToEndPoints(1);
            }
        }
    }
}