using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TerminalManager : NetworkBehaviour
{
    public TerminalMessage messageSet = null;

    [SerializeField] private UI_TerminalScreen terminalScreen = null;
    
    public MapObjectManager_S serverObjectManager = null;

    private int currentMessage = 0;

    private int playersReadyToDownload = 0;


    // Add Code that will call RPC to setup Terminals and Msgs
    // Add Method that will Handle when Players are ready to download
    // Add Method that will handle event when Hallway is Entered... (create fade to black transition)
    // Add Method that will handle command when fade is done...(after animation)
}
