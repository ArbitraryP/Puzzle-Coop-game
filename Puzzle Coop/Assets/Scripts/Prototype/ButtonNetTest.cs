using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class ButtonNetTest : NetworkBehaviour
{
    [SerializeField] private TMP_Text buttonText = null;

    private void Start()
    {
        buttonText = GameObject.FindWithTag("TextTest")?.GetComponent<TMP_Text>();
    }

    [Command]
    public void CmdChangeButtonText(string text)
    {
        RpcChangeButtonText(text);
    }

    [ClientRpc]
    private void RpcChangeButtonText(string text)
    {
        buttonText.text = text;
    }


}
