using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;


public class ButtonNetTestLocal : MonoBehaviour
{
    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    public void OnClickTestButton()
    {
        int rng = Random.Range(1, 10);
        ButtonNetTest button = FindObjectOfType<ButtonNetTest>();
        button?.CmdChangeButtonText("Clicked RNG: " + rng);
        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            Debug.Log("hasAuthority: " + player.hasAuthority);
            if (player.hasAuthority)
            {
                if(button.netIdentity.AssignClientAuthority(player.connectionToClient))
                    button?.CmdChangeButtonText("Clicked RNG: " + rng);

                // Change the authority of ButtonNetTest to client before calling command
            }
        }
        
        
    }
}
