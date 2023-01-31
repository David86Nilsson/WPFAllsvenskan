using System;

namespace WPFAllsvenskan.Models
{
    public class Game
    {
        public Team Winner { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public int Round { get; set; }
        public string Odds1 { get; set; }
        public string OddsX { get; set; }
        public string Odds2 { get; set; }
        public double OddsChances1 { get; set; }
        public double OddsChancesX { get; set; }
        public double OddsChances2 { get; set; }
        public double Chances1 { get; set; }
        public double ChancesX { get; set; }
        public double Chances2 { get; set; }
        public bool IsPlayed { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public string Pitch { get; set; }
        public DateTime Date { get; set; }
        public Serie? Serie { get; set; }
        public Game(Team hTeam, Team aTeam, int aRound, DateTime date, Serie serie)
        {
            IsPlayed = false;
            Round = aRound;
            HomeTeam = hTeam;
            AwayTeam = aTeam;
            Date = date;
            Pitch = HomeTeam.Pitch;
            Serie = serie;
            HomeTeam.Games.Add(this);
            AwayTeam.Games.Add(this);
        }
        public Game(Team hTeam, Team aTeam, int hGoals, int aGoals, int aRound, DateTime date, Serie serie)
        {
            Serie = serie;
            Round = aRound;
            HomeTeam = hTeam;
            AwayTeam = aTeam;
            Date = date;
            Pitch = HomeTeam.Pitch;
            IsPlayed = true;
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

            HomeTeam.Games.Add(this);
            HomeTeam.PlayedGames.Add(this);

            AwayTeam.Games.Add(this);
            AwayTeam.PlayedGames.Add(this);
        }
        public string PrintGame()
        {
            if (IsPlayed)
            {
                return $"\n{Date.ToShortDateString()} Omg:{Round} : {HomeGoals} - {AwayGoals} \t{HomeTeam.Name} - {AwayTeam.Name}";
            }
            else
            {
                return $"{Date.ToShortDateString()} Omg:{Round} | {HomeTeam.Name.Trim()} - {AwayTeam.Name.Trim()} | {Odds1} - {OddsX} - {Odds2} | {OddsChances1}% - {OddsChancesX}% - {OddsChances2}%";
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