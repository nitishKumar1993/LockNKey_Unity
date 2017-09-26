using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [SerializeField]
    private float m_gameWaterHeight;

    [SerializeField]
    private float m_gameDurationInMins;

    [SerializeField]
    private List<GameObject> m_islandsList = new List<GameObject>();

    public float GameWaterHeight
    {
        get
        {
            return m_gameWaterHeight;
        }

        set
        {
            m_gameWaterHeight = value;
        }
    }
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Instance = this;

        StartCoroutine(RemoveIslands());
    }
	
	IEnumerator RemoveIslands()
    {
        float totalGameDuration = m_gameDurationInMins * 60;
        float currentGameDuration = totalGameDuration / m_islandsList.Count; 

        while (totalGameDuration > 0)
        {
            totalGameDuration -= Time.deltaTime;
            currentGameDuration -= Time.deltaTime;
            if (currentGameDuration <= 0)
            {
               // Debug.Log("Remove islands");
                m_islandsList[m_islandsList.Count - 1].GetComponent<IslandBehaviour>().RemoveIsland();

                m_islandsList.RemoveAt(m_islandsList.Count - 1);
                currentGameDuration = totalGameDuration / m_islandsList.Count;
            }
            yield return null;
        }
    }
}
