using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer {

    [SyncVar]
    private int heroTypeID = -1;

    [SyncVar]
    private string m_playerName = "";

    public int HeroTypeID
    {
        get
        {
            return heroTypeID;
        }

        set
        {
            heroTypeID = value;
        }
    }

    public string PlayerName
    {
        get
        {
            return m_playerName;
        }

        set
        {
            m_playerName = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (this.isLocalPlayer)
        {
            SendReadyToBeginMessage();
        }
        GetPlayerName();
    }
   
    public void OnHeroSelected(int playerIndex, HeroData heroData)
    {
        CmdOnHeroSelected(playerIndex, heroData);
    }

    [Command]
    void CmdOnHeroSelected(int playerIndex, HeroData heroData)
    {
        GameManager.Instance.OnHeroSelectedCallBack(playerIndex, heroData);
    }

    public void GetPlayerName()
    {
        if (isLocalPlayer)
        {
            LobbyManager.Instance.OnClientEnterLobby(this, slot, isLocalPlayer, LobbyManager.Instance.CurrentPlayerName);
        }
        else
        {
            if (isServer)
                RpcGetName();
            else
            {
                LobbyManager.Instance.OnClientEnterLobby(this, slot, isLocalPlayer, LobbyManager.Instance.CurrentPlayerName);
                //CmdGetName();
            }
        }
    }

    [ClientRpc]
    void RpcGetName()
    {
        CmdSendName(LobbyManager.Instance.CurrentPlayerName);
    }

    [Command]
    void CmdGetName()
    {
        RpcSendName(LobbyManager.Instance.CurrentPlayerName);
    }

    [ClientRpc]
    void RpcSendName(string name)
    {
        LobbyManager.Instance.OnClientEnterLobby(this, slot, isLocalPlayer,name);
    }

    [Command]
    void CmdSendName(string name)
    {
        LobbyManager.Instance.OnClientEnterLobby(this, slot, isLocalPlayer, name);
    }

    public override void OnClientEnterLobby()
    {
    
    
    }
}
