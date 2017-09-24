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

    // Use this for initialization
    void Start()
    {
        m_playerNavmeshAgent.updatePosition = true;
        m_playerNavmeshAgent.updateRotation = true;

        StartCoroutine(CheckDeath());
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
    }

    void FixedUpdate()
    {
        if ((Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0 || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0) && !m_movementAllowed)
        {
            float temp = Mathf.Max(Mathf.Abs(CnInputManager.GetAxis("Horizontal")), Mathf.Abs(CnInputManager.GetAxis("Vertical")));
            m_playerAnimator.SetFloat("WalkSpeed", temp);

            m_nextPos = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));
            m_playerNavmeshAgent.SetDestination(this.transform.position + (m_nextPos * 3));
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

    IEnumerator CheckDeath()
    {
        NavMeshHit navMeshHit;
        bool deathCheck = false;
        bool dead = false;
        while(!dead)
        {
            if (NavMesh.SamplePosition(m_playerNavmeshAgent.transform.position, out navMeshHit, 0.1f, -1))
            {
                if (navMeshHit.mask == 8)
                {
                    if (deathCheck)
                    {
                        dead = true;
                        GameObject tempSplash = Instantiate(m_splashPrefab, navMeshHit.position + Vector3.up * 3, Quaternion.identity) as GameObject;
                        Debug.Log("Dead");
                    }
                    deathCheck = true;
                }
                else
                {
                    deathCheck = false;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

}
