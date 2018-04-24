using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleWithCamera : MonoBehaviour {

    public int targetWidth = 1280;
    public int pixelsToUnits = 32;

    // Update is called once per frame
    void Awake () {
        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);

        GetComponent<Camera>().orthographicSize = height / pixelsToUnits / 2;
    }
}
