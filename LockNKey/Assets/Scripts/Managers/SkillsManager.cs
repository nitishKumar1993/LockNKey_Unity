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

    public SkillData[] AllSkillsData
    {
        get
        {
            return m_allSkillsData;
        }

        set
        {
            m_allSkillsData = value;
        }
    }

    private void Awake()
    {
        Instance = this;
        AllSkillsData = ResourceManager.GetSkillsData();
    }

    // Use this for initialization
    void Start()
    {

    }


    public void UseSkill(GameObject sourcePlayerGO, int skillId)
    {
        sourcePlayerGO.GetComponent<Player>().PlayerAnimator.SetTrigger("Skill");
        switch (skillId)
        {
            case 0:
                CreateAfterImage(sourcePlayerGO);
                break;
            case 1:
                StartCoroutine(ActivateShieldDash(sourcePlayerGO,1));
                break;
            case 2:
                UseBlockBuster(sourcePlayerGO);
                break;
            case 3:
                UseSuicideBomber(sourcePlayerGO);
                break;
            case 4:
                StartCoroutine(UseFear(sourcePlayerGO));
                break;
            case 5:
                StartCoroutine(ActivateAlmightyPush(sourcePlayerGO, 1)); 
                break;
            case 6:
                UseBlind(sourcePlayerGO);
                break;
            case 7:
                break;
        }
    }

    void CreateAfterImage(GameObject sourcePlayerGO)
    {
        Skill thisSkill = GetSkillOfType(SkillType.AfterImage);

        GameObject tempCopy = Instantiate(thisSkill.m_skillPrefab, sourcePlayerGO.transform.position + thisSkill.m_positionToSpawnAt, sourcePlayerGO.transform.rotation);
        tempCopy.GetComponent<AfterImageLogic>().Init(sourcePlayerGO);
        if(sourcePlayerGO.GetComponent<Player>().isLocalPlayer)
            LobbyManager.Instance.NetworkManager.SpawnGO(tempCopy);
    }

    IEnumerator ActivateShieldDash(GameObject sourcePlayerGO, int skillID)
    {
        Skill thisSkill = GetSkillOfType(SkillType.ShieldDash);
        sourcePlayerGO.GetComponent<Player>().IsImmune = true;
        sourcePlayerGO.GetComponent<Player>().MovementAllowed = false;
        sourcePlayerGO.GetComponent<Collider>().enabled = false;
        sourcePlayerGO.GetComponent<Rigidbody>().isKinematic = true;

        GameObject tempCopy = Instantiate(thisSkill.m_skillPrefab, Vector3.zero, sourcePlayerGO.transform.rotation);
        tempCopy.transform.SetParent(sourcePlayerGO.transform);
        tempCopy.transform.localPosition = thisSkill.m_positionToSpawnAt;
        if (sourcePlayerGO.GetComponent<Player>().isLocalPlayer)
            LobbyManager.Instance.NetworkManager.SpawnGO(tempCopy);

        float timer = AllSkillsData[skillID].m_duration;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            sourcePlayerGO.transform.position += sourcePlayerGO.transform.forward * Time.deltaTime * 30 ;
            yield return null;
        }

        Destroy(tempCopy.gameObject);
        sourcePlayerGO.GetComponent<Player>().MovementAllowed = true;
        sourcePlayerGO.GetComponent<Player>().IsImmune = false;
        sourcePlayerGO.GetComponent<Collider>().enabled = true;
        sourcePlayerGO.GetComponent<Rigidbody>().isKinematic = false;
    }

    void UseSuicideBomber(GameObject sourcePlayer)
    {
        Debug.Log("UseSuicideBomber");
        Player thisPlayer = sourcePlayer.GetComponent<Player>();

        sourcePlayer.GetComponent<Rigidbody>().AddForce(Vector3.up * 50, ForceMode.Impulse);

        Collider[] hitColliders = Physics.OverlapSphere(sourcePlayer.transform.position, 4);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Player otherPlayer = hitColliders[i].GetComponent<Player>();
            if (otherPlayer && otherPlayer != thisPlayer)
            {
                Debug.Log("Suicide bomber other player :" + hitColliders[i].name);
                if (thisPlayer.PlayerHeroData.m_heroType != otherPlayer.PlayerHeroData.m_heroType)
                {
                    hitColliders[i].GetComponent<Rigidbody>().AddForce((hitColliders[i].transform.position - sourcePlayer.transform.position).normalized * 50 + Vector3.up * 50, ForceMode.Impulse);
                }
            }
            i++;
        }
    }

    IEnumerator UseFear(GameObject sourcePlayer)
    {
        if (!sourcePlayer.GetComponent<Player>().isLocalPlayer)
            yield break;

        Debug.Log("UseFear");
        Player thisPlayer = sourcePlayer.GetComponent<Player>();
        List<Player> playersList = new List<Player>();
        Collider[] hitColliders = Physics.OverlapSphere(sourcePlayer.transform.position, 4);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Player otherPlayer = hitColliders[i].GetComponent<Player>();
            if (otherPlayer && otherPlayer != thisPlayer)
            {
                if (thisPlayer.PlayerHeroData.m_heroType != otherPlayer.PlayerHeroData.m_heroType)
                {
                    playersList.Add(otherPlayer);
                }
            }
            i++;
        }

        Debug.Log(playersList.Count);

        if (playersList.Count > 0)
        {
            float timer = AllSkillsData[thisPlayer.PlayerHeroData.m_skillID].m_duration;

            while(timer > 0)
            {
                timer -= Time.deltaTime;
                foreach (Player item in playersList)
                {
                    item.MovementAllowed = false;
                    Vector3 direction = (item.transform.position - sourcePlayer.transform.position).normalized;
                    item.RemoteMove(direction.x, direction.z);
                }
                yield return null;
            }
            foreach (Player item in playersList)
            {
                item.MovementAllowed = true;
            }
        }
    }

    void UseBlind(GameObject sourcePlayer)
    {
        Player thisPlayer = sourcePlayer.GetComponent<Player>();
        if (!thisPlayer.isLocalPlayer)
            return;

        List<Player> opponentPlayerList = thisPlayer.PlayerHeroData.m_heroType == HeroType.Chaser ? GameManager.Instance.AllRunnersList : GameManager.Instance.AllChasersList;
        for (int i = 0; i < opponentPlayerList.Count; i++)
        {
            opponentPlayerList[i].GoBlind();
        }
    }

    IEnumerator UseBlockBuster(GameObject sourcePlayer)
    {
        Debug.Log("UseBlockBuster");
        if(!sourcePlayer.GetComponent<Player>().isLocalPlayer)
            yield break;

        Player thisPlayer = sourcePlayer.GetComponent<Player>();
        List<Player> playersList = new List<Player>();
        Collider[] hitColliders = Physics.OverlapSphere(sourcePlayer.transform.position, 4);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Player otherPlayer = hitColliders[i].GetComponent<Player>();
            if (otherPlayer && otherPlayer != thisPlayer)
            {
                if (thisPlayer.PlayerHeroData.m_heroType != otherPlayer.PlayerHeroData.m_heroType)
                {
                    playersList.Add(otherPlayer);
                }
            }
            i++;
        }
        Debug.Log(playersList.Count);

        if (playersList.Count > 0)
        {
            foreach (Player item in playersList)
            {
                item.CurrentMovementSpeed /= 0.5f;
            }
        }
        yield return new WaitForSeconds(AllSkillsData[thisPlayer.PlayerHeroData.m_skillID].m_duration);

        if (playersList.Count > 0)
        {
            foreach (Player item in playersList)
            {
                item.CurrentMovementSpeed *= 0.5f;
            }
        }
    }

    IEnumerator ActivateAlmightyPush(GameObject sourcePlayerGO, int skillID)
    {
        Skill thisSkill = GetSkillOfType(SkillType.AlmightyPush);
        sourcePlayerGO.GetComponent<Player>().IsImmune = true;
        sourcePlayerGO.GetComponent<Player>().MovementAllowed = false;

        GameObject tempCopy = Instantiate(thisSkill.m_skillPrefab,Vector3.zero, sourcePlayerGO.transform.rotation);
        tempCopy.transform.SetParent(sourcePlayerGO.transform);
        tempCopy.transform.localPosition = thisSkill.m_positionToSpawnAt;
        if (sourcePlayerGO.GetComponent<Player>().isLocalPlayer)
            LobbyManager.Instance.NetworkManager.SpawnGO(tempCopy);

        float timer = AllSkillsData[skillID].m_duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            sourcePlayerGO.transform.position += sourcePlayerGO.transform.forward * Time.deltaTime * 30;
            yield return null;
        }

        Destroy(tempCopy.gameObject);
        sourcePlayerGO.GetComponent<Player>().MovementAllowed = true;
        sourcePlayerGO.GetComponent<Player>().IsImmune = false;
    }

    Skill GetSkillOfType(SkillType type)
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

public enum SkillType { AfterImage, ShieldDash, AlmightyPush}
