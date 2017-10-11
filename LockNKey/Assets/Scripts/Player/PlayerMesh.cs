using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMesh : MonoBehaviour
{

    [SerializeField]
    private Material m_frozenMaterial;

    public GameObject[] m_meshArray;

    public List<Material> m_meshMaterialsList = new List<Material>();

    private void Start()
    {
        for (int i = 0; i < m_meshArray.Length; i++)
        {
            if (m_meshArray[i].GetComponent<SkinnedMeshRenderer>())
            {
                m_meshMaterialsList.Add(new Material(m_meshArray[i].GetComponent<SkinnedMeshRenderer>().materials[0]));
            }
            else
                Debug.Log("Couldnt find material for mesh :" + m_meshArray[i].name + " On " + this.name);
        }
    }

    public void Freeze(bool action)
    {
        for (int i = 0; i < m_meshArray.Length; i++)
        {
            if (m_meshArray[i].GetComponent<SkinnedMeshRenderer>())
            {
                Material tempFrozenMat = new Material(m_frozenMaterial);
                m_meshArray[i].GetComponent<SkinnedMeshRenderer>().material = action ? tempFrozenMat : m_meshMaterialsList[i];
            }
            else
                Debug.Log("Couldnt find material for mesh :" + m_meshArray[i].name + " On " + this.name);
        }
    }
}
