using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public League league;
    public Cup cup;

    // UI references (assigned via Inspector for simplicity)
    public Text matchResultText;   // UI Text to display the latest match result
    public TeamViewUI teamViewPanel; // A UI component to show team details (players list)

    void Start() {
        InitializeGameData();
        // Optionally, display initial team info or standings
        if (teamViewPanel != null) {
            teamViewPanel.ShowTeam(league.Teams[0]);  // show first team as an example
        }
    }

    // Set up a simple league with dummy teams and players
    void InitializeGameData() {
        // Create league
        league = new League("Demo League");

        // Create 4 teams with dummy data
        Team teamA = new Team("Team A", startingBudget: 1000000);
        Team teamB = new Team("Team B", startingBudget: 1000000);
        Team teamC = new Team("Team C", startingBudget: 1000000);
        Team teamD = new Team("Team D", startingBudget: 1000000);

        // Add some dummy players to each team
        // (In a full game, we'd load this data from a database or scriptable objects)
        teamA.Players.Add(new Player("Alice", 25, "FWD", rating: 80, value: 500000));
        teamA.Players.Add(new Player("Bob", 28, "MID", rating: 75, value: 300000));
        // ... (add more players to Team A)
        teamB.Players.Add(new Player("Charlie", 22, "FWD", rating: 78, value: 400000));
        teamB.Players.Add(new Player("David", 30, "DEF", rating: 82, value: 600000));
        // ... (and so on for team C and D, each with a few players)

        // Register teams in the league
        league.Teams.AddRange(new Team[] { teamA, teamB, teamC, teamD });
        // Link teams back to the league (optional)
        foreach (Team t in league.Teams) {
            t.League = league;
        }

        // Generate league fixtures for the season
        league.GenerateFixtures();

        // Initialize a cup with the same teams (for demonstration)
        cup = new Cup("Demo Cup", league.Teams);
        cup.StartCup();
    }

    // Simulate the next match day (next fixture in the league schedule)
    public void SimulateNextMatchDay() {
        if (!league.HasMoreMatches()) {
            Debug.Log("Season is over. No more matches to simulate.");
            return;
        }
        // Get next match(es) for the round
        Match match = league.GetNextMatch();
        SimulateMatch(match);
        // Update UI with the result
        if (matchResultText != null) {
            Team home = match.HomeTeam;
            Team away = match.AwayTeam;
            matchResultText.text = $"{home.TeamName} {match.HomeScore} - {match.AwayScore} {away.TeamName}";
        }
        // (Optional: update standings UI or other UI elements here)
    }

    // Simulate a single match between two teams and update league standings
    private void SimulateMatch(Match match) {
        Team home = match.HomeTeam;
        Team away = match.AwayTeam;
        int homeStrength = home.GetTeamStrength();
        int awayStrength = away.GetTeamStrength();

        // Simple match outcome algorithm:
        // Each team gets a random score draw based on their strength.
        // We'll use a very basic model: higher strength increases chance of scoring more.
        int homeGoals = Random.Range(0, 5);  // base random goals 0-4
        int awayGoals = Random.Range(0, 5);
        // Add a slight advantage: the team with higher strength gets +0 or +1 goal randomly
        if (homeStrength > awayStrength && Random.value < 0.5f) {
            homeGoals++;
        } else if (awayStrength > homeStrength && Random.value < 0.5f) {
            awayGoals++;
        }
        // Ensure non-negative (in case strengths are equal and Random causes negative adjustments)
        homeGoals = Mathf.Max(homeGoals, 0);
        awayGoals = Mathf.Max(awayGoals, 0);

        // Record the result in the league
        league.RecordMatchResult(match, homeGoals, awayGoals);

        Debug.Log($"Match Result: {home.TeamName} {homeGoals} - {awayGoals} {away.TeamName}");
    }

    // (Optional) Method to start a new season
    public void StartNewSeason() {
        // Reset team stats
        foreach (Team t in league.Teams) {
            t.Points = t.Wins = t.Draws = t.Losses = 0;
            t.GoalsFor = t.GoalsAgainst = 0;
        }
        // Regenerate fixtures and reset index
        league.GenerateFixtures();
        // Perhaps randomize team rosters or aging players, etc. (not shown here)
        Debug.Log("New season started.");
    }
}
