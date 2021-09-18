using UnityEngine;
using Mirror;
using TangentNodes.Network;
using UnityEngine.Experimental.Rendering.Universal;

public class MapObjectManager_L : MonoBehaviour
{

    public MapObjectManager_S serverObjectManager = null;
    [SerializeField] private ExitDoor[] exitDoors = null;
    [SerializeField] private CameraControl cameraControl = null;

    [Header("00 Tutorial Map")]
    [SerializeField] private GameObject passCodeScreen = null;
    [SerializeField] private GameObject darkFilter = null;

    [Header("01 Intro to IT")]
    [SerializeField] private BreakerButton[] breakerButtons = null;
    [SerializeField] private NodesMove[] nodes = null;
    [SerializeField] private Light2D lightGlow = null;

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



    #region 01 IntroToIT

    public bool M01_IsCombinationCorrect()
    {
        foreach (BreakerButton button in breakerButtons)
        {
            if (!button.isSelectedCorrect())
            {
                //Play wrong sound
                return false;
            }
        }


        serverObjectManager.CmdM01_PowerOn();
        return true;

    }

    public void M01_PowerOnAction()
    {
        darkFilter.SetActive(false);
        foreach (NodesMove node in nodes)
            node.isEnabled = true;

        // Play sound. Show text
    }

    public void M01_OnNodeInserted()
    {
        foreach (NodesMove node in nodes)
        {
            if (!node.isInCorrectSlot)
                return;
        }

        foreach (NodesMove node in nodes)
            node.isEnabled = false;

        serverObjectManager.CmdUnlockDoors();
        lightGlow.color = Color.green;
    }

    #endregion


}
