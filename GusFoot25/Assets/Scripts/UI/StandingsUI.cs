using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StandingsUI : MonoBehaviour {
    public Text leagueNameText;
    public Transform standingsContainer;
    public GameObject standingsRowPrefab;

    public void DisplayLeagueTable(League league) {
        if (league == null) return;
        leagueNameText.text = league.Name + " Standings";
        foreach (Transform c in standingsContainer) Destroy(c.gameObject);
        List<Team> table = league.GetStandings();
        for (int i = 0; i < table.Count; i++) {
            var t = table[i];
            var row = Instantiate(standingsRowPrefab, standingsContainer);
            var txt = row.GetComponentInChildren<Text>();
            if (txt != null) {
                txt.text = $"{i+1}. {t.TeamName} - {t.Points} pts ({t.Wins}-{t.Draws}-{t.Losses}, GF-{t.GoalsFor} GA-{t.GoalsAgainst})";
            }
        }
    }
}