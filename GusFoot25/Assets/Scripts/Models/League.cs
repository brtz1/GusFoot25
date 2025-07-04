// Data model for a league competition (round-robin format)
public class League {
    public string Name;
    public List<Team> Teams;
    public List<Match> Fixtures;      // all scheduled matches in the season
    private int currentMatchIndex;    // pointer to next match to be played

    public League(string name) {
        Name = name;
        Teams = new List<Team>();
        Fixtures = new List<Match>();
        currentMatchIndex = 0;
    }

    // Generate a single round-robin schedule (each team plays every other team once),
    // grouped into matchdays so that each team plays only one match per round.
    public void GenerateFixtures() {
        Fixtures.Clear();
        int n = Teams.Count;
        // If odd number of teams, add a dummy "bye" (no opponent) to make even count
        bool hasBye = (n % 2 != 0);
        Team byeTeam = null;
        if (hasBye) {
            byeTeam = new Team("BYE", 0);
            Teams.Add(byeTeam);
            n += 1;
        }
        // Round-robin schedule using the circle method
        List<Team> scheduleTeams = new List<Team>(Teams);
        // Keep the first team static, rotate the rest
        for (int round = 0; round < n - 1; round++) {
            for (int i = 0; i < n / 2; i++) {
                Team home = scheduleTeams[i];
                Team away = scheduleTeams[n - 1 - i];
                if (home == byeTeam || away == byeTeam) {
                    // Skip matches involving bye (free win, can be ignored or handled separately)
                    continue;
                }
                // Alternate home/away to balance home advantage
                if (round % 2 == 0) {
                    Fixtures.Add(new Match(home, away));
                } else {
                    Fixtures.Add(new Match(away, home));
                }
            }
            // Rotate teams (except first team stays in place)
            Team last = scheduleTeams[scheduleTeams.Count - 1];
            scheduleTeams.RemoveAt(scheduleTeams.Count - 1);
            scheduleTeams.Insert(1, last);
        }
        currentMatchIndex = 0;
        // If a bye team was added, remove it from the team list (it's not a real team in competition)
        if (hasBye) {
            Teams.Remove(byeTeam);
        }
    }

    // Check if there are more matches to play in this league
    public bool HasMoreMatches() {
        return currentMatchIndex < Fixtures.Count;
    }

    // Get the next scheduled match to be played
    public Match GetNextMatch() {
        if (!HasMoreMatches()) return null;
        return Fixtures[currentMatchIndex];
    }

    // Record the result of a played match and update the standings
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

    // Get a sorted standings list (teams sorted by Points, then Goal Difference, then Goals For)
    public List<Team> GetStandings() {
        List<Team> sorted = new List<Team>(Teams);
        sorted.Sort((Team a, Team b) => {
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