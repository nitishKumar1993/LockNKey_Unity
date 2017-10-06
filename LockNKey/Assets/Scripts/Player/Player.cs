using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private Animator m_playerAnimator;

    [SerializeField]
    private GameObject m_splashPrefab;

    [SerializeField]
    int m_heroID = 0;

    HeroData m_playerHeroData;

    bool m_movementAllowed = false;
    bool m_isDead = false;
    bool isTouchingGround = true;

    float m_currentMovementSpeed;
    Vector3 m_currentPlayerPosition;

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

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Init()
    {
        if (!this.isLocalPlayer)
            return;

        m_playerHeroData = GameManager.Instance.AllHeroesData[m_heroID];
        Camera.main.gameObject.GetComponent<CameraController>().PlayerGO = this.gameObject;
        m_currentMovementSpeed = (m_playerHeroData.m_movementSpeed);
        m_currentPlayerPosition = this.transform.position;
        GameManager.Instance.SetSkillButtonHandler(UseSkill);
    }

    void FixedUpdate()
    {
        if (this.isLocalPlayer)
            Move(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

        if (!m_isDead)
            PlayerAnimator.SetFloat("WalkSpeed", Vector3.Distance(this.transform.position, m_currentPlayerPosition) * 5);
        else PlayerAnimator.SetBool("Dead", true);

        m_currentPlayerPosition = this.transform.position;
    }

    void Move(float xInput, float yInput)
    {
        if (!m_isDead && isTouchingGround)
        {
            if ((Mathf.Abs(xInput) > 0 || Mathf.Abs(yInput) > 0) && !MovementAllowed)
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
            SkillsManager.Instance.UseSkill(this.gameObject, m_playerHeroData.m_skillID);
            CmdUseSkill();
        }
    }

    [Command]
    void CmdUseSkill()
    {
        SkillsManager.Instance.UseSkill(this.gameObject, m_playerHeroData.m_skillID);
    }

    [ClientRpc]
    void RpcUseSkill()
    {
        SkillsManager.Instance.UseSkill(this.gameObject, m_playerHeroData.m_skillID);
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

    [ClientRpc]
    void RpcOnGroundTouchingStopped()
    {
        isTouchingGround = false;
        Debug.Log("HasStoppedTouchingGround");
    }
}
