using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {

    public CircleCollider2D a;

	// Use this for initialization
	void Start () {
        Debug.Log(transform.position);
        Debug.Log(a.transform.position);
        Debug.Log("x, y = " + a.transform.position.x + ", " + a.transform.position.y);
        Debug.Log(a.offset);
        Debug.Log((Vector2)a.transform.position + a.offset);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
