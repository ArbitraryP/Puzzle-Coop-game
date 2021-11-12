using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // This Class is responsible for conversion of PlayerProgression for saving/loading
    
    public string playerName;
    public ulong playerSteamId; // adjust this based on steamID datatype

    public int[] unlockedMaps;
    public int[] completedMaps;
    public int[] unlockedAchievements;

    public int gameRating;

    public float volumeSFX;
    public float volumeBGM;

    // Add code that handles hasRated to be saved too

    public PlayerData(PlayerProgress player)
    {
        playerName = player.playerName;

        playerSteamId = player.playerSteamId;

        // Convert Achievements to Index Int Array
        unlockedAchievements = new int[player.UnlockedAchievements.Count];
        for (int i = 0; i < player.UnlockedAchievements.Count; i++)
        {
            unlockedAchievements[i] = player.UnlockedAchievements[i];
        }

        // Convert Unlocked Maps to Index Int Array
        unlockedMaps = new int[player.UnlockedMaps.Count];
        for (int i = 0; i < player.UnlockedMaps.Count; i++)
        {
            unlockedMaps[i] = player.UnlockedMaps[i];
        }

        // Convert Completed Maps to Index Int Array
        completedMaps = new int[player.CompletedMaps.Count];
        for (int i = 0; i < player.CompletedMaps.Count; i++)
        {
            completedMaps[i] = player.CompletedMaps[i];
        }

        gameRating = player.gameRating;



    }
}
