using System;
using System.Collections.Generic;
using System.Text;

namespace WPFAllsvenskan.Models

{
    public class Serie
    {
        private int maxTeamNameLength = 0;
        private string Name;
        public List<Team> teams { get; set; } = new();
        public List<Game> games { get; set; } = new();
        public List<Game> gamesToGuess { get; set; } = new();
        int nbrOfGames { get; set; }
        int omg { get; set; } = 1;
        int nbrOfTeams { get; set; }
        int hGoals { get; set; } = 0;
        int aGoals { get; set; } = 0;
        string[] lines { get; set; }
        public Serie(string league)
        {
            Name = league;
            string adress = @"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\" + league + ".txt";
            ReadSchedule(adress);
            CalculateTableBetweenRounds(1, (nbrOfTeams - 1) * 2);
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
                    else if (teams[x].Points == teams[i].Points && teams[x].GoalDiff == teams[i].GoalDiff && teams[x].GoalsFor == teams[i].GoalsFor && string.Compare(teams[x].Name, teams[i].Name) > 0)
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
        public string PrintTable(string? pPerGame = null)
        {
            StringBuilder s = new StringBuilder("\tLag \t\t\tM\tPoäng \tp/m \tms \tMotstånd \tKommande\t Slutpoäng");
            if (pPerGame == "Grass")
            {
                s = new StringBuilder("\t Lag \t\t\tM\tPoäng \tgrass \tms \tMotstånd \tKommande\t Slutpoäng");
            }
            else if (pPerGame == "Plastic")
            {
                s = new StringBuilder("\t Lag \t\t\tM\tPoäng \tplastic \tms \tMotstånd \tKommande\t Slutpoäng");
            }
            int place = 0;
            foreach (Team team in teams)
            {
                place++;
                s.Append(team.GetTablePrintLine(maxTeamNameLength, pPerGame));
                if (Name == "Allsvenskan")
                {
                    if (place == 3 || place == 13 || place == 14)
                    {
                        s.Append("\n--------------------------------------------------------------------------------------------------------------------");
                    }
                }
                else if (Name == "Superettan")
                {
                    if (place == 2 || place == 12 || place == 14)
                    {
                        s.Append("\n--------------------------------------------------------------------------------------------------------------------");
                    }
                }
                else if (teams.Count == 20)
                    if (place == 4 || place == 17)
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
                if (game.HomeTeam == home && game.AwayTeam == away)
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
            DateTime date;
            for (int i = 1; i < lines.Length - 2; i++)
            {
                if (lines[i].Contains("Omg"))
                {
                    omg++;
                }
                else if (lines[i].Contains("2022"))
                { // ADDS NEW TEAM
                    char[] dividers = { '-', ' ' };
                    string[] DateStrings = lines[i].Split(dividers);
                    int year = int.Parse(DateStrings[0]);
                    int month = int.Parse(DateStrings[1]);
                    int day = int.Parse(DateStrings[2]);
                    date = new DateTime(year, month, day);

                    Team homeTeam = FindTeam(lines[i + 1]);
                    Team awayTeam = FindTeam(lines[i + 2]);
                    if (homeTeam == null)
                    {
                        homeTeam = new(lines[i + 1]);
                        teams.Add(homeTeam);
                        if (homeTeam.Name.Length > maxTeamNameLength)
                        {
                            maxTeamNameLength = homeTeam.Name.Length;
                        }
                    }
                    if (awayTeam == null)
                    {
                        awayTeam = new(lines[i + 2]);
                        teams.Add(awayTeam);
                        if (awayTeam.Name.Length > maxTeamNameLength)
                        {
                            maxTeamNameLength = awayTeam.Name.Length;
                        }
                    }
                    if (lines.Length > i + 3 && lines[i + 3].Length == 5)
                    {
                        // Adds new Played Game
                        string[] result = lines[i + 3].Split(' ');
                        hGoals = int.Parse(result[0]);
                        aGoals = int.Parse(result[2]);
                        games.Add(new(homeTeam, awayTeam, hGoals, aGoals, omg, date));
                    }
                    else
                    { // adds new Unplayed game
                        games.Add(new(homeTeam, awayTeam, omg, date));
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
                if (!game.Played)
                {
                    if (teamsToGuess.Contains(game.HomeTeam.Name.Trim()) || teamsToGuess.Contains(game.AwayTeam.Name.Trim()))
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

        public void CalculateTableBetweenRounds(int startRound, int endRound)
        {
            foreach (Team team in teams)
            {
                team.CalculateGamesBetweenRounds(startRound, endRound);
                team.CalculateTeamStats();
            }
            SortTable();
        }
    }
}