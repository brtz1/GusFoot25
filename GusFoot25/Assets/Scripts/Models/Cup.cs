using System.Collections.Generic;

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

    public void StartCup() {
        CurrentRoundNumber = 1;
        CurrentRoundMatches.Clear();
        var draw = new List<Team>(Participants);
        var rng = new System.Random();
        for (int i = draw.Count - 1; i > 0; i--) {
            int j = rng.Next(i + 1);
            var tmp = draw[i]; draw[i] = draw[j]; draw[j] = tmp;
        }
        for (int i = 0; i < draw.Count - 1; i += 2) {
            CurrentRoundMatches.Add(new Match(draw[i], draw[i + 1]));
        }
    }

    public void AdvanceRound(List<Team> winners) {
        if (winners.Count <= 1) {
            if (winners.Count == 1) Champion = winners[0];
            CurrentRoundMatches.Clear();
            return;
        }
        CurrentRoundNumber++;
        CurrentRoundMatches.Clear();
        var rng = new System.Random();
        for (int i = winners.Count - 1; i > 0; i--) {
            int j = rng.Next(i + 1);
            var temp = winners[i]; winners[i] = winners[j]; winners[j] = temp;
        }
        for (int i = 0; i < winners.Count - 1; i += 2) {
            CurrentRoundMatches.Add(new Match(winners[i], winners[i + 1]));
        }
    }
}
