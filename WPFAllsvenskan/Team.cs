using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueHandler;

public class Team
{
    public int grassGames { get; set; }
    public int grassPoints { get; set; }
    public double pPerDiff { get; set; }
    public double pPerGrass { get; set; }
    public double pPerPlastic { get; set; }
    public double endPoint { get; set; }
    public double pPerGame { get; set; }
    public double plastPoints { get; set; }
    public double plastGames { get; set; }
    public double specialaverage { get; set; }

    public List<string> playedTeam { get; set; } = new();
    public List<Game> schedule { get; set; } = new();
    public int points { get; set; }
    public int goalsFor { get; set; }
    public int goalsAgainst { get; set; }
    public int goalDiff { get; set; }
    public int rank { get; set; } = 1;
    public int games { get; set; }
    public double average { get; set; }
    public string pitch { get; set; }
    public string name { get; set; }

    public Team(string aName)
    {
        name = aName;
        pitch = WhatPitch();
    }

    public void addGameToTeam(Game g)
    {
        schedule.Add(g);
        if (g.played == true)
        {
            if (g.homeTeam.name.Equals(name))
            {
                games++;
                if (pitch.Contains("Grass"))
                {
                    grassGames++;
                }
                else
                {
                    plastGames++;
                }
                goalsFor = goalsFor + g.homeGoals;
                goalsAgainst = goalsAgainst + g.awayGoals;
                goalDiff = goalsFor - goalsAgainst;

                if (g.homeGoals == g.awayGoals)
                {
                    points++;
                    endPoint++;
                    if (pitch.Contains("Grass"))
                    {
                        grassPoints++;
                    }
                    else
                    {
                        plastPoints++;
                    }
                }
                else if (g.homeGoals > g.awayGoals)
                {
                    points += 3;
                    endPoint += 3;
                    if (pitch.Contains("Grass"))
                    {
                        grassPoints += 3;
                    }
                    else
                    {
                        plastPoints += 3;
                    }
                }
                playedTeam.Add(g.awayTeam.name);
            }
            else
            {
                games++;
                if (g.homeTeam.pitch.Contains("Grass"))
                {
                    grassGames++;
                }
                else
                {
                    plastGames++;
                }
                goalsFor = goalsFor + g.awayGoals;
                goalsAgainst = goalsAgainst + g.homeGoals;
                goalDiff = goalsFor - goalsAgainst;

                if (g.homeGoals == g.awayGoals)
                {
                    points++;
                    endPoint++;
                    if (g.homeTeam.pitch.Contains("Grass"))
                    {
                        grassPoints++;
                    }
                    else
                    {
                        plastPoints++;
                    }
                }
                else if (g.homeGoals < g.awayGoals)
                {
                    points = points + 3;
                    endPoint += 3;
                    if (g.homeTeam.pitch.Contains("Grass"))
                    {
                        grassPoints += 3;
                    }
                    else
                    {
                        plastPoints += 3;
                    }
                }
                playedTeam.Add(g.homeTeam.name);
            }
        }
    }
    public void playedAGame(int aGoalsFor, int aGoalsAgainst)
    {

    }
    public void setSpecial(double d)
    {
        specialaverage = d;
    }
    public void setAverage(double a)
    {
        average = a;

    }
    public void endPoints()
    {
        endPoint = 0.0;
        foreach (Game aGame in schedule)
        {
            if (!aGame.played)
            {

                if (aGame.homeTeam.pitch.Equals("gräs"))
                {
                    endPoint += pPerGrass;
                }
                else
                {
                    endPoint += pPerPlastic;
                }
            }
        }
        endPoint = Math.Round(endPoint + points, 2, MidpointRounding.AwayFromZero);
    }
    public void printSchedule()
    {
        for (int i = 0; i < schedule.Count; i++)
        {
            System.Console.WriteLine(i + 1 + ": " + schedule[i]);
        }
    }
    public string WhatPitch()
    {
        if (name.Contains("Djur") || name.Contains("cken") || name.Contains("Hammarby") || name.Contains("Elfsborg") || name.Contains("Sirius") || name.Contains("Norrk") || name.Contains("Sundsvall"))
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
        endPoint += p;
    }
    public string GetGamesLeft()
    {
        StringBuilder s = new StringBuilder(name);
        foreach (Game aGame in schedule)
        {
            if (!aGame.played)
            {
                s.Append($"\n{aGame.printGame()}");
            }
        }
        return s.ToString();
    }

}