using UnityEngine;

public class FadeToBlackMapF : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private GameObject hallwayObject = null;

    public void OnFadeOut()
    {
        // Jump Camera
        // Reset Terminal

        FindObjectOfType<UI_TerminalScreen>()?.ResetProgress();
        FindObjectOfType<CameraControl>()?.JumpToFloor(0);
        hallwayObject?.SetActive(false);

        animator.SetTrigger("FadeIn");
    }

    public void OnFadeIn() => gameObject.SetActive(false);

}
