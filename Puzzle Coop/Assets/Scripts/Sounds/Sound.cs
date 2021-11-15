using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {

	public string name;
	public string id;

	public AudioClip clip;
	public bool isBGM = false;

	// This is the default volume value
	[Range(0f, 1f)]
	public float volume = .75f;
	
	// This will adjust the volume base on runtime
	[HideInInspector]
	public float dynamicVolumeFactor = 1f;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	public bool loop = false;
	public bool playOnAwake = false;


	[HideInInspector]
	public AudioSource source;

}
