using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TangentNodes.Network;
using Mirror;
using TMPro;

public class UI_Receiver : MonoBehaviour
{
    [SerializeField] private TMP_Text textScreen = null;
    [SerializeField] private Slider slider = null;

    [Header("Sentences")]
    [SerializeField] private SentenceSet sentenceSet;
    [SerializeField] private bool showTextP1 = true;

    [Header("Visibility")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float visibleThreshold;
    [SerializeField] private float minimumAlpha = 0.1f;

    [Header("Static Sound")]
    [Range(0, 1f)]
    [SerializeField] private float detectionVolume = 0.5f;
    [Range(0, 1f)]
    [SerializeField] private float muteThreshold = 0.1f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] AudioSource audioSource = null;
    

    private float baseVolume;
    

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    public void InitializeUIReceiver(SentenceSet set)
    {
        sentenceSet = set;
        
        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) { continue; }
            showTextP1 = player.isLeader;
        }

        // Initialize AudioSource
        audioManager = FindObjectOfType<AudioManager>();
        

        Sound s = audioManager.FindSound(AudioManager.SoundNames.SFX_M03_RadioStatic);
        if (s == null) return;

        baseVolume = s.volume;
        audioSource.volume = baseVolume;
        audioSource.clip = s.clip;
        audioSource.loop = s.loop;
        audioSource.playOnAwake = s.playOnAwake;

    }

    public void OnSliderMove(float value)
    {
        foreach (Sentence sentence in sentenceSet.Sentences)
        {
            textScreen.text = "";

            

            // Show text within detection range
            float minRange = sentence.location - detectionRange;
            float maxRange = sentence.location + detectionRange;
            if (!TestInRange(minRange, maxRange, value))
            {
                // Play Static Sound at full volume when no detected
                //FindObjectOfType<AudioManager>()?.PlayNonRepeat(AudioManager.SoundNames.SFX_M03_RadioStatic, 1f);
                PlayRadioStaticSound();
                continue;
            }
                

            textScreen.text = ShuffleString.Shuffle(sentence.GetText(showTextP1));
            textScreen.color = new Color(0, 0, 0, minimumAlpha);

            



            // Adjust Text Opacity within Treshold
            minRange = sentence.location - visibleThreshold;
            maxRange = sentence.location + visibleThreshold;
            if (!TestInRange(minRange, maxRange, value))
            {
                // Play Static Sound slightly lower volume when within detectionRange
                // FindObjectOfType<AudioManager>()?.PlayNonRepeat(AudioManager.SoundNames.SFX_M03_RadioStatic, detectionVolume);
                PlayRadioStaticSound(detectionVolume);
                break;
            }
                

            textScreen.text = sentence.GetText(showTextP1);

            // Calculate Percentage of Alpha based on distance to center
            float alpha = 1 - (Mathf.Abs(value - sentence.location) / visibleThreshold);
            float clampAlpha = Mathf.Clamp(alpha, minimumAlpha, 1f);

            // Calculate Dynamic Sound based on distance to center
            float soundVolume = (Mathf.Abs(value - sentence.location) / visibleThreshold);

            // mutes sound when it reached the treshold
            soundVolume = soundVolume > muteThreshold ? soundVolume : 0;

            textScreen.color = new Color(0, 0, 0, clampAlpha);

            //FindObjectOfType<AudioManager>()?.PlayNonRepeat(AudioManager.SoundNames.SFX_M03_RadioStatic, soundVolume);
            PlayRadioStaticSound(soundVolume);

            break;
        }

    }


    private bool TestInRange(float min, float max, float test) =>
        (test >= min && test <= max);

    private void PlayRadioStaticSound(float dynamicVolume = 1f)
    {
        // Change Dynamic Volume Factor first before readjusting volume.
        audioSource.volume = baseVolume * audioManager.masterVolumeSFX * dynamicVolume;

        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

}
