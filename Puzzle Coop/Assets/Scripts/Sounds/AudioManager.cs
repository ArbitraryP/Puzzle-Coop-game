using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	[Range(0,1f)]
	public float masterVolumeSFX = 0.8f;
	[Range(0, 1f)]
	public float masterVolumeBGM = 0.8f;

	private float tempVolumeBGM = 0.8f;

	public Sound[] sounds;

	public enum SoundNames
	{
		SFX_GEN_MenuButtonClick,
		SFX_GEN_MouseHover,
		SFX_GEN_MapSelected,

		SFX_MAP_AccessGrant,
		SFX_MAP_DoorUnlocked,
		SFX_MAP_DoorOpened,
		SFX_MAP_DoorLockInteract,
		SFX_MAP_StairSteps,
		SFX_MAP_Envelope,
		SFX_MAP_Paper,
		SFX_MAP_KeyTone,
		SFX_MAP_BreakerButtonClick,

		SFX_M00_CorrectCode,
		SFX_M00_PlugIn,
		SFX_M00_PlugOut,
		SFX_M00_PlugCollide,
		SFX_M00_WrongCode,

		SFX_M01_BookClose,
		SFX_M01_BookOpen,
		SFX_M01_LeverOff,
		SFX_M01_LeverOn,
		SFX_M01_PageTurn,
		SFX_M01_NodePickUp,
		SFX_M01_NodeDrop,

		SFX_M02_QuizButton,
		SFX_M02_Complete,
		SFX_M02_CorrectAnswer,
		SFX_M02_IncorrectAnswer,

		SFX_M03_RadioStatic,
		SFX_M03_CalendarClockButtonClick,
		SFX_M03_CalendarClockInteract,
		SFX_M03_CalendarClockCorrect,

		SFX_M04_TicketDispense,

		SFX_M09_TerminalTyping,
		SFX_M09_TerminalError,
		SFX_M09_HallwayLightsOn,
		SFX_M09_HallwayEnter,

		BGM_MAP_Ambient,
		BGM_SEL_Ambient,
		BGM_MainMenu,

		CUTSCENE_AudioSource
	};



	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.playOnAwake = s.playOnAwake;

		}

		tempVolumeBGM = masterVolumeBGM;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void Update()
	{
		// Update BGM volume every frame so it is not resource heavy. (Maybe)
		if (tempVolumeBGM == masterVolumeBGM) return;
		ApplySoundSettings();
		tempVolumeBGM = masterVolumeBGM;
	}

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		ReplayBGM();
		StopAllLoopingSFX();
	}

	public void ReplayBGM()
    {
		Scene scene = SceneManager.GetActiveScene();

		StopAllBGM();
		if (scene.name == "Scene_Lobby")
			PlayNonRepeat(SoundNames.BGM_MainMenu);

		else if (scene.name == "Scene_Map_Select")
			PlayNonRepeat(SoundNames.BGM_SEL_Ambient);

		else if (scene.name.StartsWith("Scene_Map_") && scene.name != "Scene_Map_Select")
			PlayNonRepeat(SoundNames.BGM_MAP_Ambient);
	}


	// Will only be used if there is something to be called manually without using enum
	public void Play(string name)
	{
		SoundNames find;
        if (Enum.TryParse<SoundNames>(name, out find))
        {
			Play(find);
			return;
		}
		Debug.LogWarning("Sound: " + name + " not found!");
	}

	public void Play(SoundNames name)
	{
		Sound s = FindSound(name);

		ApplySoundSettings();
		s.source.Play();
	}

	/// <summary>
	/// A 2nd Play method to not play the audio if it is already played
	/// This method will be used if the audioclip should not be played multiple times simultaneously
	/// </summary>
	/// <param name="name">The id of the Sound</param>
	/// <param name="dynamicVolume">This will play the sound with a volume of 0 to 1f</param>
	public void PlayNonRepeat(SoundNames name, float dynamicVolume = 1f)
	{

		Sound s = FindSound(name);
		if (s == null) return;

		// Change Dynamic Volume Factor first before readjusting volume.
		s.dynamicVolumeFactor = dynamicVolume;
		ApplySoundSettings();

		if (s.source.isPlaying) return;
		s.source.Play();

	}

	public void Stop(SoundNames name)
	{
		Sound s = FindSound(name);
		if (s == null || !s.source.isPlaying) { return; }
		s.source.Stop();
	}

	public void StopAllBGM()
    {
		foreach(Sound s in Array.FindAll<Sound>(sounds, sound => sound.isBGM))
			s.source.Stop();
    }

	public void StopAllLoopingSFX()
	{
		foreach (Sound s in Array.FindAll<Sound>(sounds, sound => !sound.isBGM && sound.loop))
			s.source.Stop();
	}

	public Sound FindSound(SoundNames name)
	{
		Sound s = Array.Find(sounds, sound => sound.id == name.ToString());
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
		}
		return s;
	}

	public AudioClip GetAudioClip(SoundNames name)
    {
		Sound s = FindSound(name);
		if (s == null) return null;

		return s.clip;
	}

	public void ApplySoundSettings()
	{
		foreach (Sound s in sounds)
		{
			if (s.isBGM)
				s.source.volume = s.volume * s.dynamicVolumeFactor * masterVolumeBGM;
			else
				s.source.volume = s.volume * s.dynamicVolumeFactor * masterVolumeSFX;
		}
	}

	public AudioSource GetCutsceneAudioSource()
    {
		Sound s = FindSound(SoundNames.CUTSCENE_AudioSource);
		if (s == null)
			return null;
		return s.source;
	}

}
