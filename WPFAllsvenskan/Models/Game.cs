using System;

namespace WPFAllsvenskan.Models
{
    public class Game
    {
        public Team Winner { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public int Round { get; set; }
        public bool Played { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public string Pitch { get; set; }
        public DateTime Date { get; set; }
        public Game(Team hTeam, Team aTeam, int aRound, DateTime date)
        {
            Played = false;
            Round = aRound;
            HomeTeam = hTeam;
            AwayTeam = aTeam;
            Date = date;
            Pitch = HomeTeam.Pitch;
            HomeTeam.AddGameToTeam(this);
            AwayTeam.AddGameToTeam(this);
        }
        public Game(Team hTeam, Team aTeam, int hGoals, int aGoals, int aRound, DateTime date)
        {
            Round = aRound;
            HomeTeam = hTeam;
            AwayTeam = aTeam;
            Date = date;
            Pitch = HomeTeam.Pitch;
            SetResult(hGoals, aGoals);
            HomeTeam.AddGameToTeam(this);
            AwayTeam.AddGameToTeam(this);
        }
        public void SetResult(int hGoals, int aGoals)
        {
            Played = true;
            HomeGoals = hGoals;
            AwayGoals = aGoals;
            if (HomeGoals > AwayGoals)
            {
                Winner = HomeTeam;
            }
            else if (HomeGoals < AwayGoals)
            {
                Winner = AwayTeam;
            }
            else
            {
                Winner = null;
            }
        }
        public string PrintGame()
        {
            if (Played)
            {
                return $"\n{Date.ToShortDateString()} Omg:{Round} : {HomeGoals} - {AwayGoals} \t{HomeTeam.Name} - {AwayTeam.Name}";
            }
            else
            {
                return $"{Date.ToShortDateString()} Omg:{Round} | {HomeTeam.Name.Trim()} - {AwayTeam.Name.Trim()}";
            }
        }
        public void GuessTheGame(string input)
        {
            if (input == HomeTeam.Name)
            {
                HomeTeam.AddToEndPoints(3);
            }
            else if (input == AwayTeam.Name)
            {
                AwayTeam.AddToEndPoints(3);

            }
            else
            {
                HomeTeam.AddToEndPoints(1);
                AwayTeam.AddToEndPoints(1);
            }
        }

        public int GetTeamPointsFromGame(Team team)
        {
            if (team == Winner)
            {
                return 3;
            }
            else if (HomeGoals == AwayGoals)
            {
                return 1;
            }
            return 0;
        }
    }
}