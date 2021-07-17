using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TangentNodes.Network
{
    public class NetworkGamePlayerTN : NetworkBehaviour
    {
        
        [SyncVar]
        public string displayName = "Loading...";

        SyncList<int> unlockedMaps = new SyncList<int>();
        SyncList<int> unlockedAchievements = new SyncList<int>();

        private bool isLeader;
        public bool IsLeader
        {
            get { return isLeader; }
            set
            {
                isLeader = value;
            }
        }

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
            // Tells the SettingsAndExit that player is inConnection
            SettingsAndExit pause = FindObjectOfType<SettingsAndExit>();
            if (pause == null) { return; }
            pause.myPlayerIdentity = this.netIdentity;
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            Room.GamePlayers.Add(this);
        }

        public override void OnStopClient()
        {
            Room.GamePlayers.Remove(this);
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

    }
}

