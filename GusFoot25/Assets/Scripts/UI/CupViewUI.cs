using UnityEngine;
using UnityEngine.UI;

public class CupViewUI : MonoBehaviour {
    public Text roundNameText;
    public Transform matchesContainer;
    public GameObject matchRowPrefab;

    private Cup cup;

    public void Initialize(Cup cupData) {
        cup = cupData;
        DisplayCurrentRound();
    }

    public void DisplayCurrentRound() {
        if (cup == null) return;
        roundNameText.text = $"Cup Round {cup.CurrentRoundNumber}";
        foreach (Transform c in matchesContainer) Destroy(c.gameObject);
        foreach (var m in cup.CurrentRoundMatches) {
            var row = Instantiate(matchRowPrefab, matchesContainer);
            var txt = row.GetComponentInChildren<Text>();
            if (txt != null) {
                txt.text = $"{m.HomeTeam.TeamName} vs {m.AwayTeam.TeamName}";
            }
        }
    }
}