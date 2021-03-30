using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

internal struct PlayerStats
{
    public string name;
    public int score;

    //constructor
    public PlayerStats(string n, int s)
    {
        name = n;
        score = s;
    }
}

public class Leaderboard
{
    private static Dictionary<string, PlayerStats> lb = new Dictionary<string, PlayerStats>();

    public static void registerPlayer(string name)
    {
        lb.Add(name, new PlayerStats(name, 0));
    }

    public static void setPosition(string name, int score)
    {
        lb[name] = new PlayerStats(name, score);
    }

    public static List<string> GetPlaces()
    {
        List<string> places = new List<string>();

        foreach (KeyValuePair<string, PlayerStats> pos in lb.OrderByDescending(key => key.Value.score))
        {
            places.Add(pos.Value.name);
        }

        return places;
    }

    public static List<int> GetScorePlace()
    {
        List<int> scorePlace = new List<int>();

        foreach (KeyValuePair<string, PlayerStats> pos in lb.OrderByDescending(key => key.Value.score))
        {
            scorePlace.Add(pos.Value.score);
        }

        return scorePlace;
    }
}