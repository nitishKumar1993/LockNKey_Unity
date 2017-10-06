using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private float m_gameWaterHeight;

    [SerializeField]
    private float m_gameDurationInMins;

    [SerializeField]
    private List<GameObject> m_islandsList = new List<GameObject>();

    public SyncListInt m_finalHeroTypeSelection = new SyncListInt();

    private List<HeroData> m_finalHeroSelectionList = new List<HeroData>();

    private HeroData[] m_allHeroesData;

    public SyncListString m_playersNameList = new SyncListString();

    private int currentPlayerSlot;
    private LobbyPlayer currentLobbyPlayer;

    private HeroData currentPlayerHeroData;

    private GameObject m_inGameCanvas;

    public SyncListInt FinalHeroTypeSelection
    {
        get
        {
            return m_finalHeroTypeSelection;
        }

        set
        {
            m_finalHeroTypeSelection = value;
        }
    }

    public float GameWaterHeight
    {
        get
        {
            return m_gameWaterHeight;
        }

        set
        {
            m_gameWaterHeight = value;
        }
    }

    public HeroData[] AllHeroesData
    {
        get
        {
            return m_allHeroesData;
        }

        set
        {
            m_allHeroesData = value;
        }
    }

    public int CurrentPlayerSlot
    {
        get
        {
            return currentPlayerSlot;
        }

        set
        {
            currentPlayerSlot = value;
        }
    }

    public HeroData CurrentPlayerHeroData
    {
        get
        {
            return currentPlayerHeroData;
        }

        set
        {
            currentPlayerHeroData = value;
        }
    }

    public LobbyPlayer CurrentLobbyPlayer
    {
        get
        {
            return currentLobbyPlayer;
        }

        set
        {
            currentLobbyPlayer = value;
        }
    }

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        AllHeroesData = ResourceManager.GetHeroesData();
    }

    void Start()
    {
        // StartCoroutine(RemoveIslands());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            m_inGameCanvas = GameObject.FindGameObjectWithTag("InGameCanvas");
        }
    }

    public void UpdatePlayersName(string name)
    {
        if (m_playersNameList.Count != 4)
        {
            m_playersNameList.Clear();
            for (int i = 0; i < 4; i++)
            {
                m_playersNameList.Add("");
            }
        }
        m_playersNameList[currentPlayerSlot] = name;
    }

    public void SetFinalHeroArray(int chaserIndex)
    {
        FinalHeroTypeSelection.Clear();
        for (int i = 0; i < 4; i++)
        {
            if (i == chaserIndex)
            {
                FinalHeroTypeSelection.Add(1);
            }
            else FinalHeroTypeSelection.Add(0);
        }
    }

    public void ShowChaserSelection()
    {
        RpcShowChaserSelection();
    }

    [ClientRpc]
    void RpcShowChaserSelection()
    {
        LobbyManager.Instance.ShowChaserSelection();
    }

    public void ShowHeroSelection()
    {
        RpcShowHeroSelection();
    }

    [ClientRpc]
    void RpcShowHeroSelection()
    {
        LobbyManager.Instance.ShowHeroSelection(currentPlayerSlot);
    }

    [ClientRpc]
    void RpcLoadGameScene()
    {
        LobbyManager.Instance.NetworkManager.LoadGameScene();
    }

    public void OnHeroSelectedCallBack(int playerIndex, HeroData heroData)
    {
        if (m_finalHeroSelectionList.Count < 4)
        {
            m_finalHeroSelectionList = new List<HeroData>();
            for (int i = 0; i < 4; i++)
            {
                m_finalHeroSelectionList.Add(new HeroData());
            }
        }

        m_finalHeroSelectionList[playerIndex] = heroData;

        if (GetReadyPlayersHeroCount() >= NetworkServer.connections.Count)
        {
            LobbyManager.Instance.ShowEnterGameBtn();
        }
    }

    int GetReadyPlayersHeroCount()
    {
        int tempNo = 0;
        for (int i = 0; i < m_finalHeroSelectionList.Count; i++)
        {
            if(m_finalHeroSelectionList[i].m_name  != null)
            {
                tempNo++;
            }
        }
        return tempNo;
    }

    public void SetSkillButtonHandler(UnityEngine.Events.UnityAction action)
    {
        Button skillBtn = m_inGameCanvas.transform.Find("SkillBtn").GetComponent<Button>();
        skillBtn.onClick.AddListener(action);
    }

    IEnumerator RemoveIslands()
    {
        float totalGameDuration = m_gameDurationInMins * 60;
        float currentGameDuration = totalGameDuration / m_islandsList.Count;

        while (totalGameDuration > 0)
        {
            totalGameDuration -= Time.deltaTime;
            currentGameDuration -= Time.deltaTime;
            if (currentGameDuration <= 0)
            {
                // Debug.Log("Remove islands");
                m_islandsList[m_islandsList.Count - 1].GetComponent<IslandBehaviour>().RemoveIsland();

                m_islandsList.RemoveAt(m_islandsList.Count - 1);
                currentGameDuration = totalGameDuration / m_islandsList.Count;
            }
            yield return null;
        }
    }
}
