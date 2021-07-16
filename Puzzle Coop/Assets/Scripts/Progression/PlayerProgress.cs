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
    }

    // Add Code here where it loads Progress from file


}
