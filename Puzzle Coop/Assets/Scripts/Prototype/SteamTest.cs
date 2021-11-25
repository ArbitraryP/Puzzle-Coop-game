using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!SteamManager.Initialized) { return; }

        string name = SteamFriends.GetPersonaName();
        ulong steamid = SteamUser.GetSteamID().m_SteamID;
        Debug.Log("The game is connected to Steam: " + name + " : " + steamid);
    }
}
