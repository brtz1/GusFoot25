// Data model for a league competition
public class League {
    public string Name;
    public List<Team> Teams;
    public List<Match> Fixtures;    // all scheduled matches in the season
    private int currentMatchIndex;  // pointer to next match to be played in the schedule

    public League(string name) {
        Name = name;
        Teams = new List<Team>();
        Fixtures = new List<Match>();
        currentMatchIndex = 0;
    }

    // Set up a single round-robin schedule (each team plays every other team once)
    public void GenerateFixtures() {
        Fixtures.Clear();
        // Round-robin pairing
        for (int i = 0; i < Teams.Count; i++) {
            for (int j = i+1; j < Teams.Count; j++) {
                Match match = new Match(Teams[i], Teams[j]);
                Fixtures.Add(match);
            }
        }
        currentMatchIndex = 0;
    }

    // Check if there are more matches to play
    public bool HasMoreMatches() {
        return currentMatchIndex < Fixtures.Count;
    }

    // Get the next match to be played
    public Match GetNextMatch() {
        if (!HasMoreMatches()) return null;
        return Fixtures[currentMatchIndex];
    }

    // Record the result of a played match and update standings
    public void RecordMatchResult(Match match, int homeGoals, int awayGoals) {
        match.HomeScore = homeGoals;
        match.AwayScore = awayGoals;
        match.HasBeenPlayed = true;
        currentMatchIndex++;

        // Update team stats for standings
        Team home = match.HomeTeam;
        Team away = match.AwayTeam;
        home.GoalsFor += homeGoals;
        home.GoalsAgainst += awayGoals;
        away.GoalsFor += awayGoals;
        away.GoalsAgainst += homeGoals;
        if (homeGoals > awayGoals) {
            home.Wins++; home.Points += 3;
            away.Losses++;
        } else if (homeGoals < awayGoals) {
            away.Wins++; away.Points += 3;
            home.Losses++;
        } else {
            // draw
            home.Draws++; home.Points += 1;
            away.Draws++; away.Points += 1;
        }
    }

    // (Optional) Get a sorted standings list (e.g., could return teams sorted by Points, GD, etc.)
    public List<Team> GetStandings() {
        List<Team> sorted = new List<Team>(Teams);
        sorted.Sort((Team a, Team b) => {
            // sort by points, then goal difference, then goals for
            int cmp = b.Points.CompareTo(a.Points);
            if (cmp == 0) {
                int gdA = a.GoalsFor - a.GoalsAgainst;
                int gdB = b.GoalsFor - b.GoalsAgainst;
                cmp = gdB.CompareTo(gdA);
                if (cmp == 0) {
                    cmp = b.GoalsFor.CompareTo(a.GoalsFor);
                }
            }
            return cmp;
        });
        return sorted;
    }
}