using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroCutscenePlayer : MonoBehaviour
{
    [SerializeField] private Map currentMap = null;
    [SerializeField] private VideoPlayer videoPlayer = null;

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
        if (Input.GetKeyDown(KeyCode.Escape))
            OnCutsceneEnded(videoPlayer);
        
    }

    public void SetupCutscene(string mapName)
    {
        currentMap = Resources.Load<Map>("ScriptableObjects/Maps/" + mapName);
        if (!currentMap)
        {
            Debug.LogWarning(mapName + ": Map not Found.");
            return;
        }

        videoPlayer.clip = currentMap.cutsceneVideo;
        videoPlayer.targetCamera = FindObjectOfType<CameraControl>()?.MainCamera;

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

        FindObjectOfType<SettingsAndExit>()?.EnableMenu(false);
        FindObjectOfType<CameraControl>()?.EnableNavigation(false);
    }

    public void OnCutsceneEnded(VideoPlayer videoPlayer)
    {
        videoPlayer.enabled = false;
        gameObject.SetActive(false);

        // Or set this after Fade to Black

        FindObjectOfType<SettingsAndExit>()?.EnableMenu(true);
        FindObjectOfType<CameraControl>()?.EnableNavigation(true);

    }


}
