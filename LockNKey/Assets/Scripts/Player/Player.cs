using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_playerMeshHolderGO;

    [SerializeField]
    private Animator m_playerAnimator;

    [SerializeField]
    private GameObject m_splashPrefab;

    private HeroData m_playerHeroData;

    bool m_movementAllowed = false;
    bool m_isDead = false;
    bool isTouchingGround = true;
    bool m_isFronze = false;
    bool m_isImmune = false;
    bool m_skillOnCD = false;

    float m_currentMovementSpeed;
    Vector3 m_currentPlayerPosition;

    public int CurrentPlayerSlot = -1;

    public int testHeroID = 0;

    GameObject m_currentRunnerUISlotGO;
    
    public bool MovementAllowed
    {
        get
        {
            return m_movementAllowed;
        }

        set
        {
            m_movementAllowed = value;
        }
    }
    
    public HeroData PlayerHeroData
    {
        get
        {
            return m_playerHeroData;
        }

        set
        {
            m_playerHeroData = value;
        }
    }

    public bool IsFronze
    {
        get
        {
            return m_isFronze;
        }

        set
        {
            m_isFronze = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return m_isDead;
        }

        set
        {
            m_isDead = value;
        }
    }

    public bool IsImmune
    {
        get
        {
            return m_isImmune;
        }

        set
        {
            m_isImmune = value;
        }
    }

    public GameObject PlayerMeshHolderGO
    {
        get
        {
            return m_playerMeshHolderGO;
        }

        set
        {
            m_playerMeshHolderGO = value;
        }
    }

    public Animator PlayerAnimator
    {
        get
        {
            return m_playerAnimator;
        }

        set
        {
            m_playerAnimator = value;
        }
    }

    public float CurrentMovementSpeed
    {
        get
        {
            return m_currentMovementSpeed;
        }

        set
        {
            m_currentMovementSpeed = value;
        }
    }

    void Start()
    {
        if (testHeroID != 0)
        {
            Initialize();
        }
        else
            Invoke("Initialize", 1);
    }

    void Initialize()
    {
        if (isLocalPlayer)
        {
            CurrentPlayerSlot = GameManager.Instance.CurrentPlayerSlot;
            CmdSetCurrentSlot(CurrentPlayerSlot);
        }
        else if (testHeroID != 0)
        {
            Init();
        }
    }

    [Command]
    void CmdSetCurrentSlot(int slot)
    {
        Debug.Log("CmdSetCurrentSlot :" + slot);
        CurrentPlayerSlot = slot;
        RpcSetCurrentSlot(slot);
    }

    [ClientRpc]
    void RpcSetCurrentSlot(int slot)
    {
        Debug.Log("RpcSetCurrentSlot :" + slot);
        CurrentPlayerSlot = slot;
        Init();
    }

    void Init()
    {
        PlayerHeroData = (testHeroID != 0) ? GameManager.Instance.AllHeroesData[testHeroID] : GameManager.Instance.FinalHeroSelectionList[CurrentPlayerSlot];
        SetMeshPlayer();
        MoveToSpawnPos();
        this.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.Instance.ChangeLayers(this, LayerMask.NameToLayer(PlayerHeroData.m_heroType.ToString()));
        if (PlayerHeroData.m_heroType == HeroType.Runner)
        {
            m_currentRunnerUISlotGO = InGameUIManager.Instance.GetUIRunnerFronzenSlot(PlayerHeroData.m_name);
            GameManager.Instance.AllRunnersList.Add(this);
        }
        else
        {
            GameManager.Instance.AllChasersList.Add(this);
        }

        if (testHeroID == 0)
            GameManager.Instance.SetPlayerReady();


        if (!this.isLocalPlayer && (testHeroID == 0))
            return;

        Camera.main.gameObject.GetComponent<CameraController>().PlayerGO = this.gameObject;
        m_currentMovementSpeed = (PlayerHeroData.m_movementSpeed);
        m_currentPlayerPosition = this.transform.position;
        GameManager.Instance.SetSkillButtonHandler(UseSkill);
        GameManager.Instance.CurrentPlayer = this ;
        MovementAllowed = true;
    }

    void MoveToSpawnPos()
    {
        if (PlayerHeroData.m_heroType == HeroType.Chaser)
            PlayersSpawnManager.Instance.SpawnChaser(this.gameObject);
        else if (PlayerHeroData.m_heroType == HeroType.Runner)
            PlayersSpawnManager.Instance.SpawnRunner(this.gameObject);
    }

    void SetMeshPlayer()
    {
        string path = "Prefabs/Players/" + PlayerHeroData.m_heroType.ToString() + "/" + PlayerHeroData.m_name;
        GameObject meshPrefab = Resources.Load<GameObject>(path) as GameObject;

        if(meshPrefab)
        {
            if(PlayerMeshHolderGO.transform.childCount > 0)
            {
                Destroy(PlayerMeshHolderGO.transform.GetChild(0));
            }

            GameObject currentMeshGo = Instantiate(meshPrefab, PlayerMeshHolderGO.transform);
            currentMeshGo.transform.localPosition = Vector3.zero + meshPrefab.transform.position;
            m_playerAnimator = currentMeshGo.GetComponent<Animator>();
        }
        else
        {
            Debug.Log("Couldnt find mesh at path :" + path);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if ((this.isLocalPlayer) && !IsDead && isTouchingGround && MovementAllowed)
            Move(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

        if (m_playerAnimator != null)
        {
            if (!IsDead && !IsFronze)
            {
                float temp = (Vector3.Distance(this.transform.position, m_currentPlayerPosition) * 10) / PlayerHeroData.m_movementSpeed;
                m_playerAnimator.SetFloat("WalkSpeed", temp);
            }
        }

        m_currentPlayerPosition = this.transform.position;
    }

    void Move(float xInput, float yInput)
    {
        if ((Mathf.Abs(xInput) > 0 || Mathf.Abs(yInput) > 0))
        {
            Vector3 nextPos = new Vector3(xInput, 0, yInput);
            this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + nextPos * m_currentMovementSpeed, 0.05f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos), Time.deltaTime * 10);
        }
    }

    public void RemoteMove(float xInput, float yInput)
    {
        RpcMove(xInput, yInput);
    }

    [ClientRpc]
    public void RpcMove(float xInput, float yInput)
    {
        Move(xInput, yInput);
    }

    public void UseSkill()
    {
        if (isServer)
            RpcUseSkill();
        else
        {
            SkillsManager.Instance.UseSkill(this.gameObject, PlayerHeroData.m_skillID);
            CmdUseSkill();
        }
    }

    public void Freeze(bool action, string ChaserName)
    {
        if(GameManager.Instance.GameStarted)
            RpcFreeze(action, ChaserName);
    }

    [ClientRpc]
    public void RpcFreeze(bool action, string ChaserName)
    {
        OnFreeze(action, ChaserName);
    }

    void OnFreeze(bool action, string ChaserName)
    {
        Debug.Log(PlayerHeroData.m_name + " : OnFreeze :" + action + " for Hero: " + PlayerHeroData.m_name + " ,from : " + ChaserName);
        InGameUIManager.Instance.ShowRunnerFrozenStatus(m_currentRunnerUISlotGO, action);
        MovementAllowed = !action;
        IsFronze = action;
        this.GetComponent<Rigidbody>().isKinematic = action;
        PlayerMeshHolderGO.transform.GetChild(0).GetComponent<PlayerMesh>().Freeze(action);
        m_playerAnimator.speed = action ? 0 : 1;
        if (isServer)
            GameManager.Instance.OnRunnerFrozen(action);
    }

    [Command]
    void CmdUseSkill()
    {
        if (!m_skillOnCD)
        {
            SkillsManager.Instance.UseSkill(this.gameObject, PlayerHeroData.m_skillID);
            StartCoroutine(RunSkillTimer());
        }
    }

    [ClientRpc]
    void RpcUseSkill()
    {
        if (!m_skillOnCD)
        {
            SkillsManager.Instance.UseSkill(this.gameObject, PlayerHeroData.m_skillID);
            StartCoroutine(RunSkillTimer());
        }
    }

    [Server]
    public void OnEnterWater()
    {
        RpcOnEnterWater();
    }

    [ClientRpc]
    void RpcOnEnterWater()
    {
        Instantiate(m_splashPrefab, this.transform.position + this.transform.forward, Quaternion.identity);
        if (PlayerHeroData.m_heroType == HeroType.Chaser)
        {
            if (isLocalPlayer)
            {
                IsDead = true;
                StartCoroutine(SpawnChaserWithDelayCR());
            }
        }
        else if (PlayerHeroData.m_heroType == HeroType.Runner)
        {
            if (isLocalPlayer)
                PlayersSpawnManager.Instance.SpawnRunner(this.gameObject);
        }
    }

    IEnumerator SpawnChaserWithDelayCR()
    {
        int tempTimer = 3;
        InGameUIManager.Instance.ShowDeathScreen(true);
        while (tempTimer > 0)
        {
            InGameUIManager.Instance.UpdateDeathScreenText(tempTimer);
            tempTimer --;
            yield return new WaitForSeconds(1);
        }
        IsDead = false;
        InGameUIManager.Instance.ShowDeathScreen(false);
        PlayersSpawnManager.Instance.SpawnChaser(this.gameObject);
    }

    IEnumerator RunSkillTimer()
    {
        m_skillOnCD = true;
        float skilltime = SkillsManager.Instance.AllSkillsData[PlayerHeroData.m_skillID].m_CD;
        float tempTimer = skilltime;
        if (isLocalPlayer)
            InGameUIManager.Instance.ShowSkillCD(true);

        while (tempTimer > 0)
        {
            tempTimer -= Time.deltaTime;
            if (isLocalPlayer)
                InGameUIManager.Instance.UpdateSkillCD(tempTimer/skilltime);
            yield return null;
        }
        if (isLocalPlayer)
            InGameUIManager.Instance.ShowSkillCD(false);
        m_skillOnCD = false;
    }
    public void GoBlind()
    {
        Debug.Log("Go Blind");
        if (isServer && isLocalPlayer)
            RpcGoBlind();
        else
            CmdGoBlind();
    }

    [Command]
    void CmdGoBlind()
    {
        Debug.Log("CmdGoBlind");
        RpcGoBlind();
    }

    [ClientRpc]
    void RpcGoBlind()
    {
        Debug.Log("RpcGoBlind");
        if (isLocalPlayer)
            StartCoroutine(GoBlindCR());
    }

    IEnumerator GoBlindCR()
    {
        LayerMask oldMask = Camera.main.cullingMask;
        int mask = PlayerHeroData.m_heroType == HeroType.Chaser ? 1024 : 2048;

        Camera.main.cullingMask = mask;

        yield return new WaitForSeconds(SkillsManager.Instance.AllSkillsData[PlayerHeroData.m_skillID].m_duration);
        Camera.main.cullingMask = oldMask;
    }

    [Server]
    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.Instance.GameStarted)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (this.transform.position.y - collision.contacts[i].point.y < -0.5f && collision.contacts[i].thisCollider.gameObject.tag == "Island")
                {
                    RpcOnGroundTouchingStopped();
                }
            }
        }
    }

    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance.GameStarted)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.gameObject.tag == "Player")
                {
                    Player otherPlayer = collision.gameObject.GetComponent<Player>();
                    if (otherPlayer)
                    {
                        if (this.PlayerHeroData.m_heroType == HeroType.Chaser && otherPlayer.PlayerHeroData.m_heroType == HeroType.Runner)
                        {
                            if (!otherPlayer.IsImmune && !otherPlayer.IsFronze && !collisionTimerRunning)
                            {
                                otherPlayer.Freeze(true, this.gameObject.name);
                                StartCoroutine(runCollsionTimer());
                            }
                        }
                        else if ((this.PlayerHeroData.m_heroType == HeroType.Runner && otherPlayer.PlayerHeroData.m_heroType == HeroType.Runner) && otherPlayer.IsFronze)
                        {
                            otherPlayer.Freeze(false, this.gameObject.name);
                        }
                    }
                    else
                    {
                        Debug.Log("Player component not found on the player GO");
                    }
                }
            }
        }
    }

    bool collisionTimerRunning = false;
    IEnumerator runCollsionTimer()
    {
        float timer = 0.2f;
        collisionTimerRunning = true;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        collisionTimerRunning = false;
           
    }

    [ClientRpc]
    void RpcOnGroundTouchingStopped()
    {
        isTouchingGround = false;
        Debug.Log("HasStoppedTouchingGround");
    }
}
