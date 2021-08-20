using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class QuestionSerializer
{
    public static void WriteArmor(this NetworkWriter writer, Question armor)
    {
        // no need to serialize the data, just the name of the armor
        writer.WriteString(armor.name);
    }

    public static Question ReadArmor(this NetworkReader reader)
    {
        // load the same armor by name.  The data will come from the asset in Resources folder
        return (Question)Resources.Load(reader.ReadString());
    }
}
