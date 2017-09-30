using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private NetworkManagerCustom n_networkManager;
    private NDiscovery n_networkDiscovery;

    [SerializeField]
    private GameObject m_commonAreaGO;
    [SerializeField]
    private GameObject m_homeAreaGO;
    [SerializeField]
    private GameObject m_playAreaGO;

    [Header("LAN")]
    [SerializeField]
    private GameObject m_playMainAreaGO;
    [SerializeField]
    private GameObject m_playLANAreaGO;
    [SerializeField]
    private GameObject m_LANSelectionBtnsGO;
    [SerializeField]
    private GameObject m_LANRoomsContentGO;
    [SerializeField]
    private GameObject m_LANRoomPlayerPrefab;
    [SerializeField]
    private InputField m_LANRoomNameInput;


    [SerializeField]
    private GameObject m_playOnlineArea;
    [SerializeField]
    private GameObject m_settingsAreaGO;
    [SerializeField]
    private GameObject m_storeAreaGO;

    private LobbyScreen m_currentLobbyScreen = LobbyScreen.None;
    private PlayScreen m_currenPlayScreen = PlayScreen.None;
    private Map m_currentMap = Map.GreenJungle;

    private List<string> m_hostedRoomsList = new List<string>();

    private void Awake()
    {
        Instance = this;
        GameObject networkManagerGO = GameObject.FindGameObjectWithTag("NetworkManager");
        n_networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerCustom>();
    }

    void Start()
    {
        NDiscovery.Instance.onServerDetected += OnReceiveBraodcast;
    }

    void OnDestroy()
    {
        NDiscovery.Instance.onServerDetected -= OnReceiveBraodcast;
    }

    #region ButtonHandlers 
    public void OnStoreBtnClick()
    {
        ChangeMainScreen(LobbyScreen.Store);
    }

    public void OnPlayBtnClick()
    {
        ChangeMainScreen(LobbyScreen.Play);
        ChangePlayScreen(PlayScreen.Main);
    }
    public void OnPlayBackBtnClick()
    {
        ChangeMainScreen(LobbyScreen.Home);
        NDiscovery.Instance.StopBroadcasting();
        n_networkManager.StopHost();
    }

    public void OnSettingsBtnClick()
    {
        ChangeMainScreen(LobbyScreen.Settings);
    }

    public void OnLANBtnClick()
    {
        ChangePlayScreen(PlayScreen.LAN);
        NDiscovery.Instance.Initialize();
        FindGames();
    }

    public void OnLANHostBtnClick()
    {
        NDiscovery.Instance.StopBroadcasting();
        HostGame();
    }

    public void OnLANJoinBtnClick()
    {
        FindGames();
    }

    public void OnLANRefreshBtnClicked()
    {
        ClearRoomContent();
    }

    #endregion

    #region Networks
    void HostGame()
    {
        if (m_LANRoomNameInput.text.Length > 1)
        {
            NDiscovery.Instance.SetBroadcastData(m_LANRoomNameInput.text);
        }
        NDiscovery.Instance.StartBroadcasting();
        n_networkManager.StartHost();
    }

    void FindGames()
    {
        NDiscovery.Instance.ReceiveBraodcast();
    }

    public void JoinGame(string address)
    {
        n_networkManager.networkAddress = address;
        n_networkManager.StartClient();
        NDiscovery.Instance.StopBroadcasting();
    }

    public void OnReceiveBraodcast(string fromIp, string data)
    {
        Debug.Log("Msg from : " + fromIp + ", Data : " + data);
        UpdateRoomsList(fromIp + "," + data);
    }

    #endregion

    #region UI
    void ChangeMainScreen(LobbyScreen screenToMoveTo)
    {
        bool activate = false;
        for (int i = 0; i < 2; i++)
        {
            if (m_currentLobbyScreen != LobbyScreen.None)
            {
                switch (m_currentLobbyScreen)
                {
                    case LobbyScreen.Home:
                        m_homeAreaGO.SetActive(activate);
                        m_commonAreaGO.SetActive(activate);
                        break;
                    case LobbyScreen.Play:
                        m_playAreaGO.SetActive(activate);
                        m_commonAreaGO.SetActive(!activate);
                        break;
                    case LobbyScreen.Settings:
                        m_settingsAreaGO.SetActive(activate);
                        break;
                    case LobbyScreen.Store:
                        m_storeAreaGO.SetActive(activate);
                        break;
                }
            }
            activate = true;
            m_currentLobbyScreen = screenToMoveTo;
        }
    }

    void ChangePlayScreen(PlayScreen screenToMoveTo)
    {
        bool activate = false;
        for (int i = 0; i < 2; i++)
        {
            if (m_currenPlayScreen != PlayScreen.None)
            {
                switch (m_currenPlayScreen)
                {
                    case PlayScreen.Main:
                        m_playMainAreaGO.SetActive(activate);
                        break;
                    case PlayScreen.LAN:
                        m_playLANAreaGO.SetActive(activate);
                        break;
                    case PlayScreen.Online:
                        m_playOnlineArea.SetActive(activate);
                        break;
                }
            }
            activate = true;
            m_currenPlayScreen = screenToMoveTo;
        }
    }

    void UpdateRoomsList(string newPlayerData)
    {
        if(!m_hostedRoomsList.Contains(newPlayerData))
        {
            m_hostedRoomsList.Add(newPlayerData);
        }
        for (int i = 0; i < m_hostedRoomsList.Count; i++)
        {
            if(m_LANRoomsContentGO.transform.childCount >= i + 1)
            {
                Debug.Log("AlreadyExist");
            }
            else
            {
                GameObject tempLobbyPlayerGo = Instantiate(m_LANRoomPlayerPrefab, m_LANRoomsContentGO.transform);
                tempLobbyPlayerGo.GetComponent<LobbyRoomPlayer>().Init(newPlayerData);
            }
        }
    }

    void ClearRoomContent()
    {
        foreach (Transform item in m_LANRoomsContentGO.transform)
        {
            Destroy(item.gameObject);
        }
        m_hostedRoomsList.Clear();
    }

    #endregion
}

public enum LobbyScreen { None, Home, Play, Settings, Store }
public enum PlayScreen { None, Main, LAN, Online }
public enum Map { None, GreenJungle }
