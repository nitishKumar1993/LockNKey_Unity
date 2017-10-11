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

    float m_currentMovementSpeed;
    Vector3 m_currentPlayerPosition;

    public int CurrentPlayerSlot = -1;

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

    void Start()
    {
        if (GameManager.Instance.isTesting)
        {
            Initialize();
        }
        else
            Invoke("Initialize", 1);
    }

    void Initialize()
    {
        Debug.Log(this.isLocalPlayer);
        if (isLocalPlayer)
        {
            CurrentPlayerSlot = GameManager.Instance.CurrentPlayerSlot;
            CmdSetCurrentSlot(CurrentPlayerSlot);
        }
        else if (GameManager.Instance.isTesting)
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
        PlayerHeroData = GameManager.Instance.isTesting ? GameManager.Instance.AllHeroesData[GameManager.Instance.testHeroID] : GameManager.Instance.FinalHeroSelectionList[CurrentPlayerSlot];
        GameManager.Instance.SetPlayerReady();
        SetMeshPlayer();

        if (!this.isLocalPlayer && !GameManager.Instance.isTesting)
            return;

        Camera.main.gameObject.GetComponent<CameraController>().PlayerGO = this.gameObject;
        m_currentMovementSpeed = (PlayerHeroData.m_movementSpeed);
        m_currentPlayerPosition = this.transform.position;
        GameManager.Instance.SetSkillButtonHandler(UseSkill);
        GameManager.Instance.CurrentPlayer = this ;
        MovementAllowed = true;
    }

    void SetMeshPlayer()
    {
        string path = "Prefabs/Players/" + PlayerHeroData.m_heroType.ToString() + "/" + PlayerHeroData.m_name;
        GameObject meshPrefab = Resources.Load<GameObject>(path) as GameObject;

        if(meshPrefab)
        {
            if(m_playerMeshHolderGO.transform.childCount > 0)
            {
                Destroy(m_playerMeshHolderGO.transform.GetChild(0));
            }

            GameObject currentMeshGo = Instantiate(meshPrefab, m_playerMeshHolderGO.transform);
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
        if (this.isLocalPlayer || GameManager.Instance.isTesting)
            Move(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

        if (m_playerAnimator != null)
        {
            if (!m_isDead)
                m_playerAnimator.SetFloat("WalkSpeed", Vector3.Distance(this.transform.position, m_currentPlayerPosition) * 5);
            else m_playerAnimator.SetBool("Dead", true);
        }

        m_currentPlayerPosition = this.transform.position;
    }

    void Move(float xInput, float yInput)
    {
        if (!m_isDead && isTouchingGround)
        {
            if ((Mathf.Abs(xInput) > 0 || Mathf.Abs(yInput) > 0) && MovementAllowed)
            {
                Vector3 m_nextPos = new Vector3(xInput, 0, yInput);
                this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + m_nextPos * m_currentMovementSpeed, 0.05f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_nextPos), Time.deltaTime * 10);
            }
        }
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

    public void Freeze(bool action)
    {
        OnFreeze(action);
        RpcFreeze(action);
    }

    void OnFreeze(bool action)
    {
        Debug.Log("OnFreeze :" + action + " for Hero: " + PlayerHeroData.m_name);
        MovementAllowed = !action;
        IsFronze = action;
        this.GetComponent<Rigidbody>().isKinematic = action;
        m_playerMeshHolderGO.transform.GetChild(0).GetComponent<PlayerMesh>().Freeze(true);
        m_playerAnimator.speed = action ? 0 : 1;
    }

    [ClientRpc]
    public void RpcFreeze(bool action)
    {
        OnFreeze(action);
    }

    [Command]
    void CmdUseSkill()
    {
        SkillsManager.Instance.UseSkill(this.gameObject, PlayerHeroData.m_skillID);
    }

    [ClientRpc]
    void RpcUseSkill()
    {
        SkillsManager.Instance.UseSkill(this.gameObject, PlayerHeroData.m_skillID);
    }

    [Server]
    public void OnEnterWater()
    {
        RpcOnEnterWater();
    }

    [ClientRpc]
    void RpcOnEnterWater()
    {
        m_isDead = true;
        Instantiate(m_splashPrefab, this.transform.position + this.transform.forward, Quaternion.identity);
        Debug.Log("Dead");
    }

    [Server]
    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (this.transform.position.y - collision.contacts[i].point.y < -0.5f && collision.contacts[i].thisCollider.gameObject.tag == "Island")
            {
                RpcOnGroundTouchingStopped();
            }
        }
    }

    [Server]
    private void OnCollisionEnter(Collision collision)
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
                        otherPlayer.Freeze(true);
                    }
                    else if ((this.PlayerHeroData.m_heroType == HeroType.Runner && otherPlayer.PlayerHeroData.m_heroType == HeroType.Runner) && otherPlayer.IsFronze)
                    {
                        otherPlayer.Freeze(false);
                    }
                }
                else
                {
                    Debug.Log("Player component not found on the player GO");
                }
            }
        }
    }

    [ClientRpc]
    void RpcOnGroundTouchingStopped()
    {
        isTouchingGround = false;
        Debug.Log("HasStoppedTouchingGround");
    }
}
