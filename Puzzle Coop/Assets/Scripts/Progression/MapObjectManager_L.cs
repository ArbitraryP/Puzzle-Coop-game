using UnityEngine;
using Mirror;
using TangentNodes.Network;
using UnityEngine.Experimental.Rendering.Universal;

public class MapObjectManager_L : MonoBehaviour
{

    public MapObjectManager_S serverObjectManager = null;
    [SerializeField] private ExitDoor[] exitDoors = null;
    [SerializeField] private CameraControl cameraControl = null;
    [SerializeField] private WelcomeNote welcomeNote = null;

    [Header("00 Tutorial Map")]
    [SerializeField] private GameObject passCodeScreen = null;
    [SerializeField] private GameObject darkFilter = null;

    [Header("01 Intro to IT")]
    [SerializeField] private BreakerButton[] breakerButtons = null;
    [SerializeField] private NodesMove[] nodes = null;
    [SerializeField] private Light2D lightGlowNode = null;
    [SerializeField] private Light2D lightGlowBreaker = null;

    [Header("03 Miscons Convo")]
    [SerializeField] private UI_Receiver receiver = null;
    [SerializeField] private UI_Calendar calendar = null;
    [SerializeField] private UI_Clock clock = null;
    [SerializeField] private SentenceSet sentenceSet = null;
    private bool isCalendarCompleted = false;
    private bool isClockCompleted = false;

    [Header("04 Career Gallery")]
    [SerializeField] private Gallery gallery = null;
    [SerializeField] private PhotoFrame photoFrame = null;
    [SerializeField] private NavBreaker navBreaker = null;

    [Header("09 Terminal")]
    [SerializeField] private GameObject hallwayOn = null;
    [SerializeField] private GameObject panelAfterVideo = null;


    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    public void InitializePlayer(string mapName)
    {
        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) continue;
            cameraControl.isHostPlayer = player.isLeader;
            cameraControl.ResetCamera();

            if (!welcomeNote)
            {
                Debug.LogWarning("Scene has no WelcomeNote assigned");
                return;
            }

            welcomeNote.SetMap(mapName);
            welcomeNote.SetText(player.isLeader);
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
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_MAP_DoorUnlocked);
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_MAP_AccessGrant);
    }

    #region 00 Tutorial

    public void M00_PowerOnAction()
    {
        darkFilter.SetActive(false);
        passCodeScreen.SetActive(true);

        // Play sound. Show text
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M00_PlugIn);
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

        lightGlowBreaker.color = Color.green;

        // Play sound. Show text
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M00_PlugIn);
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
        lightGlowNode.color = Color.green;
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
            receiver.InitializeUIReceiver(sentenceSet);

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
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M03_CalendarClockCorrect);
        M03_CheckIfAllPuzzlesCompleted();
    }

    public void M03_OnClockCompleted()
    {
        clock.clockLight.color = Color.green;
        isClockCompleted = true;

        // play sound
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M03_CalendarClockCorrect);
        M03_CheckIfAllPuzzlesCompleted();
    }

    private void M03_CheckIfAllPuzzlesCompleted()
    {
        if (isClockCompleted && isCalendarCompleted)
            UnlockDoors();
    }

    #endregion



    #region 04 Career Gallery

    public void M04_SetupPuzzles(int navBreakerIndex, int careerIndex)
    {
        photoFrame.SetCareerSolution(careerIndex);
        navBreaker.SetLightSolution(navBreakerIndex);
    }

    public void M04_PowerOnAction()
    {
        darkFilter.SetActive(false);
        gallery.ShowCode = true;
        photoFrame.codeEnabled = true;
        // Play sound. Show text

        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M00_PlugIn);
    }

    #endregion



    #region 09 Terminal

    public void M09_UnlockHallway()
    {
        hallwayOn.SetActive(true);
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M09_HallwayLightsOn);
    }

    public void M09_HideUI()
    {
        cameraControl.EnableNavigation(false);
        FindObjectOfType<SettingsAndExit>()?.EnableMenu(false);
        // Disable Floor Navigation
        // Disable Quiting the game
    }

    public void M09_OnVideoPlayerEnded(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        panelAfterVideo.SetActive(true);
        cameraControl.EnableNavigation(true);
        FindObjectOfType<SettingsAndExit>()?.EnableMenu(true);

        videoPlayer.enabled = false;
        Debug.Log("TERMINAL MAP COMPLETED! hehe");
        // Tell server that player is done

        serverObjectManager.CmdExitDoor();
    }

    #endregion


}
