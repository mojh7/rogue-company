using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 레이저 테스트용

public class LaserTest : MonoBehaviour {

    private LineRenderer lineRenderer;
    private Transform objTransform;
    private Ray2D ray;
    private RaycastHit2D hit;
    private int layerMask;

	// Use this for initialization
	void Start () {
        objTransform = GetComponent<Transform>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.cyan;

        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;

        layerMask = 1 << LayerMask.NameToLayer("Wall");
        /*
         * 복수 레이어 비트 마스크, http://tech031.blogspot.kr/2015/08/unity.html
         * int layermask = (1<<LayerMask.NameToLayer(“1번 레이어이름”) | (1<<LayerMask.NameToLayer(“5번 레이어이름”); 
         */
         
    }

    // Update is called once per frame
    void Update () {
        objTransform.position = Player.Instance.GetPosition();
        // Physics2D.BoxCast
        hit = Physics2D.Raycast(objTransform.position, Player.Instance.GetRecenteInputVector(), 100f, layerMask);
        
        if(hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            lineRenderer.SetPosition(0, objTransform.position);
            lineRenderer.SetPosition(1, hit.point);
            //hit.transform.position
        }
    }
}
