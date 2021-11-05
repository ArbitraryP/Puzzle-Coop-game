using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TangentNodes.Network
{
    public class NetworkManagerTN : NetworkManager
    {
        
        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;
        
        /*
        [Header("Maps")]
        [SerializeField] private int numberOfRounds = 1;
        [SerializeField] private MapSet mapSet = null;
        */

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerTN roomPlayerPrefab = null;
        [SerializeField] private DisconnectPanel disconnectPanelPrefab = null;
        
        [Header("Game")]
        [SerializeField] private NetworkGamePlayerTN gamePlayerPrefab = null;
        [SerializeField] private MapSelect mapSelectPrefab = null;
        [SerializeField] private MapObjectManager_S objectManagerPrefab = null;
        [SerializeField] private QuestionsManager questionsManagerPrefab = null;
        [SerializeField] private TerminalManager terminalManagerPrefab = null;
        [SerializeField] private MapCompleteMessage mapCompleteMsgPrefab = null;

        /*
        [SerializeField] private GameObject playerSpawnSystem = null;
        [SerializeField] private GameObject roundSystem = null;
        
        private MapHandler mapHandler;
        */


        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;
        
        public List<NetworkRoomPlayerTN> RoomPlayers { get; } = new List<NetworkRoomPlayerTN>();
        public List<NetworkGamePlayerTN> GamePlayers { get; } = new List<NetworkGamePlayerTN>();

        [Header("In Game")]
        public Map currentMap = null;
        public bool isCurrentMapCompleted = false;


        public void ClearCurrentMap()
        {
            currentMap = null;
            isCurrentMapCompleted = false;
        }

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                ClientScene.RegisterPrefab(prefab);
            }
        }

        new private void Start()
        {
            menuScene = Path.GetFileNameWithoutExtension(menuScene).ToString();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnClientDisconnected?.Invoke();

            // Close Loading Screen
            FindObjectOfType<SettingsAndExit>()?.EnableLoadingScreen(false);

            // Show Disconnected Panel
            Instantiate(disconnectPanelPrefab);
        }
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().name != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;

                NetworkRoomPlayerTN roomPlayerInstance = Instantiate(roomPlayerPrefab);
                
                roomPlayerInstance.isLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }
        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerTN>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();

                //OnServerDisconnected?.Invoke(conn);

                // Write some code here for AutoSaving Progress


                if (RoomPlayers.Count < 2 && SceneManager.GetActiveScene().name != menuScene) 
                {
                    // Check if partner really left. Not by a 3rd player disconnecting and is not in Lobby
                    // Since there's only 1 joining. Immediately show Disconnected Client Message.
                    DisconnectPanel disconnectPanel = Instantiate(disconnectPanelPrefab);
                    disconnectPanel.ChangeTextToLostClient();

                    // Close Loading Screen
                    FindObjectOfType<SettingsAndExit>()?.EnableLoadingScreen(false);

                }
            }

            base.OnServerDisconnect(conn);
        }
        
        public override void OnStopServer()
        {
            //OnServerStopped?.Invoke();

            RoomPlayers.Clear();
            //GamePlayers.Clear();
            ClearCurrentMap();

            base.OnStopServer();



            SceneManager.LoadScene("Scene_Lobby");
            Debug.Log("Server Stopped, Scene Changed");
        }
        
        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }
        
        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }

            return true;
        }
        
        public void StartGame()
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                if (!IsReadyToStart()) { return; }

                //mapHandler = new MapHandler(mapSet, numberOfRounds);

                //ServerChangeScene(mapHandler.NextMap);

                ServerChangeScene("Scene_Map_Select");
            }
        }
        
        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    gameplayerInstance.SetDisplayName(RoomPlayers[i].displayName);
                    gameplayerInstance.isLeader = RoomPlayers[i].isLeader;

                    foreach (int index in RoomPlayers[i].completedMaps)
                        gameplayerInstance.completedMaps.Add(index);
                    
                    foreach (int index in RoomPlayers[i].unlockedMaps)
                        gameplayerInstance.unlockedMaps.Add(index);
                    
                    foreach (int index in RoomPlayers[i].unlockedAchievements)
                        gameplayerInstance.unlockedAchievements.Add(index);
                    

                    NetworkServer.Destroy(conn.identity.gameObject);

                    // Modified Code: Added True in replace and swap position of destroy and replace
                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
                }
            }

            base.ServerChangeScene(newSceneName);

            // Runs Loading Screen When Server changes Scene
            StartLoadingScreen();
        }

        // Runs Loading Screen When Client changes Scene
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            StartLoadingScreen();
        }


        private void StartLoadingScreen()
        {
            FindObjectOfType<SettingsAndExit>()?.EnableLoadingScreen(true);
        }



        public override void OnServerSceneChanged(string sceneName)
        {
            
            if (sceneName.StartsWith("Scene_Map"))
            {
                /*
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
                
                GameObject roundSystemInstance = Instantiate(roundSystem);
                NetworkServer.Spawn(roundSystemInstance);
                */

                if (sceneName == "Scene_Map_Select")
                {
                    

                    foreach (NetworkGamePlayerTN player in GamePlayers)
                    {
                        // Spawns a Map UI when loaded owned by leader
                        if (!player.isLeader) { continue; }
                        GameObject mapSelectPrefabInstance = Instantiate(mapSelectPrefab.gameObject);
                        NetworkServer.Spawn(mapSelectPrefabInstance, player.connectionToClient);

                    }
                }
                else
                {
                    // Spawn Object Manager for server
                    GameObject objectManagerInstance = Instantiate(objectManagerPrefab.gameObject);
                    NetworkServer.Spawn(objectManagerInstance);


                    GameObject mapCompleteMsgInstance = Instantiate(mapCompleteMsgPrefab.gameObject);
                    NetworkServer.Spawn(mapCompleteMsgInstance);

                    mapCompleteMsgInstance.GetComponent<MapCompleteMessage>()?.SetUpMapMessage(currentMap.name);
                }

                if (sceneName == "Scene_Map_02_Misused")
                {
                    // Spawn Question Manager
                    GameObject questionManagerInstance = Instantiate(questionsManagerPrefab.gameObject);
                    NetworkServer.Spawn(questionManagerInstance);
                }

                if (sceneName == "Scene_Map_09_Final")
                {
                    // Spawn Terminal Manager
                    GameObject terminalManagerInstance = Instantiate(terminalManagerPrefab.gameObject);
                    NetworkServer.Spawn(terminalManagerInstance);
                }

            }


        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);
        }

    }
}
