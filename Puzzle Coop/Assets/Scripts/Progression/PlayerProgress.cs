using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    // This class is where the progress of the player will get loaded locally

    public string playerName;
    public int playerSteamId; // adjust this based on steamID datatype

    [SerializeField] private List<int> completedMaps;
    [SerializeField] private List<int> unlockedMaps;
    [SerializeField] private List<int> unlockedAchievements;

    public List<int> CompletedMaps => completedMaps;
    public List<int> UnlockedMaps => unlockedMaps;
    public List<int> UnlockedAchievements => unlockedAchievements;

    public int gameRating = 0;

    public static event Action<int> OnAchievementUnlocked;

    private static PlayerProgress instance;

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
        /*
        completedMaps.Add(2);
        completedMaps.Add(7);
        unlockedMaps.Add(1);
        unlockedMaps.Add(2);
        */
        for (int i = 0; i < 3; i++)
        {
            //completedMaps.Add(Random.Range(1, 10));
            //unlockedMaps.Add(Random.Range(1, 10));
            //unlockedAchievements.Add(Random.Range(1, 10));
        }

        CleanDuplicates();


        // Add code that cleanses duplication
    
    }

    public bool IsAllMapsCompleted => completedMaps.Count >= 10;

    public void UnlockAchievement(Achievement achievement) => UnlockAchievement(achievement.Index);

    public void UnlockAchievement(int index)
    {
        unlockedAchievements.Add(index);
        CleanDuplicates();

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
    }

    public void CleanDuplicates()
    {
        completedMaps = completedMaps.Distinct().ToList();
        unlockedMaps = unlockedMaps.Distinct().ToList();
        unlockedAchievements = unlockedAchievements.Distinct().ToList();
    }

    // Add Code here where it loads Progress from file


}
