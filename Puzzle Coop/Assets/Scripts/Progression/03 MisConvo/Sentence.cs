using UnityEngine;

[CreateAssetMenu(fileName = "New Sentence", menuName = "Progression/Sentence")]
public class Sentence : ScriptableObject
{
    [Range(0f,1f)]
    public float location;
    [TextArea]
    [SerializeField] private string firstText;
    [TextArea]
    [SerializeField] private string secondText;

    public string GetText(bool showTextP1)
    {
        if (showTextP1)
            return firstText;
        else
            return secondText;
    }
}
