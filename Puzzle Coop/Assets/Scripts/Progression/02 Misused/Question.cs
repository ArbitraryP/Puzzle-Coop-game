using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Progression/Question")]
public class Question : ScriptableObject
{
    public Sprite image;
    public string text;
    public string[] choices = new string[4];
    public int answerIndex;

}
