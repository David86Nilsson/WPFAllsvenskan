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
        if (game.Played == true)
        {
            PlayedGames.Add(game);
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
            if (!aGame.Played)
            {

                if (aGame.HomeTeam.Pitch.Equals("gräs"))
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
        if (Enum.IsDefined(typeof(PlasticPitch), Name.Replace(' ', '_')))
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
        StringBuilder s = new StringBuilder();
        foreach (Game aGame in Schedule)
        {
            if (!aGame.Played)
            {
                s.Append($"\n{aGame.PrintGame()}");
            }
        }
        return s.ToString();
    }
    public string GetScheduleAsString()
    {
        StringBuilder s = new StringBuilder();
        foreach (Game aGame in Schedule)
        {
            s.Append($"\n{aGame.PrintGame()}");
        }
        return s.ToString();
    }
    public string GetTablePrintLine(int maxTeamNameLength, string? pPerGame = null)
    {
        string emptySpaces = "";
        while (Name.Length + emptySpaces.Length < maxTeamNameLength + 10)
        {
            emptySpaces += " ";
        }
        if (pPerGame == "Grass")
        {
            return $"\n{Rank}:\t{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerGrassGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
        }
        else if (pPerGame == "Plastic")
        {
            return $"\n{Rank}:\t{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerPlasticGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
        }
        return $"\n{Rank}:\t{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
    }

    public double CalculateAverageOpponent(int startO, int endO)
    {
        if (startO > endO)
        {
            startO = endO;
        }
        if (endO > Schedule.Count)
        {
            endO = Schedule.Count;
        }
        double total = 0;
        int games = 0;
        double nbrOfGames = Convert.ToDouble(endO) - Convert.ToDouble(startO) + 1;
        foreach (Game game in Schedule)
        {
            if (game.Round >= startO && game.Round <= endO)
            {
                if (this == game.HomeTeam)
                {
                    total += game.AwayTeam.Rank;
                }
                else
                {
                    total += game.HomeTeam.Rank;
                }
                games++;
            }
        }
        return Math.Round(total / nbrOfGames, 2, MidpointRounding.AwayFromZero);
    }

    public void CalculateGamesBetweenRounds(int startRound, int endRound)
    {
        ResetStats();
        if (endRound > PlayedGames.Count)
        {
            endRound = PlayedGames.Count;
        }
        for (int i = startRound - 1; i < endRound; i++)
        {
            Game game = PlayedGames[i];
            int addedPoints = game.GetTeamPointsFromGame(this);
            Points += addedPoints;
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


            if (game.HomeTeam == this)
            {
                GoalsFor += game.HomeGoals;
                GoalsAgainst += game.AwayGoals;
            }
            else
            {
                GoalsFor += game.AwayGoals;
                GoalsAgainst += game.HomeGoals;
            }
        }
        GoalDiff = GoalsFor - GoalsAgainst;
    }

    private void ResetStats()
    {
        Points = 0;
        GrassPoints = 0;
        PlasticPoints = 0;
        Games = 0;
        GrassGames = 0;
        PlasticGames = 0;
        GoalsFor = 0;
        GoalsAgainst = 0;
        GoalDiff = 0;
    }
}