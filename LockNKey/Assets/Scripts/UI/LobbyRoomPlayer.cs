using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomPlayer : MonoBehaviour {
    [SerializeField]
    private Text m_nameText;

    private string m_address;
    private string m_name;

    // Use this for initialization
    public void Init (string data) {
        string[] dataArray = data.Split(',');
        if (dataArray.Length == 2)
        {
            m_address = dataArray[0];
            m_name = dataArray[1];

            m_nameText.text = m_name;
        }
        else Debug.Log("Error Parsing Data");
	}

    public void JoinGame()
    {
        Debug.Log("Join :" + m_address);
        LobbyManager.Instance.JoinGame(m_address);
    }
}
