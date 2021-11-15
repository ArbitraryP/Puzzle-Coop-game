using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Terminal Message", menuName = "Progression/TerminalMessage")]
public class TerminalMessage : ScriptableObject
{
    [TextArea]
    public List<string> messages = null;
}
