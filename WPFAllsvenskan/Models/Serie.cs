using System.Collections.Generic;
using System.Text;

namespace WPFAllsvenskan.Models

{
    public class Serie
    {
        public string Year { get; set; }
        public int maxTeamNameLength { get; set; } = 0;
        public League League { get; set; }
        public List<SerieMember> SerieMembers { get; set; } = new();
        public List<Game> Games { get; set; } = new();
        //public List<Team> Teams { get; set; } = new();
        public List<Game> GamesToGuess { get; set; } = new();
        public int nbrOfGames { get; set; }
        public int omg { get; set; } = 1;
        public int NbrOfTeams { get; set; }
        public int hGoals { get; set; } = 0;
        public int aGoals { get; set; } = 0;
        public string[] lines { get; set; }
        public Serie()
        {

        }
        public void SortTable()
        {
            //int i = 0;

            //while (i < NbrOfTeams)
            //{
            //    for (int x = 0; x < NbrOfTeams; x++)
            //    {
            //        if (Teams[x].Points < Teams[i].Points)
            //        {
            //            SwitchPosition(i, x);
            //        }
            //        else if (Teams[x].Points == Teams[i].Points && Teams[x].GoalDiff < Teams[i].GoalDiff)
            //        {
            //            SwitchPosition(i, x);
            //        }
            //        else if (Teams[x].Points == Teams[i].Points && Teams[x].GoalDiff == Teams[i].GoalDiff && Teams[x].GoalsFor < Teams[i].GoalsFor)
            //        {
            //            SwitchPosition(i, x);
            //        }
            //        else if (Teams[x].Points == Teams[i].Points && Teams[x].GoalDiff == Teams[i].GoalDiff && Teams[x].GoalsFor == Teams[i].GoalsFor && string.Compare(Teams[x].Name, Teams[i].Name) > 0)
            //        {
            //            SwitchPosition(i, x);
            //        }
            //    }
            //    i++;
            //}
            //for (int y = 1; y <= NbrOfTeams; y++)// Updates Team Rankings;
            //{
            //    Teams[y - 1].Rank = y;
            //}
        }

        public string PrintTable(string? pPerGame = null)
        {
            StringBuilder s = new StringBuilder("\tLag \t\t\tM\tPoäng \tp/m \tms \tMotstånd \tKommande\t Slutpoäng");
            //if (pPerGame == "Grass")
            //{
            //    s = new StringBuilder("\t Lag \t\t\tM\tPoäng \tgrass \tms \tMotstånd \tKommande\t Slutpoäng");
            //}
            //else if (pPerGame == "Plastic")
            //{
            //    s = new StringBuilder("\t Lag \t\t\tM\tPoäng \tplastic \tms \tMotstånd \tKommande\t Slutpoäng");
            //}
            //int place = 0;
            //foreach (Team team in Teams)
            //{
            //    place++;
            //    s.Append(team.GetTablePrintLine(maxTeamNameLength, pPerGame));
            //    if (Name == "Allsvenskan")
            //    {
            //        if (place == 3 || place == 13 || place == 14)
            //        {
            //            s.Append("\n--------------------------------------------------------------------------------------------------------------------");
            //        }
            //    }
            //    else if (Name == "Superettan")
            //    {
            //        if (place == 2 || place == 12 || place == 14)
            //        {
            //            s.Append("\n--------------------------------------------------------------------------------------------------------------------");
            //        }
            //    }
            //    else if (Teams.Count == 20)
            //        if (place == 4 || place == 17)
            //        {
            //            s.Append("\n--------------------------------------------------------------------------------------------------------------------");
            //        }
            //}
            return s.ToString();
        }
        public void AverageOpponentInUpcomingGames(int upcoming)
        {
            foreach (SerieMember serieMember in SerieMembers)
            {
                //serieMember.SpecialAverage = team.CalculateAverageOpponent(team.PlayedGames.Count + 1, team.PlayedGames.Count + upcoming);
            }
        }


    }
}