using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    // Leagues
    public League firstDivision, secondDivision, thirdDivision, fourthDivision;
    public List<Team> districtTeams;
    public Cup cup;

    // UI references (assign in Inspector)
    [Header("Game UI")]
    public Text matchResultText;
    public TeamViewUI teamViewPanel;

    [Header("Panels")]
    public GameObject standingsPanelObject;
    public StandingsUI standingsUI;
    public GameObject transferPanelObject;
    public TransferMarketUI transferUI;
    public GameObject cupPanelObject;
    public CupViewUI cupUI;

    private int currentSeason = 1;

    void Start() {
        InitializeGameData();
        if (teamViewPanel != null)
            teamViewPanel.ShowTeam(firstDivision.Teams[0]);
    }

    void InitializeGameData() {
        // Create and populate four divisions...
        // (See previous code for dummy team/player setup, fixture generation, cup initialization)
        // For brevity, that code is assumed here exactly as before.
    }

    // Called by "Simulate Matchday" button
    public void SimulateNextMatchDay() {
        if (!firstDivision.HasMoreMatches()) {
            Debug.Log("Season over");
            return;
        }
        var results = "";
        var leagues = new League[] { firstDivision, secondDivision, thirdDivision, fourthDivision };
        foreach (var lg in leagues) {
            if (lg.HasMoreMatches()) {
                var m = lg.GetNextMatch();
                SimulateMatch(m);
                results += $"{m.HomeTeam.TeamName} {m.HomeScore} - {m.AwayScore} {m.AwayTeam.TeamName}\n";
            }
        }
        matchResultText.text = results.TrimEnd();
    }

    void SimulateMatch(Match m) {
        var h = m.HomeTeam; var a = m.AwayTeam;
        int he = h.GetTeamStrength() + h.Morale/10;
        int ae = a.GetTeamStrength() + a.Morale/10;
        float tot = he + ae;
        float hc = tot>0? he/tot : 0.5f;
        int hg = Random.Range(0,3); int ag = Random.Range(0,3);
        if (Random.value < hc) hg++;
        if (Random.value < (1-hc)) ag++;
        // morale updates omitted for brevity
        h.League.RecordMatchResult(m, hg, ag);
        Debug.Log($"{h.TeamName} {hg}-{ag} {a.TeamName}");
    }

    // Show/Hide standings panel
    public void OnViewStandings() {
        standingsPanelObject.SetActive(true);
        standingsUI.DisplayLeagueTable(firstDivision);
    }
    public void OnHideStandings() {
        standingsPanelObject.SetActive(false);
    }

    // Show/Hide transfer market
    public void OnViewTransferMarket() {
        transferPanelObject.SetActive(true);
        transferUI.Initialize(
            new List<League>{firstDivision,secondDivision,thirdDivision,fourthDivision},
            districtTeams
        );
    }
    public void OnHideTransferMarket() {
        transferPanelObject.SetActive(false);
    }

    // Show/Hide cup view
    public void OnViewCup() {
        cupPanelObject.SetActive(true);
        cupUI.Initialize(cup);
    }
    public void OnHideCup() {
        cupPanelObject.SetActive(false);
    }
}