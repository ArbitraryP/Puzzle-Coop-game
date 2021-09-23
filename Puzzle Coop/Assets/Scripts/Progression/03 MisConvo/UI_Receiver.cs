using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TangentNodes.Network;
using Mirror;
using TMPro;

public class UI_Receiver : MonoBehaviour
{
    [SerializeField] private TMP_Text textScreen = null;
    [SerializeField] private Slider slider = null;

    [Header("Sentences")]
    [SerializeField] private SentenceSet sentenceSet;
    [SerializeField] private bool showTextP1 = true;

    [Header("Visibility")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float visibleThreshold;
    [SerializeField] private float minimumAlpha = 0.1f;

    private NetworkManagerTN room;
    private NetworkManagerTN Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTN;
        }
    }

    public void InitializeSentences(int currentMapIndex)
    {
        var AllSets = Resources.LoadAll<SentenceSet>("ScriptableObjects/Sentences");

        foreach (var set in AllSets)
        {
            if (set.AssociateMap.Index != currentMapIndex) continue;
            sentenceSet = set;
        }

        foreach (NetworkGamePlayerTN player in Room.GamePlayers)
        {
            if (!player.hasAuthority) { continue; }
            showTextP1 = player.isLeader;
        }

    }

    public void OnSliderMove(float value)
    {
        foreach (Sentence sentence in sentenceSet.Sentences)
        {
            textScreen.text = ""; 

            // Show text within detection range
            float minRange = sentence.location - detectionRange;
            float maxRange = sentence.location + detectionRange;

            if (!TestInRange(minRange, maxRange, value))
                continue;

            textScreen.text = ShuffleString.Shuffle(sentence.GetText(showTextP1));
            textScreen.color = new Color(0, 0, 0, minimumAlpha);

            // Adjust Text Opacity within Treshold
            minRange = sentence.location - visibleThreshold;
            maxRange = sentence.location + visibleThreshold;

            if (!TestInRange(minRange, maxRange, value))
                break;

            textScreen.text = sentence.GetText(showTextP1);

            // Calculate Percentage of Alpha based on distance to center
            float alpha = 1 - (Mathf.Abs(value - sentence.location) / visibleThreshold);
            float clampAlpha = Mathf.Clamp(alpha, minimumAlpha, 1f);

            textScreen.color = new Color(0, 0, 0, clampAlpha);
            break;
        }

    }


    private bool TestInRange(float min, float max, float test) =>
        (test >= min && test <= max);
}
