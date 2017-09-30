using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using UnityEngine.AI;

public class Player : MonoBehaviour
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
        m_playerHeroData = GameManager.Instance.AllHeroesData[m_heroID];
        Camera.main.gameObject.GetComponent<CameraController>().PlayerGO = this.gameObject;
    }


    void FixedUpdate()
    {
        if (!m_isDead && isTouchingGround)
        {
            if ((Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0 || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0) && !MovementAllowed)
            {
                float temp = Mathf.Max(Mathf.Abs(CnInputManager.GetAxis("Horizontal")), Mathf.Abs(CnInputManager.GetAxis("Vertical")));
                PlayerAnimator.SetFloat("WalkSpeed", temp);

                Vector3 m_nextPos = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));
                this.transform.position += (m_nextPos * 10 * m_playerHeroData.m_movementSpeed / 100) * Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_nextPos), Time.deltaTime * 10);
            }
            else
            {
                PlayerAnimator.SetFloat("WalkSpeed", 0);
            }
        }
        else
        {
            PlayerAnimator.SetFloat("WalkSpeed", 0);
            PlayerAnimator.SetBool("Dead", true);
        }
    }

    public void UseSkill()
    {
        SkillsManager.Instance.UseSkill(this.gameObject, m_playerHeroData.m_skillID);
    }

    public void OnEnterWater()
    {
        m_isDead = true;
        GameObject tempSplash = Instantiate(m_splashPrefab, this.transform.position + this.transform.forward, Quaternion.identity) as GameObject;
        Debug.Log("Dead");
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
           if(this.transform.position.y - collision.contacts[i].point.y < -0.5f)
            {
                isTouchingGround = false;
            }
        }
    }
}
