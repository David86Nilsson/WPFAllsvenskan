using System;
using System.Collections.Generic;
using System.Text;
using WPFAllsvenskan.Enums;

namespace WPFAllsvenskan.Models;

public class Team
{
    public int GrassGames { get; set; }
    public int GrassPoints { get; set; }
    public double PointsPerDifficulty { get; set; }
    public double PointsPerGrassGame { get; set; }
    public double PointsPerPlasticGame { get; set; }
    public double EndPoint { get; set; }
    public double PointsPerGame { get; set; }
    public int PlasticPoints { get; set; }
    public int PlasticGames { get; set; }
    public double SpecialAverage { get; set; }

    public List<Game> PlayedGames { get; set; } = new();
    public List<Game> Schedule { get; set; } = new();
    public int Points { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDiff { get; set; }
    public int Rank { get; set; } = 1;
    public int Games { get; set; }
    public double Difficulty { get; set; }
    public string Pitch { get; set; }
    public string Name { get; set; }

    public Team(string aName)
    {
        Name = aName;
        Pitch = SetPitch();
    }
    public void AddGameToTeam(Game game)
    {
        Schedule.Add(game);
        if (game.played == true)
        {
            int addedPoints = game.GetTeamPointsFromGame(this);
            Points += addedPoints;
            PlayedGames.Add(game);
            Games++;
            if (game.Pitch == "Grass")
            {
                GrassGames++;
                GrassPoints += addedPoints;
            }
            else
            {
                PlasticGames++;
                PlasticPoints += addedPoints;
            }


            if (game.homeTeam == this)
            {
                GoalsFor += game.homeGoals;
                GoalsAgainst += game.awayGoals;
            }
            else
            {
                GoalsFor += game.awayGoals;
                GoalsAgainst += game.homeGoals;
            }
            GoalDiff = GoalsFor - GoalsAgainst;
        }
    }
    public void CalculateTeamStats()
    {
        PointsPerGame = CalculatePointsPerGame(Points, Games);
        PointsPerGrassGame = CalculatePointsPerGame(GrassPoints, GrassGames);
        PointsPerPlasticGame = CalculatePointsPerGame(PlasticPoints, PlasticGames);
        Difficulty = Math.Round(GetFacultyMinusOwnDividedByRounds(Rank) - CalculateAverageOpponent(1, PlayedGames.Count), 2, MidpointRounding.AwayFromZero);
        PointsPerDifficulty = Math.Round(Convert.ToDouble(Points) / Convert.ToDouble(Games) / Difficulty, 2, MidpointRounding.AwayFromZero);
        SpecialAverage = CalculateAverageOpponent(PlayedGames.Count + 1, Schedule.Count);
    }
    private double GetFacultyMinusOwnDividedByRounds(int teamRank)
    {
        double answer = 0;
        for (int i = 1; i <= Schedule.Count / 2; i++)
        {
            answer += i;
        }
        double numberOfTeamsInSerie = Schedule.Count / 2;
        double numberOfGamesInSerie = numberOfTeamsInSerie * (numberOfTeamsInSerie - 1);
        return (answer - teamRank) / (numberOfGamesInSerie / numberOfTeamsInSerie);
    }
    private double CalculatePointsPerGame(int points, int games)
    {
        return Math.Round(Convert.ToDouble(points) / Convert.ToDouble(games), 2, MidpointRounding.AwayFromZero);
    }
    public void EndPoints()
    {
        EndPoint = 0.0;
        foreach (Game aGame in Schedule)
        {
            if (!aGame.played)
            {

                if (aGame.homeTeam.Pitch.Equals("gräs"))
                {
                    EndPoint += PointsPerGrassGame;
                }
                else
                {
                    EndPoint += PointsPerPlasticGame;
                }
            }
        }
        EndPoint = Math.Round(EndPoint + Points, 2, MidpointRounding.AwayFromZero);
    }
    private string SetPitch()
    {
        if (Enum.IsDefined(typeof(PlasticPitch), Name))
        {
            return "Plast";
        }
        else
        {
            return "Grass";
        }
    }
    public void AddToEndPoints(int p)
    {
        EndPoint += p;
    }
    public string GetGamesLeftAsString()
    {
        StringBuilder s = new StringBuilder(Name);
        foreach (Game aGame in Schedule)
        {
            if (!aGame.played)
            {
                s.Append($"\n{aGame.PrintGame()}");
            }
        }
        return s.ToString();
    }
    public string GetScheduleAsString()
    {
        StringBuilder s = new StringBuilder(Name);
        foreach (Game aGame in Schedule)
        {
            s.Append($"\n{aGame.PrintGame()}");
        }
        return s.ToString();
    }
    public string GetTablePrintLine()
    {
        string emptySpaces = "";
        while (Name.Length + emptySpaces.Length < 15)
        {
            emptySpaces += " ";
        }
        return $"\n{Rank}:\t{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
    }

    public double CalculateAverageOpponent(int startO, int endO)
    {
        if (startO > endO)
        {
            startO = endO;
        }
        double total = 0;
        double nbrOfGames = endO - startO + 1;
        foreach (Game game in Schedule)
        {
            if (game.round >= startO && game.round <= endO)
            {
                if (this == game.homeTeam)
                {
                    total += game.awayTeam.Rank;
                }
                else
                {
                    total += game.homeTeam.Rank;
                }
            }
        }
        return Math.Round(total / nbrOfGames, 2, MidpointRounding.AwayFromZero);
    }
}