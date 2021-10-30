using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public bool isHostPlayer = true;

    [Header("Floors")]
    [Range(0, 2)]
    public int numberOfExtraFloors = 2;
    [Range(0, 2)]
    public int currentFloor = 1;

    [Header("Buttons")]
    public Button button_up = null;
    public Button button_down = null;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera = null;
    public Transform[] camPoints_P1;
    public Transform[] camPoints_P2;

    public Camera MainCamera => mainCamera;

    [Header("Camera Pan")]
    [Range(0.1f, 10f)]
    public float smoothPanFactor;
    [SerializeField] private Vector3 direction;
    private Vector3 targetPosition;

    private void Awake()
    {
        CheckNavigatableFloors();
        ResetCamera();
    }

    private void Update()
    {
        // Move/Pan Camera where theres new coordinates
        if (targetPosition != null && Vector2.Distance(mainCamera.transform.position, targetPosition) > 0)
        {
            Vector3 smoothPosition = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                smoothPanFactor * Time.fixedDeltaTime);

            mainCamera.transform.position = new Vector3(smoothPosition.x, smoothPosition.y, -10f);
        }
    }

    public void EnableNavigation(bool active)
    {
        button_up.gameObject.SetActive(active);
        button_down.gameObject.SetActive(active);
    }

    public void JumpToFloor(int floorNumber)
    {
        currentFloor = floorNumber;

        if (floorNumber < 0 || floorNumber >= numberOfExtraFloors) return;

        if (isHostPlayer)
        {
            mainCamera.transform.position = camPoints_P1[floorNumber].position;
            targetPosition = camPoints_P1[floorNumber].position;
        }
        else
        {
            mainCamera.transform.position = camPoints_P2[floorNumber].position;
            targetPosition = camPoints_P2[floorNumber].position;
        }

        CheckNavigatableFloors();
    }

    public void ResetCamera()
    {
        int camIndex = 0;
        if (numberOfExtraFloors >= 2)
            camIndex = 1;

        if (isHostPlayer)
        {
            mainCamera.transform.position = camPoints_P1[camIndex].position;
            targetPosition = camPoints_P1[camIndex].position;
        }
        else
        {
            mainCamera.transform.position = camPoints_P2[camIndex].position;
            targetPosition = camPoints_P2[camIndex].position;
        }

        CheckNavigatableFloors();
    }

    public void OnClickButtonUp()
    {
        if (currentFloor >= numberOfExtraFloors)
            return;

        currentFloor += 1;

        if (isHostPlayer)
            targetPosition = camPoints_P1[currentFloor].position;

        else
            targetPosition = camPoints_P2[currentFloor].position;

        CheckNavigatableFloors();
    }

    public void OnClickButtonDown()
    {
        if (currentFloor <= 0)
            return;

        currentFloor -= 1;

        if (isHostPlayer)
            targetPosition = camPoints_P1[currentFloor].position;

        else
            targetPosition = camPoints_P2[currentFloor].position;

        CheckNavigatableFloors();
    }

    private void CheckNavigatableFloors()
    {
        button_up.interactable = true;
        button_down.interactable = true;

        if (currentFloor <= 0)
            button_down.interactable = false;

        else if (currentFloor >= numberOfExtraFloors)
            button_up.interactable = false;

    }
}
