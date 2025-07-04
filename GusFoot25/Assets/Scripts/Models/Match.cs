// Data model for a match fixture between two teams
public class Match {
    public Team HomeTeam;
    public Team AwayTeam;
    public int HomeScore;
    public int AwayScore;
    public bool HasBeenPlayed;

    public Match(Team home, Team away) {
        HomeTeam = home;
        AwayTeam = away;
        HomeScore = 0;
        AwayScore = 0;
        HasBeenPlayed = false;
    }
}