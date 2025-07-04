// Data model for a knockout cup competition
public class Cup {
    public string Name;
    public List<Team> Participants;
    public List<Match> CurrentRoundMatches;
    public int CurrentRoundNumber;
    public Team Champion;

    public Cup(string name, List<Team> participants) {
        Name = name;
        Participants = new List<Team>(participants);
        CurrentRoundMatches = new List<Match>();
        CurrentRoundNumber = 0;
        Champion = null;
    }

    // Initialize the first round of the cup (random draw pairings)
    public void StartCup() {
        CurrentRoundNumber = 1;
        CurrentRoundMatches.Clear();
        int count = Participants.Count;
        if (count < 2) return;  // need at least 2 teams
        // Shuffle participants for random draw
        List<Team> drawList = new List<Team>(Participants);
        System.Random rng = new System.Random();
        for (int i = drawList.Count - 1; i > 0; i--) {
            int j = rng.Next(i + 1);
            // swap drawList[i] and drawList[j]
            Team temp = drawList[i];
            drawList[i] = drawList[j];
            drawList[j] = temp;
        }
        // Pair teams for the first round
        for (int i = 0; i < drawList.Count - 1; i += 2) {
            Team teamA = drawList[i];
            Team teamB = drawList[i+1];
            CurrentRoundMatches.Add(new Match(teamA, teamB));
        }
        // Note: If an odd number of teams, the last team in drawList would not be paired and gets a bye.
        // (For simplicity, we assume an even number of participants.)
    }

    // Advance to the next round with the given winners from the previous round
    public void AdvanceRound(List<Team> winners) {
        // If only one winner, tournament is over
        if (winners.Count <= 1) {
            if (winners.Count == 1) {
                Champion = winners[0];
            }
            CurrentRoundMatches.Clear();
            return;
        }
        CurrentRoundNumber++;
        CurrentRoundMatches.Clear();
        // Shuffle winners for a fresh random draw for the next round
        System.Random rng = new System.Random();
        for (int i = winners.Count - 1; i > 0; i--) {
            int j = rng.Next(i + 1);
            Team temp = winners[i];
            winners[i] = winners[j];
            winners[j] = temp;
        }
        // Pair up winners for the new round
        for (int i = 0; i < winners.Count - 1; i += 2) {
            Team teamA = winners[i];
            Team teamB = winners[i+1];
            CurrentRoundMatches.Add(new Match(teamA, teamB));
        }
        // If winners.Count is odd (rare if initial count was power of 2), one team gets a bye to next round.
        // (Not explicitly handled here for simplicity.)
    }
}
