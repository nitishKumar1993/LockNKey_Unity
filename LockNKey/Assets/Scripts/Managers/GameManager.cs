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

    private HeroData[] m_allHeroesData;

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

    public HeroData[] AllHeroesData
    {
        get
        {
            return m_allHeroesData;
        }

        set
        {
            m_allHeroesData = value;
        }
    }

    void Awake()
    {
        Instance = this;

        AllHeroesData = ResourceManager.GetHeroesData();
    }

    void Start()
    {
       // StartCoroutine(RemoveIslands());
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
