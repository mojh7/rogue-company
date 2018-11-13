using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearRoomUI : MonoBehaviour {

    Color white = Color.white, clear = Color.clear;
    Image image;

    private void Awake()
    {
        image = this.GetComponent<Image>();
    }

    public void Clear()
    {
        StartCoroutine(CoroutineColorLerp());
    }

    IEnumerator CoroutineColorLerp()
    {
        UtilityClass.ColorLerp(this, image, clear, white, .3f);
        yield return YieldInstructionCache.WaitForSeconds(.3f);
        UtilityClass.ColorLerp(this, image, white, clear, .3f);
        yield return YieldInstructionCache.WaitForSeconds(.3f);
        UtilityClass.ColorLerp(this, image, clear, white, .3f);
        yield return YieldInstructionCache.WaitForSeconds(.5f);
        image.color = clear;
    }

}
