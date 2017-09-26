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
    private NavMeshAgent m_playerNavmeshAgent;

    [SerializeField]
    private GameObject m_splashPrefab;

    Vector3 m_nextPos = Vector3.zero;

    bool m_movementAllowed = false;
    bool m_isDead = false;

    // Use this for initialization
    void Start()
    {
        m_playerNavmeshAgent.updatePosition = true;
        m_playerNavmeshAgent.updateRotation = true;

        CheckDeath();
    }

    void Update()
    {
        /* if (Input.GetMouseButtonDown(1))
         {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             RaycastHit hitInfo;

             if (Physics.Raycast(ray, out hitInfo, 100))
             {
                 Debug.Log(hitInfo.point);
                 m_playerNavmeshAgent.SetDestination(hitInfo.point);
             }
         }*/

        CheckDeath();
    }

    void FixedUpdate()
    {
        if ((Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0 || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0) && !m_movementAllowed)
        {
            float temp = Mathf.Max(Mathf.Abs(CnInputManager.GetAxis("Horizontal")), Mathf.Abs(CnInputManager.GetAxis("Vertical")));
            m_playerAnimator.SetFloat("WalkSpeed", temp);

            m_nextPos = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));
            m_playerNavmeshAgent.SetDestination(this.transform.position + (m_nextPos * 2));
        }
        else
        {
            if (m_nextPos != Vector3.zero)
            {
                m_playerAnimator.SetFloat("WalkSpeed", 0);

                m_playerNavmeshAgent.SetDestination(this.transform.position + m_nextPos * 0.1f);
                m_nextPos = Vector3.zero;
            }
        }
    }

    public void Attack()
    {
        StopCoroutine("AttackCR");
        StartCoroutine("AttackCR");
    }

    IEnumerator AttackCR()
    {
        m_movementAllowed = true;
        m_playerAnimator.SetBool("Attack", false);
        yield return new WaitForEndOfFrame();
        m_playerAnimator.SetBool("Attack", true);
        yield return new WaitForSeconds(m_playerAnimator.GetCurrentAnimatorClipInfo(0).Length);
        m_playerAnimator.SetBool("Attack", false);
        m_movementAllowed = false;
    }

    void CheckDeath()
    {

        if (this.transform.position.y <= GameManager.Instance.GameWaterHeight)
        {
            if (!m_isDead)
            {
                m_isDead = true;
                GameObject tempSplash = Instantiate(m_splashPrefab, this.transform.position, Quaternion.identity) as GameObject;
                Debug.Log("Dead");
            }
        }
        else
        {
            m_isDead = false;
        }
    }

}
