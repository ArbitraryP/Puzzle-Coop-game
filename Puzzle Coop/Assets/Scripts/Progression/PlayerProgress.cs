using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    // This class is where the progress of the player will get loaded locally

    public string playerName;
    public ulong playerSteamId; // adjust this based on steamID datatype

    [SerializeField] private List<int> completedMaps;
    [SerializeField] private List<int> unlockedMaps;
    [SerializeField] private List<int> unlockedAchievements;

    public List<int> CompletedMaps => completedMaps;
    public List<int> UnlockedMaps => unlockedMaps;
    public List<int> UnlockedAchievements => unlockedAchievements;

    public int gameRating = 0;

    public static event Action<int> OnAchievementUnlocked;

    private static PlayerProgress instance;

    // Master Volume must be adjusted from SettingsAndExit/AudioManager
    public int volumeSFXdata;
    public int volumeBGMdata;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else { instance = this; }

        DontDestroyOnLoad(gameObject);


        unlockedMaps.Add(0);
        // Test Code to Generate Random Numbers
        /*
        completedMaps.Add(0);
        completedMaps.Add(1);
        completedMaps.Add(2);
        completedMaps.Add(3);
        completedMaps.Add(4);
        completedMaps.Add(5);
        completedMaps.Add(6);
        completedMaps.Add(7);
        completedMaps.Add(8);
        unlockedMaps.Add(1);
        unlockedMaps.Add(2);
        unlockedMaps.Add(3);
        unlockedMaps.Add(4);
        unlockedMaps.Add(5);
        unlockedMaps.Add(6);
        unlockedMaps.Add(7);
        unlockedMaps.Add(8);
        
        /*
        completedMaps.Add(2);
        completedMaps.Add(7);
        unlockedMaps.Add(1);
        unlockedMaps.Add(2);
        */

        CleanDuplicates();

        // change this later. must be called when steam has initialized first
        LoadPlayerProgress();

        // Add code that cleanses duplication
    
    }

    public bool IsAllMapsCompleted => completedMaps.Count >= 10;

    public void UnlockAchievement(Achievement achievement) => UnlockAchievement(achievement.Index);

    public void UnlockAchievement(int index)
    {
        unlockedAchievements.Add(index);
        CleanDuplicates();

        SavePlayerProgress();
        OnAchievementUnlocked?.Invoke(index);
    }

    

    public void SendPlayerProgress(
        List<int> cMaps,
        List<int> uMaps,
        List<int> uAchievements)
    {
        completedMaps.Clear();
        unlockedMaps.Clear();
        unlockedAchievements.Clear();

        // Always Add First Map
        unlockedMaps.Add(0);

        completedMaps.AddRange(cMaps);
        unlockedMaps.AddRange(uMaps);
        unlockedAchievements.AddRange(uAchievements);
        
        CleanDuplicates();

        // Save Progress every time NetworkGamePlayer send new data
        SavePlayerProgress();
    }

    public void CleanDuplicates()
    {
        completedMaps = completedMaps.Distinct().ToList();
        unlockedMaps = unlockedMaps.Distinct().ToList();
        unlockedAchievements = unlockedAchievements.Distinct().ToList();
    }

    // Add Code here where it loads Progress from file
    // Call this code when Steam has already initialized.
    public void LoadPlayerProgress()
    {
        Debug.Log("Loading Player Progress...");
        PlayerData data = SaveSystem.LoadPlayer(playerSteamId);

        if (data == null) return;

        if (data.playerSteamId != playerSteamId)
        {
            Debug.Log("Save File not for this user...");
            return;
        }

        completedMaps.Clear();
        unlockedMaps.Clear();
        unlockedAchievements.Clear();

        // Always Add First Map
        unlockedMaps.Add(0);

        completedMaps.AddRange(data.completedMaps);
        unlockedMaps.AddRange(data.unlockedMaps);
        unlockedAchievements.AddRange(data.unlockedAchievements);

        CleanDuplicates();

        gameRating = data.gameRating;


        // Dont Show HowToPlay when theres is a completed map already
        if (completedMaps.Count <= 0) return;
        SettingsAndExit settingsAndExit = FindObjectOfType<SettingsAndExit>();
        if(settingsAndExit) settingsAndExit.showHelpInMapSelect = false;
        
            

    }


    public void SavePlayerProgress()
    {
        Debug.Log("Saving Player Progress...");
        SaveSystem.SavePlayer(this);        
    }

    
    private void OnApplicationQuit()
    {
        // Autosaves whenever the application quits
        // Double check the documentation if game is ported to different OS

        SavePlayerProgress();
    }

}
