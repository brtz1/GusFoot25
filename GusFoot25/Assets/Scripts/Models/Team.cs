using System.Collections.Generic;

public class Team {
    public string TeamName;
    public List<Player> Players;
    public int Budget;
    public League League;      // reference to its league (null for district teams)

    // Season stats
    public int Points, Wins, Draws, Losses, GoalsFor, GoalsAgainst;
    public int Morale;         // 0–100, starts at 50

    public Team(string name, int startingBudget) {
        TeamName = name;
        Budget = startingBudget;
        Players = new List<Player>();
        Points = Wins = Draws = Losses = GoalsFor = GoalsAgainst = 0;
        Morale = 50;
    }

    public int GetTeamStrength() {
        int total = 0;
        foreach (var p in Players) total += p.OverallRating;
        return total;
    }
}
