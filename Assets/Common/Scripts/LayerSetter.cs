using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSetter : MonoBehaviour {

    private void Start()
    {
        Camera cam = transform.parent.GetComponent<Camera>();
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        transform.localScale = new Vector3(width, height);
    }
}
