using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;

public class TerminalManager : NetworkBehaviour
{
    public TerminalMessage messageSet = null;

    [SerializeField] private UI_TerminalScreen terminalScreen = null;
    
    public MapObjectManager_S serverObjectManager = null;

    [SerializeField] private int currentMessage = 0;

    private int playersReadyToDownload = 0;
    private int playersDownloaded = 0;
    private int playersReadyToRetry = 0;

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }


    private void Awake()
    {
        terminalScreen = FindObjectOfType<UI_TerminalScreen>();
    }

    [Server]
    public void InitializeTerminal()
    {
        RpcInitializeTerminal();
    }

    [ClientRpc]
    private void RpcInitializeTerminal()
    {
        foreach(NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) continue;
            terminalScreen.SetupTerminal(player.isLeader, messageSet.messages.Count);
            return;
        }
    }

    [Server]
    public void NextMessage()
    {
        currentMessage++;

        if (currentMessage - 1 >= messageSet.messages.Count)
            return;

        RpcSetTerminalMessage(
            currentMessage,
            messageSet.messages[currentMessage - 1],
            currentMessage - 1 == messageSet.messages.Count - 1);
    }

    [ClientRpc]
    private void RpcSetTerminalMessage(int currentMsg, string newMsg, bool isFinalMsg)
    {
        terminalScreen.SetTerminalMessage(currentMsg, newMsg, isFinalMsg);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayerReadytoDownload()
    {
        playersReadyToDownload++;
        if (playersReadyToDownload < 2) return;

        playersReadyToDownload = 0;
        RpcStartDownload();


    }

    [ClientRpc]
    private void RpcStartDownload()
    {
        terminalScreen.StartDownload();
    }


    [Command(requiresAuthority = false)]
    public void CmdPlayerReadyToRetry()
    {
        playersReadyToRetry++;
        if (playersReadyToRetry < 2) return;

        playersReadyToRetry = 0;
        RpcStartRetry();
    }

    [ClientRpc]
    private void RpcStartRetry()
    {
        terminalScreen.StartRetry();
    }

    [Command(requiresAuthority = false)]
    public void CmdDownloadCompleted()
    {
        playersDownloaded++;
        if (playersDownloaded < 2) return;

        playersDownloaded = 0;
        if (currentMessage - 1 < messageSet.messages.Count - 1)
        {
            serverObjectManager.M09_UnlockHallway();
            NextMessage();
            return;
        }
        
        // --- Code that runs when it is the Last Message ---

        Debug.Log("Terminal Map Completed.");

    }

    

    // Add Code that will call RPC to setup Terminals and Msgs
    // Add Method that will Handle when Players are ready to download
    // Add Method that will handle event when Hallway is Entered... (create fade to black transition)
    // Add Method that will handle command when fade is done...(after animation)
}
