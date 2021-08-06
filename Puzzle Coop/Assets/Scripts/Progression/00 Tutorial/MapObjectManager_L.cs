using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;

public class MapObjectManager_L : MonoBehaviour
{

    public MapObjectManager_S serverObjectManager = null;
    [SerializeField] private ExitDoor[] exitDoors = null;
    [SerializeField] private CameraControl cameraControl = null;

    [Header("00 Tutorial Map")]
    [SerializeField] private GameObject passCodeScreen = null;
    [SerializeField] private GameObject darkFilter_00 = null;

    [Header("01 Intro to IT")]
    [SerializeField] private GameObject darkFilter_01 = null;

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    public void InitializePlayer()
    {
        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) continue;
            cameraControl.isHostPlayer = player.isLeader;
            cameraControl.ResetCamera();
        }
    }

    #region 00 Tutorial
    
    public void M00_PowerOnAction()
    {
        darkFilter_00.SetActive(false);
        passCodeScreen.SetActive(true);
        exitDoors[0].gameObject.SetActive(true);
        exitDoors[1].gameObject.SetActive(true);

        // Play sound. Show text
    }

    public void M00_PassCodeAction()
    {
        exitDoors[0].isUnlocked = true;
        exitDoors[1].isUnlocked = true;

        // Play sound correct. Show text

    }

    #endregion




}
