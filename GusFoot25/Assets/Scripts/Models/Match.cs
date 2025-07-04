public class Match {
    public Team HomeTeam;
    public Team AwayTeam;
    public int HomeScore;
    public int AwayScore;
    public bool HasBeenPlayed;

    public Match(Team home, Team away) {
        HomeTeam = home;
        AwayTeam = away;
        HomeScore = AwayScore = 0;
        HasBeenPlayed = false;
    }
}