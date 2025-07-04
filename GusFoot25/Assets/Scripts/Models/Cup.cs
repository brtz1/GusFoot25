// Data model for a knockout cup competition
public class Cup {
    public string Name;
    public List<Team> Participants;
    public List<Match> CurrentRoundMatches;
    public int CurrentRoundNumber;  // e.g., 1 = semifinal, 2 = final, etc.

    public Cup(string name, List<Team> participants) {
        Name = name;
        Participants = new List<Team>(participants);
        CurrentRoundMatches = new List<Match>();
        CurrentRoundNumber = 0;
    }

    // Initialize the first round of the cup (e.g., semi-finals if 4 teams)
    public void StartCup() {
        CurrentRoundNumber = 1;
        CurrentRoundMatches.Clear();
        // Example: if 4 teams, create 2 semi-final matches by pairing teams
        if (Participants.Count >= 4) {
            // Pair team 0 vs 1, and team 2 vs 3 for semi-finals (simple pairing)
            CurrentRoundMatches.Add(new Match(Participants[0], Participants[1]));
            CurrentRoundMatches.Add(new Match(Participants[2], Participants[3]));
        }
        // (If more teams, a random shuffle and pairing would be done.)
    }

    // Advance to next round (e.g., from semi-final to final)
    public void AdvanceRound(List<Team> winners) {
        CurrentRoundNumber++;
        CurrentRoundMatches.Clear();
        if (winners.Count >= 2) {
            // Create next round matches (e.g., final) from winners
            CurrentRoundMatches.Add(new Match(winners[0], winners[1]));
        }
    }
}
