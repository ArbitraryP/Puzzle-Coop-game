using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    // This class is where the progress of the player will get loaded

    public string playerName;
    public int playerSteamId; // adjust this based on steamID datatype

    public List<int> unlockedMaps;
    public List<int> unlockedAchievements;

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


        // Test Code to Generate Random Numbers


        for (int i = 0; i < 3; i++)
        {
            unlockedMaps.Add(Random.Range(1, 10));
            unlockedAchievements.Add(Random.Range(1, 10));
        }
    }

    // Add Code here where it loads Progress from file


}
