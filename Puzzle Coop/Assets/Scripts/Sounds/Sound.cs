using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {

	public string name;
	public AudioManager.SoundNames id;

	public AudioClip clip;
	public bool isBGM = false;

	[Range(0f, 1f)]
	public float volume = .75f;
	[Range(.1f, 3f)]
	public float pitch = 1f;
	public bool loop = false;
	public bool playOnAwake = false;


	[HideInInspector]
	public AudioSource source;

}
