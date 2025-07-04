// Data model for a football team
public class Team {
    public string TeamName;
    public List<Player> Players;
    public int Budget;
    public League League;      // reference to the league this team is in (if any)
    // League performance stats:
    public int Points;
    public int Wins;
    public int Draws;
    public int Losses;
    public int GoalsFor;
    public int GoalsAgainst;

    public Team(string name, int startingBudget) {
        TeamName = name;
        Budget = startingBudget;
        Players = new List<Player>();
        Points = Wins = Draws = Losses = GoalsFor = GoalsAgainst = 0;
    }

    // Calculate team strength (e.g., sum of all player ratings)
    public int GetTeamStrength() {
        int totalRating = 0;
        foreach (Player p in Players) {
            totalRating += p.OverallRating;
        }
        return totalRating;
    }
}
