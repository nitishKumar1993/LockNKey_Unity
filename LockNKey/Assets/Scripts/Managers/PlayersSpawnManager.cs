using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersSpawnManager : MonoBehaviour
{
    public static PlayersSpawnManager Instance;

    [SerializeField]
    private List<Transform> m_runnersSpawnPoints;
    [SerializeField]
    private List<Transform> m_chasersSpawnPoints;

    int m_lastRunnerMovePos = 0;
    int m_lastChaserMovePos = 0;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    public void SpawnRunner(GameObject PlayerGO)
    {
        PlayerGO.transform.position = m_runnersSpawnPoints[m_lastRunnerMovePos].position;
        m_lastRunnerMovePos++;
        if (m_lastRunnerMovePos >= m_runnersSpawnPoints.Count)
            m_lastRunnerMovePos = 0;
    }

    public void SpawnChaser(GameObject PlayerGO)
    {
        PlayerGO.transform.position = m_chasersSpawnPoints[m_lastChaserMovePos].position;
    }
}
