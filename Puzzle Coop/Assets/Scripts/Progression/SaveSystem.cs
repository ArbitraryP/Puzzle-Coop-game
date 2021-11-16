using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // Location where will be saved
    private static string saveLocation = Application.persistentDataPath + "/data";

    public static void SavePlayer(PlayerProgress player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = saveLocation + "/player-" + player.playerSteamId + ".tns";

        // Add Function that will double check for saves that is not his.

        try
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            PlayerData data = new PlayerData(player);

            // Might add Finaly if stream close not working properly
        
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Save Completed");
        }
        catch(IOException ex)
        {
            Debug.LogWarning("Save Failed: " + ex.Message);
        }
        
    }

    // Loads a save file based on playerSteamID
    public static PlayerData LoadPlayer(ulong playerSteamId)
    {
        string path = saveLocation + "/player-" + playerSteamId + ".tns";

        try
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;

                stream.Close();
                Debug.Log("Load Completed");
                return data;
            }
            else
            {
                Debug.LogWarning("Load Failed: No Save file for this player found in " + path);
                return null;
            }

        }
        catch(IOException ex)
        {
            Debug.LogWarning("Load Failed: " + ex.Message);
            return null;
        }

    }

}
