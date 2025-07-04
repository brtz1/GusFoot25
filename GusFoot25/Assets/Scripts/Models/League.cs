using System.Collections.Generic;

public class League {
    public string Name;
    public List<Team> Teams;
    public List<Match> Fixtures;
    private int currentMatchIndex;

    public League(string name) {
        Name = name;
        Teams = new List<Team>();
        Fixtures = new List<Match>();
        currentMatchIndex = 0;
    }

    public void GenerateFixtures() {
        Fixtures.Clear();
        int n = Teams.Count;
        // Round-robin circle method
        var schedule = new List<Team>(Teams);
        for (int round = 0; round < n - 1; round++) {
            for (int i = 0; i < n / 2; i++) {
                var home = schedule[i];
                var away = schedule[n - 1 - i];
                Fixtures.Add(round % 2 == 0 
                    ? new Match(home, away) 
                    : new Match(away, home));
            }
            // rotate
            var last = schedule[n - 1];
            schedule.RemoveAt(n - 1);
            schedule.Insert(1, last);
        }
        currentMatchIndex = 0;
    }

    public bool HasMoreMatches() => currentMatchIndex < Fixtures.Count;

    public Match GetNextMatch() {
        if (!HasMoreMatches()) return null;
        return Fixtures[currentMatchIndex];
    }

    public void RecordMatchResult(Match m, int homeGoals, int awayGoals) {
        m.HomeScore = homeGoals;
        m.AwayScore = awayGoals;
        m.HasBeenPlayed = true;
        currentMatchIndex++;

        var home = m.HomeTeam;
        var away = m.AwayTeam;

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
            home.Draws++; home.Points += 1;
            away.Draws++; away.Points += 1;
        }
    }

    public List<Team> GetStandings() {
        var sorted = new List<Team>(Teams);
        sorted.Sort((a, b) => {
            int cmp = b.Points.CompareTo(a.Points);
            if (cmp == 0) {
                int gdA = a.GoalsFor - a.GoalsAgainst;
                int gdB = b.GoalsFor - b.GoalsAgainst;
                cmp = gdB.CompareTo(gdA);
                if (cmp == 0) cmp = b.GoalsFor.CompareTo(a.GoalsFor);
            }
            return cmp;
        });
        return sorted;
    }
}