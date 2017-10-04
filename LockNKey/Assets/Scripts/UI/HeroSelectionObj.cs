using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelectionObj : MonoBehaviour {

    HeroData m_heroData = new HeroData();

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnSelected (bool value) {
        if (value)
            LobbyManager.Instance.OnHeroSelected(m_heroData);
	}

    public void Init(HeroData heroData)
    {
        m_heroData = heroData;
    }


}
