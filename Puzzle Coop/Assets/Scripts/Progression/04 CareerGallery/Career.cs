using UnityEngine;

[CreateAssetMenu(fileName = "New Career", menuName = "Progression/Career")]
public class Career : ScriptableObject
{
    public Sprite image;
    public string careerName;
    [Range(0,9)]
    public int[] code = new int[4];
    
    public string GetCode()
    {
        string fullcode = "";
        for (int i = 0; i < code.Length; i++)
        {
            fullcode += code[i].ToString();
        }

        return fullcode;
    }
}