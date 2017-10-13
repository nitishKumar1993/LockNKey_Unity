using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject m_playerGO;
    private Player m_player;

    [SerializeField]
    private Vector3 m_cameraOffset;

    [SerializeField]
    private float m_smoothTime = 0.3f;

    private Vector3 m_smoothVelocity = Vector3.zero;
    private Vector3 m_lastPlayerPos;

    public GameObject PlayerGO
    {
        get
        {
            return m_playerGO;
        }

        set
        {
            m_playerGO = value;
            m_player = m_playerGO.GetComponent<Player>();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(PlayerGO != null)
        {
            Vector3 movePos = !m_player.IsDead ? PlayerGO.transform.position + m_cameraOffset : m_lastPlayerPos + m_cameraOffset;
            this.transform.position = Vector3.SmoothDamp(this.transform.position, movePos, ref m_smoothVelocity, m_smoothTime);
            m_lastPlayerPos = !m_player.IsDead ? PlayerGO.transform.position : m_lastPlayerPos;
        }
	}
}
