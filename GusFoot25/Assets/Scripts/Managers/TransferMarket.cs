using System.Collections.Generic;
using UnityEngine;

public class TransferMarket
{
    public static List<Player> ListAllPlayersForSale(
        List<League> leagues,
        List<Team> districtTeams = null
    )
    {
        var all = new List<Player>();
        foreach (var league in leagues)
        {
            foreach (var team in league.Teams)
            {
                all.AddRange(team.Players);
            }
        }
        if (districtTeams != null)
        {
            foreach (var dt in districtTeams)
            {
                all.AddRange(dt.Players);
            }
        }
        return all;
    }

    public static bool TryTransfer(Player p, Team from, Team to)
    {
        if (p == null || from == null || to == null) return false;
        if (!from.Players.Contains(p)) return false;
        int price = p.Value;
        if (to.Budget < price)
        {
            Debug.Log($"{to.TeamName} cannot afford {p.Name}.");
            return false;
        }
        from.Players.Remove(p);
        to.Players.Add(p);
        to.Budget -= price;
        from.Budget += price;
        Debug.Log($"Transferred {p.Name} from {from.TeamName} to {to.TeamName} for {price}.");
        return true;
    }
}