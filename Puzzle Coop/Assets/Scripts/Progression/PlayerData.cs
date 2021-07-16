using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // This Class is responsible for conversion of PlayerProgression for saving/loading
    
    public string playerName;
    public int playerSteamId; // adjust this based on steamID datatype

    public int[] unlockedMaps;
    public int[] unlockedAchievements;

    public PlayerData(PlayerProgress player)
    {
        playerName = player.playerName;

        playerSteamId = player.playerSteamId;

        // Convert Achievements to Index Int Array
        unlockedAchievements = new int[player.unlockedAchievements.Count];
        for (int i = 0; i < player.unlockedAchievements.Count; i++)
        {
            unlockedAchievements[i] = player.unlockedAchievements[i];
        }

        // Convert Maps to Index Int Array
        unlockedMaps = new int[player.unlockedMaps.Count];
        for (int i = 0; i < player.unlockedMaps.Count; i++)
        {
            unlockedMaps[i] = player.unlockedMaps[i];
        }



    }
}
