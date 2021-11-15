using System.Collections.Generic;
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

            SendPlayerData();
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [Server]
        public void AddCompletedMap(int mapIndex)
        {
            if (completedMaps.Contains(mapIndex)) return;
            completedMaps.Add(mapIndex);
            
        }

        [Server]
        public void AddUnlockMap(int mapIndex)
        {
            if (unlockedMaps.Contains(mapIndex)) return;
            unlockedMaps.Add(mapIndex);
        }

        [Server]
        public void AddUnlockAchievement(int achievementIndex)
        {
            if (unlockedAchievements.Contains(achievementIndex)) return;
            unlockedAchievements.Add(achievementIndex);
        }

        [Server]
        public void ServerSendPlayerData()
        {
            SendPlayerData(
                new List<int>(completedMaps).ToArray(),
                new List<int>(unlockedMaps).ToArray(),
                new List<int>(unlockedAchievements).ToArray());
        }

        [ClientRpc]
        public void SendPlayerData(int[] completedMaps, int[] unlockedMaps, int[] unlockedAchievements)
        {
            if (!hasAuthority) return;
            PlayerProgress localPlayerProgress = FindObjectOfType<PlayerProgress>();

            localPlayerProgress?.SendPlayerProgress(
                new List<int>(completedMaps),
                new List<int>(unlockedMaps),
                new List<int>(unlockedAchievements));
        }

        private void SendPlayerData()
        {
            if (!hasAuthority) return;
            PlayerProgress localPlayerProgress = FindObjectOfType<PlayerProgress>();

            localPlayerProgress?.SendPlayerProgress(
                new List<int>(completedMaps),
                new List<int>(unlockedMaps),
                new List<int>(unlockedAchievements));
        }


    }
}

