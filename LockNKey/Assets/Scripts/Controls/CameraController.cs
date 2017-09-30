using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject m_playerGO;

    [SerializeField]
    private Vector3 m_cameraOffset;

    [SerializeField]
    private float m_smoothTime = 0.3f;

    private Vector3 m_smoothVelocity = Vector3.zero;

    public GameObject PlayerGO
    {
        get
        {
            return m_playerGO;
        }

        set
        {
            m_playerGO = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(PlayerGO != null)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, PlayerGO.transform.position + m_cameraOffset, ref m_smoothVelocity, m_smoothTime);
        }
	}
}
