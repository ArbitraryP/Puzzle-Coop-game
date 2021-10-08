using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;

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
        [SerializeField] private List<Map> mapSet = null;
        [SerializeField] private List<MapIdentity> mapButtons = null;

        // Map Select
        public Map selectedMap;

        private NetworkManagerTN room;
        private NetworkManagerTN Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerTN;
            }
        }

        private void Start()
        {
            FindObjectOfType<SettingsAndExit>()?.DisplayHowTo();
        }

        public override void OnStartAuthority()
        {
            // Removes the InputBlocker for Host who has Authority
            inputBlocker.SetActive(false);
        }

        public override void OnStartClient()
        {
            targetZoomSize = zoomOutMax;

            // Loads all maps will be used if there'll be bonus hiden levels ever.
            var mapScriptableObjects = Resources.LoadAll<Map>("ScriptableObjects/Maps");
            foreach (var loadedMap in mapScriptableObjects)
            {
                mapSet.Add(loadedMap);
            }

            // Enable/disable selectable maps based on leader's progress
            InitializeUnlockedMaps();
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

        private void InitializeUnlockedMaps()
        {
            foreach (NetworkGamePlayerTN player in Room.GamePlayers)
            {
                if (!player.isLeader) { continue; }
                foreach (int unlockedMapIndex in player.unlockedMaps)
                {
                    
                    // Will enable the maps if prerequisites of unlocked maps are met
                    MapIdentity mapButton = mapButtons.Find(i => i.map.Index == unlockedMapIndex);
                    List<int> listOfCompletedMaps = new List<int>(player.completedMaps);
                    List<int> listOfUnlockedMaps = new List<int>(player.unlockedMaps);

                    mapButton.SetMapAsSelectable(mapButton.map.IsPrerequisiteMet(listOfCompletedMaps, listOfUnlockedMaps));

                }
            }
        }

        [Command]
        public void CmdCancelSelectMap()
        {
            if (!hasAuthority) { return; } //Only Host can CancelSelectMap
            isMapSelected = false;
            selectedMap = null;
            RpcUpdateSelectMapStatus(0, 0, isMapSelected);

        }

        public void SelectMap(MapIdentity mapButton) =>
            CmdSelectMap(
                mapButton.transform.position.x,
                mapButton.transform.position.y,
                mapButton.map.Index);
        

        [Command]
        public void CmdSelectMap(float targetPositionX, float targetPositionY, int mapIndex)
        {
            if (!hasAuthority || isMapSelected) { return; } //Only Host can SelectMap and only select 1 at a time
            isMapSelected = true;
            selectedMap = mapSet.Find(i => i.Index == mapIndex);
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

        [Command]
        public void OnClickEnterSelectedMap()
        {
            if (selectedMap.Scene == "")
                return;

            string newScene = Path.GetFileNameWithoutExtension(selectedMap.Scene).ToString();
            Room.currentMap = selectedMap;
            Room.ServerChangeScene(newScene);

            Debug.Log("Change Scene to " + newScene);
        }

    }
}

