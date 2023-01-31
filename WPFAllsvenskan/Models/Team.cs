using System;
using System.Collections.Generic;
using System.Text;
using WPFAllsvenskan.Enums;

namespace WPFAllsvenskan.Models;

public class Team
{
    public List<Game> PlayedGames { get; set; } = new();
    public List<Game> Games { get; set; } = new();
    public string Pitch { get; set; }
    public string Name { get; set; }

    public Team(string aName)
    {
        Name = aName;
        Pitch = SetPitch();
    }
    public void AddGameToTeam(Game game)
    {
        Games.Add(game);
        if (game.IsPlayed == true)
        {
            PlayedGames.Add(game);
        }
    }
    private double GetFacultyMinusOwnDividedByRounds(int teamRank)
    {
        double answer = 0;
        for (int i = 1; i <= Games.Count / 2; i++)
        {
            answer += i;
        }
        double numberOfTeamsInSerie = Games.Count / 2;
        double numberOfGamesInSerie = numberOfTeamsInSerie * (numberOfTeamsInSerie - 1);
        return (answer - teamRank) / (numberOfGamesInSerie / numberOfTeamsInSerie);
    }
    private double CalculatePointsPerGame(int points, int games)
    {
        return Math.Round(Convert.ToDouble(points) / Convert.ToDouble(games), 2, MidpointRounding.AwayFromZero);
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
    public string GetGamesLeftAsString()
    {
        StringBuilder s = new StringBuilder();
        foreach (Game aGame in Games)
        {
            if (!aGame.IsPlayed)
            {
                s.Append($"\n{aGame.PrintGame()}");
            }
        }
        return s.ToString();
    }
    public string GetScheduleAsString()
    {
        StringBuilder s = new StringBuilder();
        foreach (Game aGame in Games)
        {
            s.Append($"\n{aGame.PrintGame()}");
        }
        return s.ToString();
    }

}