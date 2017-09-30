using UnityEngine;
using System.Collections;

public class WaterBehaviour : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;

    public bool m_notifyCollision = false;

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (this.GetComponent<Renderer>().enabled)
        {
            this.GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_notifyCollision)
        {
            if (other.GetComponent<Player>())
            {
                other.GetComponent<Player>().OnEnterWater();
            }
        }
    }
}