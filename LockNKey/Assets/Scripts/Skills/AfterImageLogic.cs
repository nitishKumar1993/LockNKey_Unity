using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageLogic : MonoBehaviour {

	public void Init (GameObject playerGO) {
        StartCoroutine(CreateAfterImageCR(playerGO));
	}

    IEnumerator CreateAfterImageCR(GameObject playerGO)
    {
        GameObject meshGO = Instantiate(playerGO.transform.Find("Mesh").GetChild(0).gameObject, this.transform.GetChild(0));
        meshGO.GetComponent<Animator>().SetFloat("WalkSpeed", 1);

        float duration = 3;

        while (duration > 0)
        {
            duration -= Time.deltaTime;
            this.transform.position += (this.transform.forward * 0.1f);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
