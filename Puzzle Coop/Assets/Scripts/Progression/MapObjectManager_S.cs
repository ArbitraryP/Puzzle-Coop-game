using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;
using UnityEngine.SceneManagement;
using System.Linq;

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
    [SerializeField] private QuestionsManager questionsManager = null;

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
        NetworkManagerTN.OnServerReadied += InitializeScene;
    }

    private void OnDestroy()
    {
        NetworkManagerTN.OnServerReadied -= InitializeScene;
    }
        

    [Server]
    private void InitializeScene(NetworkConnection conn)
    {
        if(SceneManager.GetActiveScene().name == "Scene_Map_02_Misused")
        {
            questionsManager = FindObjectOfType<QuestionsManager>();
            if (questionsManager)
            {
                questionsManager.serverObjectManager = this;
                questionsManager.InitializeQuestions(Room.currentMap.Index);
            }
        }
        RpcInitializePlayers();

        // Check if Players are ready at the scene
        CheckToStartMap();

    }

    [Server]
    private void CheckToStartMap()
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }

        // Add Code that Closes or Stops the "Waiting for Other Player" loading screen or smth

        questionsManager?.ResetQuestions(); // Code that starts question if it is Map02 etc.
    }

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
    public void CmdSpawnMapCompleteMsg()
    {
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

            Room.isCurrentMapCompleted = true;
            Room.ServerChangeScene("Scene_Map_Select");


        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUnlockDoors() => RpcUnlockDoors();

    [ClientRpc]
    private void RpcUnlockDoors() => localObjectManager.UnlockDoors();



    #region 00 Tutorial

    [Command(requiresAuthority = false)]
    public void CmdM00_PowerOn() => RpcM00_PowerOn();

    [ClientRpc]
    private void RpcM00_PowerOn() => localObjectManager.M00_PowerOnAction();


    #endregion

    #region 01 Intro To IT

    [Command(requiresAuthority = false)]
    public void CmdM01_PowerOn() => RpcM01_PowerOn();

    [ClientRpc]
    private void RpcM01_PowerOn() => localObjectManager.M01_PowerOnAction();



    [ClientRpc]
    public void CmdM01_BreakerOn() => localObjectManager.UnlockDoors();

    #endregion


    #region 02 Misused Terms

    [ClientRpc]
    public void RpcM02_QuizCompleted() => localObjectManager.UnlockDoors();

    #endregion


}
