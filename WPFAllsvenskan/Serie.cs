using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueHandler

{
    public class Serie
    {
        public List<Team> teams { get; set; } = new();
        public List<Game> games { get; set; } = new();
        public List<Game> gamesToGuess { get; set; } = new();
        int nbrOfGames { get; set; }
        int omg { get; set; } = 1;
        int nbrOfTeams { get; set; }
        int hGoals { get; set; } = 0;
        int aGoals { get; set; } = 0;
        string[] lines { get; set; }
        public Serie()
        {
            string adress = @"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\Schema.txt";
            ReadSchedule(adress);
            SortTable();
            ResetTeams();
            CalculateStats();
            AverageOpponentInUpcomingGames(1);
        }

        public void SortTable()
        {
            int i = 0;

            while (i < nbrOfTeams)
            {
                for (int x = 0; x < nbrOfTeams; x++)
                {
                    if (teams[x].Points < teams[i].Points)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].Points == teams[i].Points && teams[x].GoalDiff < teams[i].GoalDiff)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].Points == teams[i].Points && teams[x].GoalDiff == teams[i].GoalDiff && teams[x].GoalsFor < teams[i].GoalsFor)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].Points == teams[i].Points && teams[x].GoalDiff == teams[i].GoalDiff && teams[x].GoalsFor == teams[i].GoalsFor && String.Compare(teams[x].Name, teams[i].Name) > 0)
                    {
                        SwitchPosition(i, x);
                    }
                }
                i++;
            }
            for (int y = 1; y <= nbrOfTeams; y++)// Updates Team Rankings;
            {
                teams[y - 1].Rank = y;
            }
        }
        public string PrintTable()
        {
            StringBuilder s = new StringBuilder("\t Lag \t\tM\tPoäng \tp/m \tms \tMotstånd \tKommande\t Slutpoäng");
            int place = 0;
            foreach (Team team in teams)
            {
                place++;
                s.Append(team.GetTablePrintLine());
                if (place == 3 || place == 13 || place == 14)
                {
                    s.Append("\n--------------------------------------------------------------------------------------------------------------------");
                }
            }
            return s.ToString();
        }
        public Team FindTeam(string teamName)
        {
            foreach (Team team in teams)
            {
                if (team.Name == teamName)
                {
                    return team;
                }
            }
            return null;
        }
        public Game FindGame(string hTeamName, string aTeamName)
        {
            Team home = FindTeam(hTeamName);
            Team away = FindTeam(aTeamName);

            foreach (Game game in games)
            {
                if (game.homeTeam == home && game.awayTeam == away)
                {
                    return game;
                }
            }
            return null;
        }
        public void AverageOpponentInUpcomingGames(int upcoming)
        {
            foreach (Team team in teams)
            {
                team.SpecialAverage = team.CalculateAverageOpponent(team.PlayedGames.Count + 1, team.PlayedGames.Count + upcoming);
            }
        }
        public void AverageOpponentBetweenRounds(int startO, int endO)
        {
            foreach (Team team in teams)
            {
                team.SpecialAverage = team.CalculateAverageOpponent(startO, endO);
            }
        }
        public string PrintTeamSchedule(string teamName)
        {
            Team t = FindTeam(teamName);
            StringBuilder schedule = new StringBuilder();
            foreach (Game game in t.Schedule)
            {
                schedule.Append(game.PrintGame());
            }
            return schedule.ToString();
        }
        private void ReadSchedule(string adress)
        {
            lines = System.IO.File.ReadAllLines(adress);
            omg = 1;
            for (int i = 1; i < lines.Length - 2; i++)
            {
                if (lines[i].Contains("Omg"))
                {
                    omg++;
                }
                else if (lines[i].Contains("2022"))
                { // ADDS NEW TEAM
                    Team homeTeam = FindTeam(lines[i + 1]);
                    Team awayTeam = FindTeam(lines[i + 2]);
                    if (homeTeam == null)
                    {
                        homeTeam = new(lines[i + 1]);
                        teams.Add(homeTeam);
                    }
                    if (awayTeam == null)
                    {
                        awayTeam = new(lines[i + 2]);
                        teams.Add(awayTeam);
                    }
                    if (lines.Length > i + 3 && lines[i + 3].Length == 5)
                    {
                        // Adds new Played Game
                        char[] result;
                        result = lines[i + 3].ToCharArray();
                        hGoals = Convert.ToInt32(result[0]);
                        aGoals = Convert.ToInt32(result[4]);
                        games.Add(new(homeTeam, awayTeam, hGoals, aGoals, omg));
                    }
                    else
                    { // adds new Unplayed game
                        games.Add(new(homeTeam, awayTeam, omg));
                    }
                }

                else if (lines[i].Contains("-"))
                {
                }
            }
            nbrOfGames = games.Count;
            nbrOfTeams = teams.Count;
        }
        public void CalculateStats()
        {
            foreach (Team team in teams)
            {
                team.CalculateTeamStats();
            }
        }
        private void SwitchPosition(int a, int b)
        {
            Team temp = teams[a];
            teams[a] = teams[b];
            teams[b] = temp;
        }
        public void GuessTheFinish(List<string> teamsToGuess)
        {
            gamesToGuess = new();
            ResetTeams();
            foreach (Game game in games)
            {
                if (!game.played)
                {
                    if (teamsToGuess.Contains(game.homeTeam.Name.Trim()) || teamsToGuess.Contains(game.awayTeam.Name.Trim()))
                    {
                        gamesToGuess.Add(game);
                    }
                }
            }
        }
        private void ResetTeams()
        {
            foreach (Team team in teams)
            {
                team.EndPoint = team.Points;
            }
        }

        public void SortByEndPoints()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams.Count; j++)
                {
                    if (teams[j].EndPoint < teams[i].EndPoint)
                    {
                        SwitchPosition(i, j);
                    }
                }
            }
        }
    }
}