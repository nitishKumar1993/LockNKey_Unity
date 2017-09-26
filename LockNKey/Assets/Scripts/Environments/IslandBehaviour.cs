using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBehaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public void RemoveIsland()
    {
        StartCoroutine(ShakeAndRemove());
    }

    IEnumerator ShakeAndRemove()
    {
        float shakeTime = 1.5f;
        float moveTime = 3;
        Vector3 movePostion = this.transform.position - Vector3.up * 5;

        //iTween.ShakePosition(this.gameObject, Vector3.up, shakeTime);
        yield return new WaitForSeconds(shakeTime);
        //iTween.MoveTo(this.gameObject, movePostion, moveTime);
        yield return new WaitForSeconds(moveTime);
        this.gameObject.SetActive(false);
    }
}
