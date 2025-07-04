using System.Collections.Generic;
using UnityEngine;

public class TransferMarket {
    // List all players available for transfer from all teams (for simplicity, includes all teams)
    public static List<Player> ListAllPlayersForSale(List<League> leagues, List<Team> districtTeams = null) {
        List<Player> available = new List<Player>();
        // Add players from each league team
        foreach (League league in leagues) {
            foreach (Team team in league.Teams) {
                available.AddRange(team.Players);
            }
        }
        // Include district teams' players as well
        if (districtTeams != null) {
            foreach (Team team in districtTeams) {
                available.AddRange(team.Players);
            }
        }
        // (Optionally, could filter out players from the user's own team or other criteria)
        return available;
    }

    // Attempt to transfer a player from one team to another. Returns true if successful.
    public static bool TryTransfer(Player player, Team fromTeam, Team toTeam) {
        if (player == null || fromTeam == null || toTeam == null) return false;
        // Check if the player indeed belongs to the fromTeam
        if (!fromTeam.Players.Contains(player)) {
            Debug.LogWarning("Player does not belong to the specified fromTeam.");
            return false;
        }
        // Check budget
        int price = player.Value;
        if (toTeam.Budget < price) {
            Debug.Log($"{toTeam.TeamName} cannot afford {player.Name} (price {price}).");
            return false;
        }
        // Execute transfer: remove from old team, add to new team
        fromTeam.Players.Remove(player);
        toTeam.Players.Add(player);
        // Adjust budgets
        toTeam.Budget -= price;
        fromTeam.Budget += price;
        // (Optional: reset player's morale due to transfer, or other adjustments)
        Debug.Log($"Transfer Complete: {player.Name} moved from {fromTeam.TeamName} to {toTeam.TeamName} for ${price}.");
        return true;
    }
}