using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;

    public SkillBehaviour[] m_skillsPrefabArray;

    private SkillData[] m_allSkillsData;

    private void Awake()
    {
        Instance = this;
        m_allSkillsData = ResourceManager.GetSkillsData();
    }

    // Use this for initialization
    void Start()
    {

    }


    public void UseSkill(GameObject sourcePlayerGO, int skillId)
    {
        switch (skillId)
        {
            case 0:
                CreateAfterImage(sourcePlayerGO, skillId);
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    void CreateAfterImage(GameObject playerGO, int skillId)
    {
        Debug.Log("CreateAfterImage");

        StartCoroutine(CreateAfterImageCR(playerGO, skillId));
    }

    IEnumerator CreateAfterImageCR(GameObject playerGO, int skillId)
    {
        GameObject tempCopy = Instantiate(playerGO, playerGO.transform.position + playerGO.transform.forward, playerGO.transform.rotation);
        tempCopy.GetComponent<NavMeshAgent>().SetDestination(tempCopy.transform.position + tempCopy.transform.forward * 20);
        tempCopy.GetComponent<Player>().PlayerAnimator.SetFloat("WalkSpeed", 1);
        Destroy(tempCopy.GetComponent<Player>());

        NavMeshAgent mNavMeshAgent = tempCopy.GetComponent<NavMeshAgent>();
        while (Vector3.Distance(mNavMeshAgent.destination, mNavMeshAgent.transform.position) > mNavMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        Destroy(tempCopy);
    }
}
