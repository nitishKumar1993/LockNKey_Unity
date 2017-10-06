using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;

    public Skill[] m_skillsPrefabArray;

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
                CreateAfterImage(sourcePlayerGO);
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

    void CreateAfterImage(GameObject sourcePlayerGO)
    {
        Skill thisSkill = GetSkillOfType(SkillType.AfterImage);

        GameObject tempCopy = Instantiate(thisSkill.m_skillPrefab, sourcePlayerGO.transform.position + thisSkill.m_positionToSpawnAt, sourcePlayerGO.transform.rotation);
        tempCopy.GetComponent<AfterImageLogic>().Init(sourcePlayerGO);
        LobbyManager.Instance.NetworkManager.SpawnGO(tempCopy);
    }

    public Skill GetSkillOfType(SkillType type)
    {
        Skill tempSkill = null;

        for (int i = 0; i < m_skillsPrefabArray.Length; i++)
        {
            if(m_skillsPrefabArray[i].m_skillType == type)
            {
                tempSkill = m_skillsPrefabArray[i];
                return tempSkill;
            }
        }
        return tempSkill;
    }
}

[System.Serializable]
public class Skill
{
    public SkillType m_skillType;
    public GameObject m_skillPrefab;
    public Vector3 m_positionToSpawnAt;
}

public enum SkillType { AfterImage}
