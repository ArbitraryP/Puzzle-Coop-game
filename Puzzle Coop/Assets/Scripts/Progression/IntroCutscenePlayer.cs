using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroCutscenePlayer : MonoBehaviour
{
    [SerializeField] private Map currentMap = null;
    [SerializeField] private VideoPlayer videoPlayer = null;
    [SerializeField] private MapObjectManager_S serverObjectManager = null;

    private bool hasVotedSkip = false;

    private void Awake()
    {
        videoPlayer.loopPointReached += OnCutsceneEnded;
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnCutsceneEnded;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasVotedSkip)
        {
            serverObjectManager.CmdVoteSkipCutscene();
            hasVotedSkip = true;
        }
                   
    }

    public void SetupCutscene(string mapName)
    {
        // Set the AudioSource of Cutscene Video to the provided AudioSource from AudioManager
        videoPlayer.SetTargetAudioSource(0, FindObjectOfType<AudioManager>()?.GetCutsceneAudioSource());


        currentMap = Resources.Load<Map>("ScriptableObjects/Maps/" + mapName);
        if (!currentMap)
        {
            Debug.LogWarning(mapName + ": Map not Found.");
            return;
        }

        videoPlayer.clip = currentMap.cutsceneVideo;
        videoPlayer.Stop();
    }

    public void PlayCutscene()
    {
        if (!currentMap.cutsceneVideo)
        {
            Debug.Log("Map has no intro cutscene video clip");
            OnCutsceneEnded(videoPlayer);
            return;
        }

        videoPlayer.enabled = true;
        gameObject.SetActive(true);

        FindObjectOfType<AudioManager>()?.StopAllBGM();
        FindObjectOfType<SettingsAndExit>()?.EnableMenu(false);
        FindObjectOfType<CameraControl>()?.EnableNavigation(false);
    }

    public void SkipCutscene() => OnCutsceneEnded(videoPlayer);

    public void OnCutsceneEnded(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        gameObject.SetActive(false);

        // Or set this after Fade to Black

        FindObjectOfType<AudioManager>()?.ReplayBGM();
        FindObjectOfType<SettingsAndExit>()?.EnableMenu(true);
        FindObjectOfType<CameraControl>()?.EnableNavigation(true);

        hasVotedSkip = false;
    }


}
