using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameUIManager : MonoBehaviour
{

    public void OnQuitBtnCicked()
    {
        if (NDiscovery.Instance.running)
            NDiscovery.Instance.StopBroadcasting();

        NetworkManager.singleton.StopHost();
    }
}
