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

    [Header("03 Miscons Convo")]
    [SerializeField] private UI_Receiver receiver = null;
    [SerializeField] private UI_Calendar calendar = null;
    [SerializeField] private UI_Clock clock = null;
    [SerializeField] private SentenceSet sentenceSet = null;
    private bool isCalendarCompleted = false;
    private bool isClockCompleted = false;

    [Header("04 Career Gallery")]
    [SerializeField] private Gallery gallery = null;
    

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



    #region 03 Miscons Convo

    public void M03_InitializeClockCalendarReceiver(int currentMapIndex)
    {
        var AllSets = Resources.LoadAll<SentenceSet>("ScriptableObjects/Sentences");

        foreach (var set in AllSets)
        {
            if (set.AssociateMap.Index != currentMapIndex) continue;
            sentenceSet = set;

            // Loads the Sentences to the Receiver
            receiver.InitializeSentences(sentenceSet);

            // Setup the Correct Solution of Clock and Calendar
            calendar.SetSolution(sentenceSet.CorrectMonth, sentenceSet.CorrectDate);
            clock.SetSolution(sentenceSet.CorrectHour, sentenceSet.CorrectMinutes, sentenceSet.CorrectMeridiemAM);

            // Either Show Calendar or Clock to Either player based on Sentence Set
            if (cameraControl.isHostPlayer)
            {
                calendar.iconButton.gameObject.SetActive(sentenceSet.ShowCalendarHideClock);
                clock.iconButton.gameObject.SetActive(!sentenceSet.ShowCalendarHideClock);
            }
            else
            {
                calendar.iconButton.gameObject.SetActive(!sentenceSet.ShowCalendarHideClock);
                clock.iconButton.gameObject.SetActive(sentenceSet.ShowCalendarHideClock);
            }
            

            break;
        }

    }

    public void M03_OnCalendarCompleted()
    {
        calendar.calendarLight.color = Color.green;
        isCalendarCompleted = true;

        // play sound
        M03_CheckIfAllPuzzlesCompleted();
    }

    public void M03_OnClockCompleted()
    {
        clock.clockLight.color = Color.green;
        isClockCompleted = true;

        // play sound
        M03_CheckIfAllPuzzlesCompleted();
    }

    private void M03_CheckIfAllPuzzlesCompleted()
    {
        if (isClockCompleted && isCalendarCompleted)
            UnlockDoors();
    }

    #endregion


}
