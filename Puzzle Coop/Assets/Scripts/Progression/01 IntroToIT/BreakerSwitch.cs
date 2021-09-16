using UnityEngine;

public class BreakerSwitch : MonoBehaviour, IClickable
{
    [SerializeField] private MapObjectManager_L objectManager_L = null;
    [SerializeField] private Transform BreakerHandle = null;
    [SerializeField] private Transform ObjectRef_On = null;
    [SerializeField] private Transform ObjectRef_Off = null;

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
            targetPosition = ObjectRef_On.position;
        
        else
            targetPosition = ObjectRef_Off.position;

        targetSwitchState = changeToOn;
        isHandleFullySwitched = false;
    }

    public void Click()
    {
        ChangeSwitchState(true);
    }
    
}
