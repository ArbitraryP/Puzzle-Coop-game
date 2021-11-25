using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if(!SteamManager.Initialized) { return; }

        //string name = SteamFriends.GetPersonaName();
        Debug.Log("The game is connected to Steam: " + name);
    }
}
