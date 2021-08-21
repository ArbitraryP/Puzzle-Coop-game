using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;
using System;

public class MapObjectManager_L : MonoBehaviour
{

    public MapObjectManager_S serverObjectManager = null;
    [SerializeField] private ExitDoor[] exitDoors = null;
    [SerializeField] private CameraControl cameraControl = null;

    [Header("00 Tutorial Map")]
    [SerializeField] private GameObject passCodeScreen = null;
    [SerializeField] private GameObject darkFilter = null;

    [Header("01 Intro to IT")]
    [SerializeField] private GameObject sampleObject = null;

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

    public void UnlockDoors()
    {
        foreach(ExitDoor door in exitDoors)
        {
            if (!door) continue;
            door.gameObject.SetActive(true);
            door.isUnlocked = true;
        }

        // Play unlocked door sound and Show doors unlock text
    }

    #region 00 Tutorial

    public void M00_PowerOnAction()
    {
        darkFilter.SetActive(false);
        passCodeScreen.SetActive(true);

        // Play sound. Show text
    }


    #endregion

    #region 02 Misused


    #endregion


}
