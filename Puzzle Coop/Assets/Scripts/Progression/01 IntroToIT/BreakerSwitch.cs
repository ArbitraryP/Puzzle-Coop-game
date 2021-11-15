using UnityEngine;
using UnityEngine.UI;

public class BreakerSwitch : MonoBehaviour, IClickable
{
    [SerializeField] private MapObjectManager_L objectManager_L = null;
    [SerializeField] private Transform BreakerHandle = null;
    [SerializeField] private Transform ObjectRef_On = null;
    [SerializeField] private Transform ObjectRef_Off = null;
    [SerializeField] private Button[] breakerButtons = null;

    [Range(0.1f, 10f)]
    public float smoothPanFactor;
    private Vector3 targetPosition;

    private bool isSwitchedOn = false;
    private bool targetSwitchState = false;
    private bool isHandleFullySwitched = true;


    private void Start()
    {
        targetPosition = ObjectRef_Off.position;
    }

    private void Update()
    {
        // Move Handle where theres new coordinates
        if (Vector2.Distance(BreakerHandle.position, targetPosition) > 0.01)
        {
            Vector3 smoothPosition = Vector3.Lerp(
                BreakerHandle.position,
                targetPosition,
                smoothPanFactor * Time.fixedDeltaTime);

            BreakerHandle.position = new Vector3(smoothPosition.x, smoothPosition.y, 0);
        }
        else if (!isHandleFullySwitched)
        {
            isHandleFullySwitched = true;

            isSwitchedOn = targetSwitchState;

            if(isSwitchedOn)
                isCorrectCombination();
        }

        LockButtons();

    }

    private void isCorrectCombination()
    {
        if (!objectManager_L.M01_IsCombinationCorrect())
        {
            ChangeSwitchState(false);
        }

    }

    public void ChangeSwitchState(bool changeToOn)
    {
        if (changeToOn == isSwitchedOn)
            return;

        if (changeToOn)
        {
            targetPosition = ObjectRef_On.position;
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M01_LeverOn);
        }
        else
        {
            targetPosition = ObjectRef_Off.position;
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_M01_LeverOff);
        }
            
        targetSwitchState = changeToOn;
        isHandleFullySwitched = false;
    }

    private void LockButtons()
    {
        if (targetSwitchState || isSwitchedOn)
            foreach (Button breakerButton in breakerButtons)
                breakerButton.interactable = false;

        else
            foreach (Button breakerButton in breakerButtons)
                breakerButton.interactable = true;
    }

    public void Click()
    {
        ChangeSwitchState(true);
    }
    
}
