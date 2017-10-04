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
        LobbyManager.Instance.OnClientEnterLobby(this, slot, isLocalPlayer);
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

    public override void OnClientEnterLobby()
    {
    
    
    }
}
