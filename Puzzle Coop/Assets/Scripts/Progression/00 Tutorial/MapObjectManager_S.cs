using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;
using UnityEngine.SceneManagement;

/// <summary>
/// This object is owned by players through network
/// Searches for LocalMapObjectManager to Initialize based on Leader status
/// 
/// This class will check if players are ready in the Scene and
/// communicates wth Local MapObjectManager to Initialize values for respective players
/// </summary>
public class MapObjectManager_S : NetworkBehaviour
{


    [SerializeField] private MapObjectManager_L localObjectManager = null;

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    private int numberOfPlayersReadyToExit = 0;



    public override void OnStartServer()
    { 
        NetworkManagerTN.OnServerReadied += InitializePlayers;
    }

    private void OnDestroy() =>
        NetworkManagerTN.OnServerReadied -= InitializePlayers;

    private void InitializePlayers(NetworkConnection conn) => RpcInitializePlayers();

    [ClientRpc]
    private void RpcInitializePlayers()
    {

        localObjectManager = FindObjectOfType<MapObjectManager_L>();
        if (!localObjectManager)
        {
            Debug.Log("Local Map Object Manager Missing!");
            return;
        }
        localObjectManager.InitializePlayer();
        localObjectManager.serverObjectManager = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdExitDoor(int playerNum)
    {
        numberOfPlayersReadyToExit++;
        CheckReadyToExit();
    }

    [Server]
    private void CheckReadyToExit()
    {
        if(numberOfPlayersReadyToExit >= 2)
        {
            numberOfPlayersReadyToExit = 0;

            foreach(NetworkGamePlayerTN player in Room.GamePlayers)
            {
                Debug.Log(" Current MAP is : " + Room.currentMap);
                player.AddCompletedMap(Room.currentMap.Index);

                foreach(Map mapsToUnlock in Room.currentMap.unlocks)
                    player.AddUnlockMap(mapsToUnlock.Index);
                
                // Invoke Achievement if ever there is
            }

            Room.ServerChangeScene("Scene_Map_Select");


        }
    }

    #region 00 Tutorial

    [Command(requiresAuthority = false)]
    public void CmdM00_PowerOn() => RpcM00_PowerOn();

    [ClientRpc]
    private void RpcM00_PowerOn() => localObjectManager.M00_PowerOnAction();


    [Command(requiresAuthority = false)]
    public void CmdM00_PassCodeOn() => RpcM00_PassCodeOn();

    [ClientRpc]
    private void RpcM00_PassCodeOn() => localObjectManager.M00_PassCodeAction();

    #endregion




}
