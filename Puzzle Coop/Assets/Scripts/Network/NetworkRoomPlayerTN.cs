using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TangentNodes.Network
{
    public class NetworkRoomPlayerTN : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];
        [SerializeField] private Button startGameButton = null;

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string displayName = "Loading...";
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        public SyncList<int> completedMaps = new SyncList<int>();
        public SyncList<int> unlockedMaps = new SyncList<int>();
        public SyncList<int> unlockedAchievements = new SyncList<int>();

        [SyncVar]
        public bool isLeader;

        private NetworkManagerTN room;
        private NetworkManagerTN Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerTN;
            }
        }

        

        public override void OnStartAuthority()
        {
            CmdSetDisplayName("SteamPlayer");

            lobbyUI.SetActive(true);

            // Enable Start Button for Leader
            startGameButton.gameObject.SetActive(isLeader);

            // Tells the SettingsAndExit that player is inConnection
            SettingsAndExit pause = FindObjectOfType<SettingsAndExit>();
            if (pause == null) { return; }
            pause.myPlayerIdentity = this.netIdentity;

            // Place code for passing the Progress of this Player

            
            
            PlayerProgress playerProgress = FindObjectOfType<PlayerProgress>();
            if (playerProgress != null)
            {
                // Add code to save first if not empty or has newer data

                CmdClearPlayerProgress();

                foreach (int i in playerProgress.completedMaps)
                {
                    CmdAddPlayerCompletedMapsProgress(i);
                }

                foreach (int i in playerProgress.unlockedMaps)
                {
                    CmdAddPlayerUnlockedMapsProgress(i);                    
                }
                
                foreach (int i in playerProgress.unlockedAchievements)
                {
                    CmdAddPlayerAchievementProgress(i);
                }

            }


        }

        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);

            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);

            UpdateDisplay();
        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (!hasAuthority)
            {
                foreach (var player in Room.RoomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }

                return;
            }

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                playerReadyTexts[i].text = string.Empty;
            }

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                playerNameTexts[i].text = Room.RoomPlayers[i].displayName;
                playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>";
            }
        }

        public void HandleReadyToStart(bool readyToStart)
        {
            if (!isLeader) { return; }

            startGameButton.interactable = readyToStart;
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [Command]
        public void CmdReadyUp()
        {
            IsReady = !IsReady;

            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        private void CmdAddPlayerCompletedMapsProgress(int index) => completedMaps.Add(index);

        [Command]
        private void CmdAddPlayerUnlockedMapsProgress(int index) => unlockedMaps.Add(index);

        [Command]
        private void CmdAddPlayerAchievementProgress(int index) => unlockedAchievements.Add(index);


        [Command]
        private void CmdClearPlayerProgress()
        {
            unlockedAchievements.Clear();
            unlockedMaps.Clear();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient)
            {
                return;
            }

            Debug.Log("Game Started");
            Room.StartGame();
        }

        public void PlayUIButtonClick()
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
        }
    }
}

