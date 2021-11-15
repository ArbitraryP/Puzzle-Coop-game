using UnityEngine;

public class BookSounds : MonoBehaviour
{
    public void PlaySoundBookClose()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M01_BookClose);
    }

    public void PlaySoundPageTurn()
    {
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M01_PageTurn);
    }
}
