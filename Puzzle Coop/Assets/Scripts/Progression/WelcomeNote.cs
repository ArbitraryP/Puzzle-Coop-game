using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomeNote : MonoBehaviour
{
    [SerializeField] private TMP_Text textAll = null;
    [SerializeField] private Map map = null;



    public void SetMap(string newMap)
    {
        List<Map> allMaps = new List<Map>( Resources.LoadAll<Map>("ScriptableObjects/Maps"));
        map = allMaps.Find(i => i.name == newMap);
    }
    

    public void SetText(bool isHost)
    {
        if (!map) return;

        string stringPlayer = isHost ? map.stringP1 : map.stringP2;
        string stringGeneral = "<size=35>" + map.stringGeneral + "</size>";
        textAll.text = "<b>Jacquard's Notes:</b>\n" + stringGeneral + "\n" + stringPlayer;
    }


}
