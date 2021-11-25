using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Steamworks;

public class IpAddressChecker : MonoBehaviour
{
    
    void Start()
    {
        // Shows LAN IP Address if there's no Steam
        GetComponent<TMP_Text>().text = "My IP Address: "+ GetLocalIPAddress();


        // Shows Player's name if Steam is initialized
        if (!SteamManager.Initialized) { return; }

        string name = SteamFriends.GetPersonaName();
        GetComponent<TMP_Text>().text = "Welcome: " + name;
    }


    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "My IP Address: No connection";
    }
}
