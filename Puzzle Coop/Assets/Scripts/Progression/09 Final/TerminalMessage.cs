using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Terminal Message", menuName = "Progression/TerminalMessage")]
public class TerminalMessage : ScriptableObject
{
    [TextArea(8,25)]
    public List<string> messages = null;
}
