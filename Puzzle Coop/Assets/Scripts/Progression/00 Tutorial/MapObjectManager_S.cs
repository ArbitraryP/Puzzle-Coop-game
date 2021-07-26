using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TangentNodes.Network;

/// <summary>
/// This object is owned by players through network
/// Searches for LocalMapObjectManager to Initialize based on Leader status
/// 
/// This class will check if players are ready in the Scene and
/// communicates wth Local MapObjectManager to Initialize values for respective players
/// </summary>
public class MapObjectManager_S : NetworkBehaviour
{

    public override void OnStartClient()
    {

    }



}
