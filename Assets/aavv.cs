using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aavv : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LayerMask layerMask = (1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Wall"));
        if (!Physics2D.OverlapCircle(transform.position, 0.5f, layerMask))
        {
            Debug.DrawLine(transform.position, Vector3.zero, Color.cyan, 100);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
