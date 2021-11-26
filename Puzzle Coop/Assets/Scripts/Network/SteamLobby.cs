using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

namespace TangentNodes.Network
{
    public class SteamLobby : MonoBehaviour
    {
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;

        private const string HostAddressKey = "HostAddress";

        [SerializeField] private NetworkManagerTN networkManager;

        private void Start()
        {
            networkManager = GetComponent<NetworkManagerTN>();

            if (!SteamManager.Initialized)
            {
                Debug.Log("STEAM NOT initialized");
                return;
            }


            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
            // Adjust LobbyType if players should not be able to join game without invite
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);

        }


        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.Log("Steam Lobby not Created" + callback.m_eResult.ToString());
                return;
            }

            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey,
                SteamUser.GetSteamID().ToString());

            SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(callback.m_ulSteamIDLobby));

        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

            // Code that jumps to Join Waiting Area
            MainMenu mainMenu = FindObjectOfType<MainMenu>();
            if (!mainMenu) return;

            mainMenu.JumpToJoinWait("Joining Game...", false);

        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) { return; }

            if (callback.m_EChatRoomEnterResponse != (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                // Code that tells if join is NOT successfull
                MainMenu mainMenu = FindObjectOfType<MainMenu>();
                if (mainMenu)
                    mainMenu.JumpToJoinWait("Join Failed...", true);
            }


            string hostAdress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);

            networkManager.networkAddress = hostAdress;
            networkManager.StartClient();
            
        }


    }
}

