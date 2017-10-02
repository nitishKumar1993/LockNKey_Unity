using System;
using UnityEngine.Networking;

public class NDiscovery : NetworkDiscovery
{
    public static NDiscovery Instance;

    NetworkManagerCustom m_networkManager;

    public Action<string, string> onServerDetected;

    void OnServerDetected(string fromAddress, string data)
    {
        if (onServerDetected != null)
        {
            onServerDetected.Invoke(fromAddress, data);
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_networkManager = this.GetComponent<NetworkManagerCustom>();
        }
    }

    public bool InitializeNetworkDiscovery()
    {
        NetworkTransport.Init();
        return Initialize();
    }

    public void StartBroadcasting()
    {
        StartAsServer();
    }

    public void ReceiveBraodcast()
    {
        StartAsClient();
    }

    public void StopBroadcasting()
    {
        if (m_networkManager.isNetworkActive)
            m_networkManager.StopHost();

        if (running)
            StopBroadcast();
    }

    public void SetBroadcastData(string broadcastPayload)
    {
        broadcastData = broadcastPayload;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        OnServerDetected(fromAddress.Split(':')[3], data);
    }
}