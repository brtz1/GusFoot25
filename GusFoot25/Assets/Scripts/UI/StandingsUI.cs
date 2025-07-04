using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StandingsUI : MonoBehaviour {
    public Text leagueNameText;
    public Transform standingsContainer;   // Parent object (e.g., ScrollView content) for rows
    public GameObject standingsRowPrefab;  // Prefab for one row displaying team standings

    // Display the given league's standings in the UI
    public void DisplayLeagueTable(League league) {
        if (league == null) return;
        // Set league name header
        if (leagueNameText != null) {
            leagueNameText.text = league.Name + " Standings";
        }
        // Clear existing rows
        foreach (Transform child in standingsContainer) {
            Destroy(child.gameObject);
        }
        // Get sorted standings and create a row for each team
        List<Team> ranking = league.GetStandings();
        for (int i = 0; i < ranking.Count; i++) {
            Team team = ranking[i];
            GameObject rowObj = Instantiate(standingsRowPrefab, standingsContainer);
            Text rowText = rowObj.GetComponentInChildren<Text>();
            if (rowText != null) {
                // Example row format: "1. TeamName - Pts pts (W-D-L, GF-GA)"
                rowText.text = string.Format("{0}. {1} - {2} pts  ({3}-{4}-{5}, GF-{6} GA-{7})",
                    i+1, team.TeamName, team.Points, team.Wins, team.Draws, team.Losses,
                    team.GoalsFor, team.GoalsAgainst);
            }
        }
    }
}