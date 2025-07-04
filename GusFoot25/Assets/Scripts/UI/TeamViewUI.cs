using UnityEngine;
using UnityEngine.UI;

public class TeamViewUI : MonoBehaviour {
    public Text teamNameText;
    public Transform playersContainer;
    public GameObject playerRowPrefab;

    public void ShowTeam(Team team) {
        if (team == null) return;
        teamNameText.text = team.TeamName;
        foreach (Transform c in playersContainer) Destroy(c.gameObject);
        foreach (var p in team.Players) {
            var row = Instantiate(playerRowPrefab, playersContainer);
            var txt = row.GetComponentInChildren<Text>();
            if (txt != null) {
                txt.text = $"{p.Name} - {p.Position} - OVR {p.OverallRating}";
            }
        }
    }
}