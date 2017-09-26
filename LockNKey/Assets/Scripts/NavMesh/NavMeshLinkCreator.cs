using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshLinkCreator : MonoBehaviour {

    [SerializeField]
    private List<GameObject> m_islandsList;

    private List<GameObject> m_linkPointsList = new List<GameObject>();

    // Use this for initialization
    void Start () {
        AddLinkPointsOnEdge();
    }

    void AddLinkPointsOnEdge()
    {
        float minDistForPoints = 0.5f;
        Vector3 direction;
        for (int i = 0; i < m_islandsList.Count; i++)
        {
            GameObject navMeshHolder = m_islandsList[i].transform.Find("NavLinkEdges").gameObject;

            for (int j = 0; j < navMeshHolder.transform.childCount; j++)
            {
                if (navMeshHolder.transform.GetChild(j).tag == "NoNavMeshLink")
                    continue ;

                Vector3 pos1 = navMeshHolder.transform.GetChild(j).position;
                Vector3 pos2 = navMeshHolder.transform.GetChild((j == navMeshHolder.transform.childCount - 1) ? 0 : j + 1).position;

                float angle = 360 - GetAngle.Angle(pos1, pos2);

                direction = (pos2 - pos1).normalized;

                int pointsNo = (int)(Vector3.Distance(pos1, pos2) / minDistForPoints);

                for (int k = 1; k <= pointsNo; k++)
                {
                    GameObject tempGo = new GameObject();
                    tempGo.transform.position = pos1 + direction * minDistForPoints * k;
                    tempGo.transform.localEulerAngles = Vector3.up * angle;
                    tempGo.transform.SetParent(navMeshHolder.transform.GetChild(j).transform);
                    m_linkPointsList.Add(tempGo);
                }
            }
        }
        AddLinkOnPoints();
    }

    void AddLinkOnPoints()
    {
        for (int i = 0; i < m_linkPointsList.Count; i++)
        {
            m_linkPointsList[i].AddComponent<NavMeshLink>();
            NavMeshLink currentLink = m_linkPointsList[i].GetComponent<NavMeshLink>();
            currentLink.startPoint = Vector3.zero;
            currentLink.endPoint = Vector3.forward * 2 - Vector3.up * 7;
        }
    }
}
