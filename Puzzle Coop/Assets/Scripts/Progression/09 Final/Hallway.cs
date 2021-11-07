using UnityEngine;

public class Hallway : MonoBehaviour, IClickable
{
    [SerializeField] private FadeToBlackMapF fade = null;

    public void Click()
    {
        fade.gameObject.SetActive(true);
        FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M09_HallwayEnter);
    }
}
