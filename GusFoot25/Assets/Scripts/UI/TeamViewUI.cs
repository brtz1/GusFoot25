using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamViewUI : MonoBehaviour {
    public Text teamNameText;
    public Transform playersContainer;   // The parent object (e.g., Content of a ScrollView) for player entries
    public GameObject playerRowPrefab;   // Prefab for a UI element displaying one player's info

    // Display the given team's information on the UI panel
    public void ShowTeam(Team team) {
        if (team == null) return;
        teamNameText.text = team.TeamName;

        // Clear existing player entries in the UI list
        foreach (Transform child in playersContainer) {
            Destroy(child.gameObject);
        }

        // Create a UI row for each player in the team
        foreach (Player p in team.Players) {
            GameObject rowObj = Instantiate(playerRowPrefab, playersContainer);
            Text rowText = rowObj.GetComponentInChildren<Text>();
            if (rowText != null) {
                // Example display: "Name - Pos - OVR Rating"
                rowText.text = $"{p.Name} - {p.Position} - OVR {p.OverallRating}";
            }
        }
    }
}