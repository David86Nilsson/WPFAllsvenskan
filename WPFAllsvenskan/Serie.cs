using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueHandler

{
    public class Serie
    {
        public List<Team> teams { get; set; } = new();
        public List<Game> games { get; set; } = new();
        public List<Game> gamesToGuess { get; set; }
        int nbrOfGames { get; set; } = 0;
        int gameNbr { get; set; } = 0;
        Game temp { get; set; } = new();
        int omg { get; set; } = 1;
        int nbrOfTeams { get; set; }
        int hGoals { get; set; } = 0;
        int aGoals { get; set; } = 0;
        string[] lines { get; set; }
        public Serie()
        {
            string adress = @"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\Schema.txt";

            gamesToGuess = new();

            ReadSchedule(adress);
            CountPointsPerGame();
            averageOpponent();
        }

        public void sortTable()
        {
            int i = 0;
            while (i < 16)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (teams[x].points < teams[i].points)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].points == teams[i].points && teams[x].goalDiff < teams[i].goalDiff)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].points == teams[i].points && teams[x].goalDiff == teams[i].goalDiff && teams[x].goalsFor < teams[i].goalsFor)
                    {
                        SwitchPosition(i, x);
                    }
                    else if (teams[x].points == teams[i].points && teams[x].goalDiff == teams[i].goalDiff && teams[x].goalsFor == teams[i].goalsFor && String.Compare(teams[x].name, teams[i].name) > 0)
                    {
                        SwitchPosition(i, x);
                    }
                }
                i++;
            }
            for (int y = 0; y < 16; y++)
            { // Updates Team Rankings;
                teams[y].rank = y + 1;
            }
        }
        public void sortTable(int nbr)
        {
            //Team temp = new Team("temp");
            AverageOpponent(nbr);
            //for(int i = 0; i< teams.Length;i++){
            //    for(int x = 0;x<teams.Length;x++){
            //       if(teams[x].specialaverage > teams[i].specialaverage){
            //            temp=teams[i];
            //            teams[i]=teams[x];
            //            teams[x]=temp;
            //       } 
            //   }
            //}
            //for(int y=0;y<16;y++){
            //    teams[y].rank=y+1;
            //}
        }
        public void sortTable(int startO, int endO)
        {
            averageOpponent(startO, endO);
            for (int i = 0; i < teams.Count; i++)
            {
                for (int x = 0; x < teams.Count; x++)
                {
                    if (teams[x].specialaverage > teams[i].specialaverage)
                    {
                        Team temp = teams[i];
                        teams[i] = teams[x];
                        teams[x] = temp;
                    }
                }
            }
            //for(int y=0;y<16;y++){
            //    teams[y].rank=y+1;
            //}
        }
        public string PrintTable()
        {
            StringBuilder s = new StringBuilder("\t Lag \t\tM\tPoäng \tp/m \tms \tMotstånd \tKommande\t Slutpoäng");
            for (int j = 0; j < 16; j++)
            {
                while (teams[j].name.Length < 15)
                {
                    teams[j].name += " ";
                }
                if (j == 3 || j == 13 || j == 14)
                {
                    s.Append("\n--------------------------------------------------------------------------------------------------------------------");
                }
                //s.Append("\n" + teams[j].rank + ":\t " + teams[j].name + "\t" + teams[j].games + "\t" + teams[j].points + "\t" + teams[j].pPerGame + "\t"
                //                      + teams[j].goalDiff + "\t" + teams[j].average + "\t\t" + teams[j].specialaverage + "\t\t" + teams[j].endPoint);
                s.Append($"\n{teams[j].rank}:\t{teams[j].name}\t{teams[j].games}\t{teams[j].points}\t{teams[j].pPerGame}\t{teams[j].goalDiff}\t{teams[j].average}\t\t{teams[j].specialaverage}\t\t{teams[j].endPoint}");
            }
            return s.ToString();
        }
        public Team findTeam(string teamName)
        {
            for (int j = 0; j < nbrOfTeams; j++)
            {
                if (teamName.Equals(teams[j].name.Trim()))
                {
                    return teams[j];
                }
            }
            return null;
        }
        public Game findGame(string hTeamName, string aTeamName)
        {
            bool isFound = false;
            int i = 0;
            //Team home = findTeam(hTeamName);
            //Team away = findTeam(aTeamName);
            gameNbr = 0;
            while (isFound == false && i < nbrOfGames)
            {
                if (games[i] == null)
                {
                    break;
                }
                else if (games[i].homeTeam.name.Equals(hTeamName) && games[i].awayTeam.name.Equals(aTeamName))
                {
                    isFound = true;
                    gameNbr = i;
                    //System.Console.WriteLine(i);
                    return games[i];
                }
                i++;
            }
            if (gameNbr >= nbrOfGames)
            {
                gameNbr = nbrOfGames - 1;
            }
            return games[gameNbr];
        }
        public void averageOpponent()
        {
            sortTable();
            double total = 0;
            double av = 0;
            double nbrOfTeams = 0;
            int number = 0;
            int upcoming = 0;
            Team temp = new Team("temp");
            Game tempGame;
            for (int i = 0; i < teams.Count; i++)
            {
                total = 0;
                nbrOfTeams = teams[i].playedTeam.Count;
                number = 0;
                for (int j = 0; j < nbrOfTeams; j++)
                {
                    temp = findTeam(teams[i].playedTeam[j]);
                    total += Convert.ToDouble(temp.rank);
                }
                av = total / nbrOfTeams - (272.0 - 2 * Convert.ToDouble(teams[i].rank)) / 30.0;
                teams[i].average = Math.Round(av, 2, MidpointRounding.AwayFromZero);
                teams[i].pPerDiff = Math.Round(Convert.ToDouble(teams[i].points) / Convert.ToDouble(teams[i].games) / teams[i].average, 2, MidpointRounding.AwayFromZero);
                upcoming = teams[i].schedule.Count - teams[i].playedTeam.Count;
                total = 0;
                for (int j = (int)nbrOfTeams; j < teams[i].schedule.Count; j++)
                {
                    tempGame = teams[i].schedule[j];
                    temp = findTeam(tempGame.awayTeam.name);
                    total += Convert.ToDouble(temp.rank);
                    number++;
                }
                av = total / upcoming;
                teams[i].specialaverage = Math.Round(av, 2, MidpointRounding.AwayFromZero);

            }
        }
        public void AverageOpponent(int upcoming)
        {
            sortTable();

            for (int i = 0; i < teams.Count; i++)
            {
                double total = 0;
                int number = 0;
                for (int j = 0; j < teams[i].schedule.Count; j++)
                {
                    Game tempGame = teams[i].schedule[j];

                    if (tempGame.played == false && number < upcoming && tempGame.homeTeam.name.Equals(teams[i].name))
                    { // Adds opponent rank 
                        total += Convert.ToDouble(tempGame.awayTeam.rank);
                        number++;
                    }
                    else if (tempGame.played == false && number < upcoming && tempGame.awayTeam.name.Equals(teams[i].name))
                    { // Adds opponent rank
                        total += Convert.ToDouble(tempGame.homeTeam.rank);
                        number++;
                    }

                    teams[i].specialaverage = Math.Round(total / number, 2, MidpointRounding.AwayFromZero);

                }
            }
        }
        public void averageOpponent(int startO, int endO)
        {
            sortTable();
            double total = 0;
            double av = 0;
            double nbrOfTeams = endO - startO + 1;
            Team temp = new Team("temp");
            Game g = new Game();
            for (int i = 0; i < teams.Count; i++)
            {
                total = 0;
                //nbrOfTeams=teams[i].schedule.Count;
                for (int j = startO - 1; j < endO; j++)
                {
                    g = teams[i].schedule[j];
                    if (g.homeTeam.Equals(teams[i].name))
                    {
                        temp = findTeam(g.awayTeam.name);
                    }
                    else
                    {
                        temp = findTeam(g.homeTeam.name);
                    }
                    total += Convert.ToDouble(temp.rank);
                }
                av = total / nbrOfTeams;
                teams[i].specialaverage = Math.Round(av, 2, MidpointRounding.AwayFromZero);
            }
        }
        public string printSchedule(string s)
        {
            Team t = findTeam(s);
            Game tempGame;
            StringBuilder schedule = new StringBuilder();
            for (int i = 0; i < t.schedule.Count; i++)
            {
                tempGame = t.schedule[i];
                schedule.Append(tempGame.printGame());
            }
            return schedule.ToString();

            //System.Console.WriteLine(i+1 + ": " + t.schedule[i]);
        }
        public string findPitch(Team t)
        {
            string s = t.name;
            if (s.Contains("AIK") || s.Contains("Malmö FF") || s.Contains("IFK Göteborg") || s.Contains("Kalmar FF") || s.Contains("Mjällby") || s.Contains("Varbergs BoIS") || s.Contains("IFK Värnamo") || s.Contains("Degerfors") || s.Contains("Helsingborg"))
            {
                return "gräs";
            }
            else
            {
                return "plast";
            }

        }
        public void ReadSchedule(string adress)
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
                    Team homeTeam = findTeam(lines[i + 1]);
                    Team awayTeam = findTeam(lines[i + 2]);
                    if (homeTeam == null)
                    {
                        homeTeam = new(lines[i + 1]);
                        teams.Add(homeTeam);
                        nbrOfTeams++;
                    }
                    if (awayTeam == null)
                    {
                        awayTeam = new(lines[i + 2]);
                        teams.Add(awayTeam);
                        nbrOfTeams++;
                    }
                    if (lines.Length > i + 3)
                    {
                        if (lines[i + 3].Length == 5)
                        { // Adds new Played Game
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
                    else
                    { // adds new Unplayed game
                        games.Add(new(homeTeam, awayTeam, omg));
                    }
                }

                else if (lines[i].Contains("-"))
                {
                }
            }
        }
        public void CountPointsPerGame()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                teams[i].pPerGame = Math.Round(Convert.ToDouble(teams[i].points) / Convert.ToDouble(teams[i].games), 2, MidpointRounding.AwayFromZero);
                teams[i].pPerGrass = Math.Round(Convert.ToDouble(teams[i].grassPoints) / Convert.ToDouble(teams[i].grassGames), 2, MidpointRounding.AwayFromZero);
                teams[i].pPerPlastic = Math.Round(teams[i].plastPoints / teams[i].plastGames, 2, MidpointRounding.AwayFromZero);
            }
        }
        private void SwitchPosition(int a, int b)
        {
            Team temp = new Team("temp");
            temp = teams[a];
            teams[a] = teams[b];
            teams[b] = temp;
        }
        public void GuessTheFinish(int topTeams)
        {
            gamesToGuess = new();
            ResetTeams();
            foreach (Game game in games)
            {
                if (!game.played)
                {
                    if (game.homeTeam.rank <= topTeams || game.awayTeam.rank <= topTeams)
                    {
                        gamesToGuess.Add(game);
                    }
                }
            }
        }
        public void GuessTheFinish(List<string> teamsToGuess)
        {
            gamesToGuess = new();
            ResetTeams();
            foreach (Game game in games)
            {
                if (!game.played)
                {
                    if (teamsToGuess.Contains(game.homeTeam.name.Trim()) || teamsToGuess.Contains(game.awayTeam.name.Trim()))
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
                team.endPoint = team.points;
            }
        }

        public void SortByEndPoints()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams.Count; j++)
                {
                    if (teams[j].endPoint < teams[i].endPoint)
                    {
                        SwitchPosition(i, j);
                    }
                }
            }
        }
    }
}