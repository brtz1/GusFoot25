using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    // Four league divisions
    public League firstDivision;
    public League secondDivision;
    public League thirdDivision;
    public League fourthDivision;
    public List<Team> districtTeams;
    public Cup cup;

    // UI references (assign in Unity Inspector)
    public Text matchResultText;      // UI Text to display latest match results (possibly multiline)
    public TeamViewUI teamViewPanel;  // UI panel to show team details
    public StandingsUI standingsPanel; // UI panel to show league table standings

    private int currentSeason = 1;

    void Start() {
        InitializeGameData();
        // Display initial info
        if (teamViewPanel != null) {
            // Show first team of first division as an example
            teamViewPanel.ShowTeam(firstDivision.Teams[0]);
        }
        if (standingsPanel != null) {
            // Show initial standings of the first division
            standingsPanel.DisplayLeagueTable(firstDivision);
        }
    }

    // Set up leagues, teams, and initial cup for the first season
    void InitializeGameData() {
        // Create four leagues (divisions)
        firstDivision  = new League("1st Division");
        secondDivision = new League("2nd Division");
        thirdDivision  = new League("3rd Division");
        fourthDivision = new League("4th Division");
        districtTeams  = new List<Team>();

        // For each division, create 8 teams with dummy data
        string[] divisionNames = { "Premier Div", "Second Div", "Third Div", "Fourth Div" };
        for (int div = 1; div <= 4; div++) {
            League league = null;
            int startingBudget;
            switch (div) {
                case 1: league = firstDivision;  startingBudget = 1000000; break;
                case 2: league = secondDivision; startingBudget = 800000;  break;
                case 3: league = thirdDivision;  startingBudget = 500000;  break;
                case 4: league = fourthDivision; startingBudget = 300000;  break;
            }
            // Create 8 teams for this league
            for (int t = 1; t <= 8; t++) {
                Team team = new Team($"{divisionNames[div-1]} Team {t}", startingBudget);
                // Add some dummy players to the team
                for (int p = 1; p <= 5; p++) {
                    string[] positions = { "GK", "DEF", "MID", "FWD" };
                    string pos = positions[(p - 1) % positions.Length];
                    int rating = Random.Range(60, 91);  // random overall rating 60-90
                    int value = rating * 10000;        // rough value based on rating
                    team.Players.Add(new Player($"{team.TeamName} Player{p}", Random.Range(18, 35), pos, rating, value));
                }
                // Register team in the league and set reference
                league.Teams.Add(team);
                team.League = league;
            }
            // Generate fixtures (schedule) for this league
            league.GenerateFixtures();
        }

        // Create 32 district teams (outside the main divisions) with dummy data
        for (int d = 1; d <= 32; d++) {
            Team distTeam = new Team($"District Team {d}", startingBudget: 100000); 
            // Add a few players to each district team
            for (int p = 1; p <= 3; p++) {
                string[] positions = { "GK", "DEF", "MID", "FWD" };
                string pos = positions[(p - 1) % positions.Length];
                int rating = Random.Range(50, 81);
                int value = rating * 5000;
                distTeam.Players.Add(new Player($"{distTeam.TeamName} Player{p}", Random.Range(18, 40), pos, rating, value));
            }
            // District teams are not in a League (League remains null)
            districtTeams.Add(distTeam);
        }

        // Set up the Cup competition for the season with all division teams + top district teams (max 64 teams)
        List<Team> cupParticipants = new List<Team>();
        // add all teams from the four divisions:
        cupParticipants.AddRange(firstDivision.Teams);
        cupParticipants.AddRange(secondDivision.Teams);
        cupParticipants.AddRange(thirdDivision.Teams);
        cupParticipants.AddRange(fourthDivision.Teams);
        // ensure max 64 teams: take top district teams if needed
        int maxCupTeams = 64;
        int neededFromDistrict = maxCupTeams - cupParticipants.Count;
        if (neededFromDistrict > 0) {
            // If more district teams available than needed, pick the best by strength (simulating "best district teams")
            if (districtTeams.Count > neededFromDistrict) {
                districtTeams.Sort((a, b) => b.GetTeamStrength().CompareTo(a.GetTeamStrength()));
                // take top N
                cupParticipants.AddRange(districtTeams.GetRange(0, neededFromDistrict));
            } else {
                // take all if not enough to fill
                cupParticipants.AddRange(districtTeams);
            }
        }
        // Initialize cup with these participants
        cup = new Cup("National Cup", cupParticipants);
        cup.StartCup();
        // (Cup matches will be simulated at end of season)
    }

    // Simulate the next "match day" across all divisions (each call plays one match per league if available)
    public void SimulateNextMatchDay() {
        // If all leagues are finished, no more matches to simulate
        if (!firstDivision.HasMoreMatches() && !secondDivision.HasMoreMatches() &&
            !thirdDivision.HasMoreMatches() && !fourthDivision.HasMoreMatches()) {
            Debug.Log("Season is over. No more league matches to simulate.");
            return;
        }
        string resultsText = "";
        // Simulate one match in each league (if that league still has pending matches)
        League[] allLeagues = { firstDivision, secondDivision, thirdDivision, fourthDivision };
        foreach (League league in allLeagues) {
            if (league.HasMoreMatches()) {
                Match match = league.GetNextMatch();
                // Simulate the match result
                SimulateMatch(match);
                Team home = match.HomeTeam;
                Team away = match.AwayTeam;
                resultsText += $"{home.TeamName} {match.HomeScore} - {match.AwayScore} {away.TeamName}\n";
            }
        }
        // Update the UI text with all results from this "day"
        if (matchResultText != null) {
            matchResultText.text = resultsText.TrimEnd();  // remove last newline
        }
        // Update the standings UI (for example, show first division standings by default)
        if (standingsPanel != null) {
            standingsPanel.DisplayLeagueTable(firstDivision);
        }
        // Check if season ended after this matchday
        if (!firstDivision.HasMoreMatches() && !secondDivision.HasMoreMatches() &&
            !thirdDivision.HasMoreMatches() && !fourthDivision.HasMoreMatches()) {
            Debug.Log("All leagues finished. Season " + currentSeason + " complete!");
            EndSeasonAndPrepareNext();
        }
    }

    // Simulate a single match between two teams and update league standings.
    private void SimulateMatch(Match match) {
        Team home = match.HomeTeam;
        Team away = match.AwayTeam;
        // Effective strength includes team strength and a morale bonus
        int baseHomeStrength = home.GetTeamStrength();
        int baseAwayStrength = away.GetTeamStrength();
        int homeEff = baseHomeStrength + home.Morale / 10;
        int awayEff = baseAwayStrength + away.Morale / 10;
        // Simple outcome algorithm: random base goals, plus extra weighted by relative strength
        float totalEff = (homeEff + awayEff > 0) ? (homeEff + awayEff) : 1f;
        float homeChance = homeEff / totalEff;
        // Base random goals 0-2 for each
        int homeGoals = Random.Range(0, 3);
        int awayGoals = Random.Range(0, 3);
        // Extra goals chances: do two trials for each team
        if (Random.value < homeChance) homeGoals++;
        if (Random.value < homeChance) homeGoals++;
        if (Random.value < (1 - homeChance)) awayGoals++;
        if (Random.value < (1 - homeChance)) awayGoals++;
        // Ensure no negative values (not expected here, but just in case)
        homeGoals = Mathf.Max(homeGoals, 0);
        awayGoals = Mathf.Max(awayGoals, 0);
        // Update morale based on match outcome (optional tweak: winners get a morale boost, losers drop)
        if (homeGoals > awayGoals) {
            home.Morale = Mathf.Min(home.Morale + 5, 100);
            away.Morale = Mathf.Max(away.Morale - 5, 0);
        } else if (awayGoals > homeGoals) {
            away.Morale = Mathf.Min(away.Morale + 5, 100);
            home.Morale = Mathf.Max(home.Morale - 5, 0);
        } else {
            // draw: small boost to both
            home.Morale = Mathf.Min(home.Morale + 2, 100);
            away.Morale = Mathf.Min(away.Morale + 2, 100);
        }
        // Record result in the league standings
        home.League.RecordMatchResult(match, homeGoals, awayGoals);
        Debug.Log($"Match Result: {home.TeamName} {homeGoals} - {awayGoals} {away.TeamName}");
    }

    // Handle end of season: determine promotions/relegations, run the Cup, and set up next season
    private void EndSeasonAndPrepareNext() {
        Debug.Log("Processing end of season " + currentSeason + "...");
        // 1. Run the Cup knockout matches to determine the cup winner (using teams from this season)
        if (cup != null) {
            Team cupWinner = RunCupCompetition();
            Debug.Log("Cup Winner of season " + currentSeason + ": " + (cupWinner != null ? cupWinner.TeamName : "N/A"));
        }

        // 2. Determine promotions and relegations between divisions and district
        List<Team> firstRank = firstDivision.GetStandings();
        List<Team> secondRank = secondDivision.GetStandings();
        List<Team> thirdRank = thirdDivision.GetStandings();
        List<Team> fourthRank = fourthDivision.GetStandings();
        // Top 2 from lower division get promoted, bottom 2 from higher get relegated.
        // First Division champion (firstRank[0]) is overall champion (could announce separately).
        Team firstChampion = firstRank[0];

        // Relegate bottom 2 of 1st to 2nd, promote top 2 of 2nd to 1st
        Team fd_relegated1 = firstRank[firstRank.Count - 1];
        Team fd_relegated2 = firstRank[firstRank.Count - 2];
        Team sd_promoted1 = secondRank[0];
        Team sd_promoted2 = secondRank[1];
        firstDivision.Teams.Remove(fd_relegated1);
        firstDivision.Teams.Remove(fd_relegated2);
        secondDivision.Teams.Remove(sd_promoted1);
        secondDivision.Teams.Remove(sd_promoted2);
        firstDivision.Teams.Add(sd_promoted1); sd_promoted1.League = firstDivision;
        firstDivision.Teams.Add(sd_promoted2); sd_promoted2.League = firstDivision;
        secondDivision.Teams.Add(fd_relegated1); fd_relegated1.League = secondDivision;
        secondDivision.Teams.Add(fd_relegated2); fd_relegated2.League = secondDivision;

        // Relegate bottom 2 of 2nd to 3rd, promote top 2 of 3rd to 2nd
        secondRank = secondDivision.GetStandings(); // update ranking after above moves
        thirdRank = thirdDivision.GetStandings();
        Team sd_relegated1 = secondRank[secondRank.Count - 1];
        Team sd_relegated2 = secondRank[secondRank.Count - 2];
        Team td_promoted1 = thirdRank[0];
        Team td_promoted2 = thirdRank[1];
        secondDivision.Teams.Remove(sd_relegated1);
        secondDivision.Teams.Remove(sd_relegated2);
        thirdDivision.Teams.Remove(td_promoted1);
        thirdDivision.Teams.Remove(td_promoted2);
        secondDivision.Teams.Add(td_promoted1); td_promoted1.League = secondDivision;
        secondDivision.Teams.Add(td_promoted2); td_promoted2.League = secondDivision;
        thirdDivision.Teams.Add(sd_relegated1); sd_relegated1.League = thirdDivision;
        thirdDivision.Teams.Add(sd_relegated2); sd_relegated2.League = thirdDivision;

        // Relegate bottom 2 of 3rd to 4th, promote top 2 of 4th to 3rd
        thirdRank = thirdDivision.GetStandings();
        fourthRank = fourthDivision.GetStandings();
        Team td_relegated1 = thirdRank[thirdRank.Count - 1];
        Team td_relegated2 = thirdRank[thirdRank.Count - 2];
        Team fd4_promoted1 = fourthRank[0];
        Team fd4_promoted2 = fourthRank[1];
        thirdDivision.Teams.Remove(td_relegated1);
        thirdDivision.Teams.Remove(td_relegated2);
        fourthDivision.Teams.Remove(fd4_promoted1);
        fourthDivision.Teams.Remove(fd4_promoted2);
        thirdDivision.Teams.Add(fd4_promoted1); fd4_promoted1.League = thirdDivision;
        thirdDivision.Teams.Add(fd4_promoted2); fd4_promoted2.League = thirdDivision;
        fourthDivision.Teams.Add(td_relegated1); td_relegated1.League = fourthDivision;
        fourthDivision.Teams.Add(td_relegated2); td_relegated2.League = fourthDivision;

        // Relegate bottom 2 of 4th to district, promote top 2 district teams to 4th
        fourthRank = fourthDivision.GetStandings();
        // Determine top 2 district teams (simulate a "district league" outcome - here we use team strength + random)
        if (districtTeams.Count > 0) {
            // Assign each district team a random performance score and sort by it
            List<Team> districtRanking = new List<Team>(districtTeams);
            foreach (Team dt in districtRanking) {
                // Use team strength + some randomness as a proxy for season performance
                dt.Morale += Random.Range(0, 11); // random boost to morale as proxy for form
            }
            districtRanking.Sort((a, b) => b.GetTeamStrength().CompareTo(a.GetTeamStrength()));
            // Top 2 become promoted
            Team promotedDist1 = districtRanking[0];
            Team promotedDist2 = (districtRanking.Count > 1 ? districtRanking[1] : null);
            // Bottom 2 of 4th division go to district
            Team fd4_relegated1 = fourthRank[fourthRank.Count - 1];
            Team fd4_relegated2 = fourthRank[fourthRank.Count - 2];
            // Remove/transfer teams
            fourthDivision.Teams.Remove(fd4_relegated1);
            fourthDivision.Teams.Remove(fd4_relegated2);
            districtTeams.Remove(promotedDist1);
            if (promotedDist2 != null) districtTeams.Remove(promotedDist2);
            // Add promotions to 4th
            fourthDivision.Teams.Add(promotedDist1); promotedDist1.League = fourthDivision;
            if (promotedDist2 != null) {
                fourthDivision.Teams.Add(promotedDist2); promotedDist2.League = fourthDivision;
            }
            // Add relegations to district
            fd4_relegated1.League = null;
            fd4_relegated2.League = null;
            districtTeams.Add(fd4_relegated1);
            districtTeams.Add(fd4_relegated2);
        }

        // 3. Reset all team stats for the new season and regenerate fixtures
        League[] allLeagues = { firstDivision, secondDivision, thirdDivision, fourthDivision };
        foreach (League league in allLeagues) {
            foreach (Team t in league.Teams) {
                t.Points = t.Wins = t.Draws = t.Losses = 0;
                t.GoalsFor = t.GoalsAgainst = 0;
                // (Optionally, reset morale towards neutral or carry over as desired)
            }
            league.GenerateFixtures();
        }

        // 4. Prepare new Cup for next season (with updated teams composition)
        currentSeason++;
        List<Team> newCupTeams = new List<Team>();
        newCupTeams.AddRange(firstDivision.Teams);
        newCupTeams.AddRange(secondDivision.Teams);
        newCupTeams.AddRange(thirdDivision.Teams);
        newCupTeams.AddRange(fourthDivision.Teams);
        int neededDist = 64 - newCupTeams.Count;
        if (neededDist > 0) {
            if (districtTeams.Count > neededDist) {
                districtTeams.Sort((a, b) => b.GetTeamStrength().CompareTo(a.GetTeamStrength()));
                newCupTeams.AddRange(districtTeams.GetRange(0, neededDist));
            } else {
                newCupTeams.AddRange(districtTeams);
            }
        }
        cup = new Cup("National Cup", newCupTeams);
        cup.StartCup();

        Debug.Log("Season " + (currentSeason - 1) + " ended. Champions: " +
                  firstChampion.TeamName + " (1st Div). New season " + currentSeason + " begins.");
        // You could update UI or notify player of promotions/relegations here.
        if (standingsPanel != null) {
            // Refresh standings display for the new season (e.g., show 1st Division at start)
            standingsPanel.DisplayLeagueTable(firstDivision);
        }
    }

    // Simulate the entire knockout cup competition and return the champion team
    private Team RunCupCompetition() {
        if (cup == null) return null;
        Team cupChampion = null;
        // Iterate through rounds until we have a winner
        while (true) {
            List<Team> winners = new List<Team>();
            // Simulate all matches in the current round
            foreach (Match match in cup.CurrentRoundMatches) {
                Team winner = SimulateCupMatch(match);
                winners.Add(winner);
            }
            if (winners.Count <= 1) {
                // Tournament ends (we have a champion)
                if (winners.Count == 1) {
                    cupChampion = winners[0];
                    cup.Champion = cupChampion;
                }
                break;
            }
            // Advance to next round with the winners
            cup.AdvanceRound(winners);
        }
        return cupChampion;
    }

    // Simulate a single cup match (must produce a winner since it's knockout)
    private Team SimulateCupMatch(Match match) {
        Team home = match.HomeTeam;
        Team away = match.AwayTeam;
        int homeEff = home.GetTeamStrength() + home.Morale / 10;
        int awayEff = away.GetTeamStrength() + away.Morale / 10;
        float totalEff = (homeEff + awayEff > 0) ? homeEff + awayEff : 1f;
        float homeChance = homeEff / totalEff;
        // Generate base goals 0-2
        int homeGoals = Random.Range(0, 3);
        int awayGoals = Random.Range(0, 3);
        // Add extra goals with similar weighted chances (ensure at least one scores)
        if (Random.value < homeChance) homeGoals++;
        if (Random.value < (1 - homeChance)) awayGoals++;
        // If draw, decide winner by "penalty" (sudden death)
        if (homeGoals == awayGoals) {
            if (Random.value < 0.5f) homeGoals++; else awayGoals++;
        }
        // (We don't update league points for cup matches, only track morale)
        if (homeGoals > awayGoals) {
            home.Morale = Mathf.Min(home.Morale + 5, 100);
            away.Morale = Mathf.Max(away.Morale - 5, 0);
            Debug.Log($"Cup match: {home.TeamName} wins {homeGoals}-{awayGoals} vs {away.TeamName}");
            return home;
        } else {
            away.Morale = Mathf.Min(away.Morale + 5, 100);
            home.Morale = Mathf.Max(home.Morale - 5, 0);
            Debug.Log($"Cup match: {away.TeamName} wins {awayGoals}-{homeGoals} vs {home.TeamName}");
            return away;
        }
    }
}