using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraMovement : MonoBehaviour
{
    // Camera Drag
    [Header("Drag")]
    public Camera cam = null;
    private Vector3 touchStart;
    public Vector3 offset;
    [Range(1, 10)]
    public float smoothFactor;
    public Vector3 direction;
    public Vector3 targetPosition;

    // Camera Zoom
    [Header("Zoom")]
    public float zoomOutMin = 2;
    public float zoomOutMax = 8;
    public float targetZoomSize = 0;
    [Range(1, 10)]
    public float smoothZoomFactor;
    private bool isZooming = false;

    // Camera Boundaries
    [Header("Boundaries")]
    [SerializeField] private SpriteRenderer mapRenderer;
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    public Vector3 deviationAmount;

    private void Awake()
    {
        targetPosition = cam.transform.position;
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;

    }


    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            //Sets destination while dragging
            direction = touchStart - cam.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = ClampCamera(cam.transform.position + direction); //Clamps the end position
        }
        

        if (Input.GetMouseButtonUp(0))
        {
            targetPosition = new Vector3(0, 0, -10);
        }
        */

        //Move Cam when it did not reach its destination yet
        if (targetPosition != null && Vector2.Distance(cam.transform.position, targetPosition) != 0)
        {
            Vector3 smoothPosition = Vector3.Lerp(cam.transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
            cam.transform.position = new Vector3(smoothPosition.x, smoothPosition.y, -10f);
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
        if (isZooming)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoomSize, smoothZoomFactor * Time.fixedDeltaTime);
            cam.transform.position = ClampCamera(cam.transform.position);

        }
        
    }

    private void Zoom(float increment)
    { 
        if (increment == 0) { return; }
        else if (increment > 0) { targetZoomSize = zoomOutMin; }
        else { targetZoomSize = zoomOutMax; }

        isZooming = true;

    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }


    public void MoveToLevel(Transform levelButton)
    {
        direction = cam.transform.position + levelButton.position;
        //targetPosition = ClampCamera(cam.transform.position + direction); //Clamps the end position
        targetPosition = levelButton.position + deviationAmount; //Clamps the end position
        
    }
}
