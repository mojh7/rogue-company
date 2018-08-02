using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour {

    public float time = 1;
	// Use this for initialization
	void Start () {

        StartCoroutine(DestroyEffect_Fuc());
	}
    IEnumerator DestroyEffect_Fuc()
    {
        yield return new WaitForSeconds (time);
        DestroyObject(this.gameObject);


       
    }
}
