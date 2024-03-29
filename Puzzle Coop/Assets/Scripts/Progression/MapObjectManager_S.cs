using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Video;

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
    [SerializeField] private TerminalManager terminalManager = null;
    [SerializeField] private IntroCutscenePlayer cutscenePlayer = null;

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
    private int numberOfPlayersVoteSkip = 0;

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
        RpcInitializePlayers(Room.currentMap.name);

        //Initialize Cutscene
        RpcSetupCutscene(Room.currentMap.name);
        

        if (SceneManager.GetActiveScene().name == "Scene_Map_02_Misused")
        {
            questionsManager = FindObjectOfType<QuestionsManager>();
            if (questionsManager)
            {
                questionsManager.serverObjectManager = this;
                questionsManager.InitializeQuestions(Room.currentMap.Index);
            }
        }

        if (SceneManager.GetActiveScene().name == "Scene_Map_03_MisConvo")
            RpcM03_InitializeSentences(Room.currentMap.Index);

        if (SceneManager.GetActiveScene().name == "Scene_Map_04_CareerGallery")
        {
            int lightRNG = Random.Range(0, 3);
            int careerRNG = Random.Range(0, 7); //Change this based on Max count of Careers. If needed


            RpcM04_InitializeCareerAndLight(lightRNG, careerRNG);
            // call RPC to initialize these values
        }

        if (SceneManager.GetActiveScene().name == "Scene_Map_09_Final")
        {
            terminalManager = FindObjectOfType<TerminalManager>();
            if (terminalManager)
            {
                terminalManager.serverObjectManager = this;
                terminalManager.InitializeTerminal();
            }
        }


        // Check if Players are ready at the scene
        CheckToStartMap();

    }

    [Server]
    private void CheckToStartMap()
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }

        questionsManager?.ResetQuestions(); // Code that starts question if it is Map02 etc.
        terminalManager?.NextMessage();

        // Code that tells the Cutscene to start playing
        RpcPlayCutscene();
    }


    [ClientRpc]
    private void RpcInitializePlayers(string mapName)
    {

        localObjectManager = FindObjectOfType<MapObjectManager_L>();
        if (!localObjectManager)
        {
            Debug.Log("Local Map Object Manager Missing!");
            return;
        }
        localObjectManager.InitializePlayer(mapName);
        localObjectManager.serverObjectManager = this;
    }

    #region Cutscenes

    [ClientRpc]
    private void RpcSetupCutscene(string mapName)
    {
        cutscenePlayer.SetupCutscene(mapName);
    }

    [ClientRpc]
    private void RpcPlayCutscene()
    {
        cutscenePlayer.PlayCutscene();
    }

    [Command(requiresAuthority =false)]
    public void CmdVoteSkipCutscene()
    {
        numberOfPlayersVoteSkip++;
        if (numberOfPlayersVoteSkip >= 2)
        {
            RpcSkipCutscene();
            numberOfPlayersVoteSkip = 0;
        }
    }

    [ClientRpc]
    private void RpcSkipCutscene() => cutscenePlayer.SkipCutscene();
    

    #endregion

    [Command(requiresAuthority = false)]
    public void CmdExitDoor()
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

                // Unlocks Final Map if all completed
                List<int> completedMaps = new List<int>(player.completedMaps);
                if (completedMaps.Exists(i => i > 0 && i <= 8))
                    player.AddUnlockMap(9);

                // Unlocks Achievements
                foreach (Achievement achievementToUnlock in Room.currentMap.achievements)
                    player.AddUnlockAchievement(achievementToUnlock.Index);

                // Removes all Unlocked Maps IF Final Map is Finished
                if(Room.currentMap.name == "09 Final Level")
                {
                    player.completedMaps.Clear();
                    player.unlockedMaps.Clear();
                    player.unlockedMaps.Add(0); // Adds Map S, the first map

                }


                // Sync Progress to Local PlayerProgress Object
                player.ServerSendPlayerData();

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


    #region 03 Miscon Convo

    [ClientRpc]
    private void RpcM03_InitializeSentences(int currentMapIndex) 
    {
        localObjectManager.M03_InitializeClockCalendarReceiver(currentMapIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdM03_CalendarCompleted() => RpcM03_CalendarCompleted();

    [ClientRpc]
    public void RpcM03_CalendarCompleted() => localObjectManager.M03_OnCalendarCompleted();

    [Command(requiresAuthority = false)]
    public void CmdM03_ClockCompleted() => RpcM03_ClockCompleted();

    [ClientRpc]
    public void RpcM03_ClockCompleted() => localObjectManager.M03_OnClockCompleted();

    #endregion


    #region 04 Career Gallery

    [ClientRpc] 
    private void RpcM04_InitializeCareerAndLight(int correctLightIndex, int correctCareerIndex)
    {
        localObjectManager.M04_SetupPuzzles(correctLightIndex, correctCareerIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdM04_LightCompleted() => RpcM04_LightCompleted();

    [ClientRpc]
    private void RpcM04_LightCompleted() => localObjectManager.M04_PowerOnAction();

    #endregion


    #region 09 Terminal

    [Server]
    public void M09_UnlockHallway()
    {
        RpcM09_UnlockHallway();
    }

    [ClientRpc]
    private void RpcM09_UnlockHallway() => localObjectManager.M09_UnlockHallway();

    #endregion


}
