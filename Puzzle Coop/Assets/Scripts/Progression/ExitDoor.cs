using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour, IClickable
{
    
    public void Click()
    {
        Debug.Log("Level Completed for this player");
        // tell server that it is ready to exit
        // server determines if is ready to return to MapHub and Unlock next maps
    }

}
