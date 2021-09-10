using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TangentNodes.Network;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class MapCompleteMessage : NetworkBehaviour
{
    private MapObjectManager_S serverObjectManager = null;

    [SerializeField] private TMP_Text textTitle = null;
    [SerializeField] private TMP_Text textDescription = null;
    [SerializeField] private TMP_Text textButton = null;
    [SerializeField] private Button buttonContinue = null;
    [SerializeField] private GameObject uiCanvas = null;
    private Map completedMap = null;

    public void ShowMessage(bool state)
    {
        uiCanvas.SetActive(state);
    }

    public void SetUpMapMessage(string mapName)
    {
        completedMap = Resources.Load<Map>("ScriptableObjects/Maps/" + mapName);

        if (completedMap.Index == 9) // return if Final Map
            return;

        /*
         * Add code to customize Message if ever needed to show
         * - map name
         * - unlocked achievements
         * - unlocked maps
         * 
        */

    }

    public void OnClickContinue()
    {
        buttonContinue.interactable = false;
        textButton.text = "Waiting for partner...";
        CmdOnClickContinue();
    }

    [Command(requiresAuthority = false)]
    private void CmdOnClickContinue()
    {
        serverObjectManager = FindObjectOfType<MapObjectManager_S>();
        serverObjectManager?.CmdExitDoor(1);
    }


}
