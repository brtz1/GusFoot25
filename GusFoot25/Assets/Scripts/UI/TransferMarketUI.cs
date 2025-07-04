using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TransferMarketUI : MonoBehaviour {
    public Transform playerContainer;
    public GameObject playerRowPrefab;

    public void Initialize(List<League> leagues, List<Team> districtTeams) {
        var players = TransferMarket.ListAllPlayersForSale(leagues, districtTeams);
        foreach (Transform c in playerContainer) Destroy(c.gameObject);
        foreach (var p in players) {
            var row = Instantiate(playerRowPrefab, playerContainer);
            var txt = row.GetComponentInChildren<Text>();
            if (txt != null) {
                txt.text = $"{p.Name} ({p.Position}) - ${p.Value}";
            }
        }
    }
}