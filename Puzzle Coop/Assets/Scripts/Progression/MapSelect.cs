using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TangentNodes.Network
{
    public class MapSelect : NetworkBehaviour
    {
        [SerializeField] private GameObject canvasOverlay = null;
        [SerializeField] private GameObject buttonConfirm = null;
        [SerializeField] private GameObject buttonCancel = null;
        [SerializeField] private GameObject inputBlocker = null;
        [SerializeField] private Camera mainCamera = null;
        private bool isMapSelected = false;

        [Header("Camera Pan")]
        [Range(0.1f, 10f)]
        public float smoothPanFactor;
        [SerializeField] private Vector3 direction;
        private Vector3 targetPosition;
        [SerializeField] private Vector3 deviationAmount;

        [Header("Camera Zoom")]
        [SerializeField] private float zoomOutMin;
        [SerializeField] private float zoomOutMax;
        [SerializeField] private float targetZoomSize;
        [Range(0.1f, 10f)]
        public float smoothZoomFactor;

        [Header("Maps")]
        [SerializeField] private List<MapIdentity> mapButtons = null;

        // Map Select
        public int indexOfMapSelected = -1;

        private NetworkManagerTN room;
        private NetworkManagerTN Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerTN;
            }
        }

        
        public override void OnStartAuthority()
        {
            // Removes the InputBlocker for Host who has Authority
            inputBlocker.SetActive(false);
        }

        public override void OnStartClient()
        {
            targetZoomSize = zoomOutMax;

            // Enable/disable selectable maps based on leader's progress
            foreach (NetworkGamePlayerTN player in Room.GamePlayers)
            {
                if (!player.isLeader) { continue; }
                foreach (int mapIndex in player.unlockedMaps)
                {
                    int index = mapButtons.FindIndex(i => i.MapIndexNumber == mapIndex);
                    mapButtons[index].SetMapAsSelectable(true);
                    
                }
            }

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

            // Zoom Camera if there's new targetZoomSize
            if (mainCamera.orthographicSize != targetZoomSize)
            {
                mainCamera.orthographicSize = Mathf.Lerp(
                    mainCamera.orthographicSize,
                    targetZoomSize,
                    smoothZoomFactor * Time.fixedDeltaTime);

            }

            // Back if a Map is Selected and hasAuthority
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && isMapSelected && hasAuthority)
            {
                CmdCancelSelectMap();
            }


        }

        
        [Command]
        public void CmdCancelSelectMap()
        {
            if (!hasAuthority) { return; } //Only Host can CancelSelectMap
            isMapSelected = false;
            indexOfMapSelected = -1;
            RpcUpdateSelectMapStatus(0, 0, isMapSelected);

        }

        public void SelectMap(MapIdentity mapButton) =>
            CmdSelectMap(mapButton.transform.position.x, mapButton.transform.position.y, mapButton.MapIndexNumber);
        

        [Command]
        public void CmdSelectMap(float targetPositionX, float targetPositionY, int mapIndex)
        {
            if (!hasAuthority || isMapSelected) { return; } //Only Host can SelectMap and only select 1 at a time
            isMapSelected = true;
            indexOfMapSelected = mapIndex;
            RpcUpdateSelectMapStatus(targetPositionX, targetPositionY, isMapSelected);
        }


        [ClientRpc]
        private void RpcUpdateSelectMapStatus(float targetPositionX, float targetPositionY, bool isMapSelected)
        {
            // Sets the Camera
            targetZoomSize = isMapSelected ? zoomOutMin : zoomOutMax; // Zooms in or out
            targetPosition = isMapSelected ? // Sets to center of scene if isMapSelected is false
                new Vector3(targetPositionX, targetPositionY, -10f) + deviationAmount :
                new Vector3(0, 0, -10f);

            

            // Shows/Hides Description and Confirm Button

            canvasOverlay.SetActive(isMapSelected);
            if (!hasAuthority) { return; }
            buttonCancel.SetActive(isMapSelected);
            buttonConfirm.SetActive(isMapSelected);

            
        }



    }
}
