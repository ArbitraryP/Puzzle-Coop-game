using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Net.Sockets;

public class IpAddressChecker : MonoBehaviour
{
    
    void Start()
    {
        GetComponent<TMP_Text>().text = "My IP Address: "+ GetLocalIPAddress();
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
