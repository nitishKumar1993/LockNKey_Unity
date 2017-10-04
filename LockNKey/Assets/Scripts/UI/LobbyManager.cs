using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private NetworkManagerCustom m_networkManager;
    private NDiscovery m_networkDiscovery;

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
    private GameObject m_playerLobbyArea;
    [SerializeField]
    private GameObject m_playerLobbyStartGameBtn;
    [SerializeField]
    private GameObject m_playerLobbyEnterHeroeSelectionBtn;
    [SerializeField]
    private GameObject m_playerLobbyContainerGO;
    [SerializeField]
    private Text m_playerLobbyStatusText;

    [SerializeField]
    private GameObject m_heroSelectionArea;
    [SerializeField]
    private GameObject m_heroSelectionContentGO;
    [SerializeField]
    private Text m_heroSelectionStatusText;
    [SerializeField]
    private GameObject m_heroSelectionSelectedHeroGO;
    [SerializeField]
    private GameObject m_heroSelectionConfirmBtnGO;
    [SerializeField]
    private GameObject m_heroSelectionEnterGameBtnGO;
    [SerializeField]
    private GameObject m_playOnlineArea;
    [SerializeField]
    private GameObject m_settingsAreaGO;
    [SerializeField]
    private GameObject m_storeAreaGO;
    [SerializeField]
    private GameObject m_nameAreaGO;
    [SerializeField]
    private GameObject m_nameInputGO;

    private LobbyScreen m_currentLobbyScreen = LobbyScreen.None;
    private PlayScreen m_currenPlayScreen = PlayScreen.None;
    private Map m_currentMap = Map.GreenJungle;

    private List<string> m_hostedRoomsList = new List<string>();

    private HeroData m_selectedHeroData = new HeroData();

    private string m_nameSaveKey = "PlayerName";
    private string m_welcomeMsg = "Welcome back ";
    private string m_clientChaserSelectionMsg = "Wait...Host selecting Chaser";
    private string m_hostChaserSelectionMsg = "Host select Chaser";

    private string m_heroSelectionChaserMsg = "Choose Chaser";
    private string m_heroSelectionRunnerMsg = "Choose Runner";

    private string currentPlayerName = "Player";

    private void Awake()
    {
        Instance = this;
        NetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerCustom>();
    }

    void Start()
    {
        CheckAndShowNameInput();
        NDiscovery.Instance.onServerDetected += OnReceiveBraodcast;
    }

    public bool IsRunningAsServer
    {
        get { return NetworkServer.active; }
    }

    public string CurrentPlayerName
    {
        get
        {
            return currentPlayerName;
        }

        set
        {
            currentPlayerName = value;
        }
    }

    public NetworkManagerCustom NetworkManager
    {
        get
        {
            return m_networkManager;
        }

        set
        {
            m_networkManager = value;
        }
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
        if (m_currenPlayScreen == PlayScreen.PlayerLobby || m_currenPlayScreen == PlayScreen.LAN)
        {
            ChangePlayScreen(PlayScreen.Main);
        }
        else if (m_currenPlayScreen == PlayScreen.Main)
            ChangeMainScreen(LobbyScreen.Home);

        NDiscovery.Instance.StopBroadcasting();
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

    public void OnLobbyStartGameBtnClicked()
    {
        GameManager.Instance.ShowChaserSelection();
        m_playerLobbyStartGameBtn.SetActive(false);
        //m_networkManager.ServerChangeScene(m_networkManager.playScene);
    }
    public void OnLobbyEnterHeroSelectionClicked()
    {
        Debug.Log("OnLobbyEnterHeroSelectionClicked");
        GameManager.Instance.ShowHeroSelection();
    }

    public void OnNameValueEntered(string value)
    {
        Debug.Log("OnNameValueEntered : " + value);
        PlayerPrefs.SetString(m_nameSaveKey, value);
        CheckAndShowNameInput();
    }

    public void OnNameChangeBtnClicked()
    {
        m_nameAreaGO.SetActive(false);
        m_nameInputGO.SetActive(true);
    }

    public void OnHeroSelectionConfirm()
    {
        if (m_selectedHeroData.m_name != null)
        {
            GameManager.Instance.CurrentPlayerHeroData = m_selectedHeroData;
            GameManager.Instance.CurrentLobbyPlayer.OnHeroSelected(GameManager.Instance.CurrentPlayerSlot, m_selectedHeroData);
            m_heroSelectionConfirmBtnGO.SetActive(false);

            m_heroSelectionStatusText.text = "Hero Selected";

            m_heroSelectionContentGO.transform.parent.parent.gameObject.SetActive(false);
            m_heroSelectionSelectedHeroGO.SetActive(true);
            m_heroSelectionSelectedHeroGO.transform.Find("Text").GetComponent<Text>().text = m_selectedHeroData.m_name;
        }
        else
        {
            Debug.Log("no hero selected");
        }
    }

    public void OnEnterGameClicked()
    {
        NetworkManager.LoadGameScene();
    }

    #endregion

    #region Networks
    void HostGame()
    {
        if (m_LANRoomNameInput.text.Length > 1)
        {
            NDiscovery.Instance.SetBroadcastData(m_LANRoomNameInput.text);
            NDiscovery.Instance.StartBroadcasting();
            NetworkManager.StartHost();
        }
        else
            Debug.Log("Room Name error...");
    }

    void FindGames()
    {
        NDiscovery.Instance.ReceiveBraodcast();
    }

    public void JoinGame(string address)
    {
        NDiscovery.Instance.StopBroadcasting();
        NetworkManager.networkAddress = address;
        NetworkManager.StartClient();
    }

    public void OnReceiveBraodcast(string fromIp, string data)
    {
        Debug.Log("Msg from : " + fromIp + ", Data : " + data);
        UpdateRoomsList(fromIp + "," + data);
    }

    public void OnClientEnterLobby(LobbyPlayer player, int id,bool isLocal)
    {
        if (isLocal)
        {
            GameManager.Instance.CurrentLobbyPlayer = player;
            GameManager.Instance.CurrentPlayerSlot = id;
            GameManager.Instance.UpdatePlayersName(CurrentPlayerName);
        }
        ChangePlayScreen(PlayScreen.PlayerLobby);


        for (int i = 0; i < 4; i++)
        {
            if (i <= id)
            {
                m_playerLobbyContainerGO.transform.GetChild(i).Find("Image").GetComponent<Image>().color = Color.white;
                m_playerLobbyContainerGO.transform.GetChild(i).Find("PlayerName").GetComponent<Text>().text = GameManager.Instance.m_playersNameList[i];
            }
            else
            {
                m_playerLobbyContainerGO.transform.GetChild(i).Find("Image").GetComponent<Image>().color = Color.grey;
                m_playerLobbyContainerGO.transform.GetChild(i).Find("PlayerName").GetComponent<Text>().text = "";
            }
        }
    }

    public void OnAllPlayerReady()
    {
        Debug.Log("OnAllPlayerReady");
        if (IsRunningAsServer)
        {
            m_playerLobbyEnterHeroeSelectionBtn.SetActive(false);
            m_playerLobbyStartGameBtn.SetActive(true);
            Debug.Log("Running as a server");
        }
        else
        {
            Debug.Log("Running as a client");
        }
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
                    case PlayScreen.PlayerLobby:
                        m_playerLobbyArea.SetActive(activate);
                        break; 
                    case PlayScreen.HeroSelection:
                        m_heroSelectionArea.SetActive(activate);
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

    void UpdateHeroSelectionScreen(int heroType)
    {
        int tempNo = 0;
        for (int i = 0; i < GameManager.Instance.AllHeroesData.Length; i++)
        {
            if(GameManager.Instance.AllHeroesData[i].m_heroType == (heroType == 0 ? HeroType.Chaser : HeroType.Runner))
            {
                HeroData currentHeroData = GameManager.Instance.AllHeroesData[i];
                m_heroSelectionContentGO.transform.GetChild(tempNo).GetComponent<HeroSelectionObj>().Init(currentHeroData);
                m_heroSelectionContentGO.transform.GetChild(tempNo).Find("NameText").GetComponent<Text>().text = currentHeroData.m_name;
                tempNo++;
            }
        }
        for (int i = tempNo; i < m_heroSelectionContentGO.transform.childCount; i++)
        {
            m_heroSelectionContentGO.transform.GetChild(i).gameObject.SetActive(false);
        }
        m_heroSelectionStatusText.text =  (heroType == 0 ? m_heroSelectionChaserMsg : m_heroSelectionRunnerMsg);
    }

    #endregion

    void CheckAndShowNameInput()
    {
        if(PlayerPrefs.HasKey(m_nameSaveKey))
        {
            currentPlayerName = PlayerPrefs.GetString(m_nameSaveKey);
        }
        m_nameAreaGO.SetActive(PlayerPrefs.HasKey(m_nameSaveKey));
        m_nameInputGO.SetActive(!PlayerPrefs.HasKey(m_nameSaveKey));
        m_nameAreaGO.transform.Find("Text").GetComponent<Text>().text = PlayerPrefs.HasKey(m_nameSaveKey) ? m_welcomeMsg + PlayerPrefs.GetString(m_nameSaveKey) : "";
    
    }
    
    public void ShowChaserSelection()
    {
        if (IsRunningAsServer)
        {
            foreach (Transform item in m_playerLobbyContainerGO.transform)
            {
                item.Find("ChaserSelection").gameObject.SetActive(true);
            }
            m_playerLobbyStatusText.text = m_hostChaserSelectionMsg;
        }
        else 
        {
            m_playerLobbyStatusText.text = m_clientChaserSelectionMsg;
        }
    }

    public void OnPlayerChaserSelected(Toggle toggle)
    {
        int index = int.Parse(toggle.transform.parent.name.Remove(0, 6));
        if (toggle.isOn)
        {
            GameManager.Instance.SetFinalHeroArray(index);
        }

        int playersNo = 0;
        for (int i = 0; i < NetworkManager.lobbySlots.Length; i++)
        {
            if(NetworkManager.lobbySlots[i] != null)
            {
                playersNo++;
            }
        }
 
        m_playerLobbyEnterHeroeSelectionBtn.SetActive(index <= playersNo ? true : false);
    }

    public void ShowHeroSelection(int heroType)
    {
        ChangePlayScreen(PlayScreen.HeroSelection);
        UpdateHeroSelectionScreen(heroType);
    }

    public void OnHeroSelected(HeroData heroData)
    {
        m_selectedHeroData = heroData;
    }

    public void ShowEnterGameBtn()
    {
        m_heroSelectionEnterGameBtnGO.SetActive(true);
    }
}

public enum LobbyScreen { None, Home, Play, Settings, Store }
public enum PlayScreen { None, Main, LAN, Online, PlayerLobby,HeroSelection }
public enum Map { None, GreenJungle }
