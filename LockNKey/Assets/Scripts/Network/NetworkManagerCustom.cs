using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkLobbyManager {

    [SerializeField]
    private LobbyManager m_lobbyManager;

    // Use this for initialization
    void Start () {
    }

    public void StartHosting()
    {
        if (!Network.isServer)
        {
            StartHost();
        }
        else Debug.Log("Already hosting");
    }

    public void LoadGameScene()
    {
        ServerChangeScene("Game");
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        //Debug.Log("OnLobbyClientConnect connectionId : " + conn.connectionId);
        //  LobbyManager.Instance.OnClientEnterLobby(conn.connectionId);
    }

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        // Debug.Log("OnLobbyServerConnect connectionId : " + conn.connectionId);
        //  LobbyManager.Instance.OnClientEnterLobby(conn.connectionId);
    }

    public override void OnLobbyServerPlayersReady()
    {
        LobbyManager.Instance.OnAllPlayerReady();
    }

    public override void OnStartHost()
    {
        Debug.Log("OnStartHost");
    }

    public override void OnLobbyStartHost()
    {
        Debug.Log("OnLobbyStartHost");
    }

    public void SpawnGO(GameObject GO)
    {
        NetworkServer.Spawn(GO);
    }
}
