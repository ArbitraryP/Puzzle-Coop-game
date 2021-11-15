using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.IO;
using TMPro;

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

        [Header("Description UI")]
        [SerializeField] private TMP_Text textMapName = null;
        [SerializeField] private TMP_Text textDescription = null;

        [Header("Difficulty UI")]
        [SerializeField] private Image imageDifficultyPrefab = null;
        [SerializeField] private Transform panelDifficultyParent = null;
        [SerializeField] private List<Image> imageDifficulties = null;

        [Header("Achievement List")]
        [SerializeField] private Panel_Achievement panelAchievementPrefab = null;
        [SerializeField] private Transform panelAchievementParent = null;
        [SerializeField] private List<Panel_Achievement> panelAchievements = null;



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

                    // Check if Unlocked Map is also Completed
                    if (player.completedMaps.Contains(unlockedMapIndex))
                        mapButton.SetMapAsCompleted();
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
            RpcUpdateMapDescriptionDisplay(mapIndex);
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

        [ClientRpc]
        private void RpcUpdateMapDescriptionDisplay(int mapIndex)
        {
            // -- Code That Changes value of text description/name
            Map map = mapSet.Find(i => i.Index == mapIndex);

            textDescription.text = map.description;
            textMapName.text = map.displayName;

            // -- Code that displays difficulty
            foreach(Image imageStar in imageDifficulties)
                Destroy(imageStar.gameObject);

            imageDifficulties.Clear();

            for (int i = 1; i <= map.difficulty; i++)
                imageDifficulties.Add( GameObject.Instantiate(imageDifficultyPrefab, panelDifficultyParent) );


            // -- Ccode that displays achievements
            foreach (Panel_Achievement panelAchievement in panelAchievements)
                Destroy(panelAchievement.gameObject);

            panelAchievements.Clear();

            for (int i = 0; i < map.achievements.Count; i++)
            {
                var panelAchiv = GameObject.Instantiate(panelAchievementPrefab, panelAchievementParent);
                panelAchiv.DisplayAchievement(map.achievements[i]);
                panelAchievements.Add(panelAchiv);
            }
        }

        [Command]
        public void OnClickEnterSelectedMap()
        {
            if (selectedMap.Scene == "")
                return;

            RpcPlayUIButtonClick();

            string newScene = Path.GetFileNameWithoutExtension(selectedMap.Scene).ToString();
            Room.currentMap = selectedMap;
            Room.ServerChangeScene(newScene);

            Debug.Log("Change Scene to " + newScene);
            
        }

        [ClientRpc]
        private void RpcPlayUIButtonClick()
        {
            PlayUIMapSelectedSound();
        }

        public void PlayUIButtonClick()
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MenuButtonClick);
        }

        public void PlayUIMapSelectedSound()
        {
            FindObjectOfType<AudioManager>()?.Play(AudioManager.SoundNames.SFX_GEN_MapSelected);
        }


    }
}

