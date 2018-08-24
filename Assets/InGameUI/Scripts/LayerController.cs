using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerController : MonoBehaviourSingleton<LayerController> {
    Color clear;
    [SerializeField]
    private Image attackedLayer;

    private bool isFlashLayer;


    private void Awake()
    {
        clear = Color.clear;
    }

    public void FlashAttackedLayer(float time)
    {
        if (isFlashLayer || time <= 0)
            return;
        isFlashLayer = true;

        StartCoroutine(CoroutineFlashLayer(time, attackedLayer));
    }

    IEnumerator CoroutineFlashLayer(float time, Image layer)
    {
        attackedLayer.gameObject.SetActive(true);
        Color src = attackedLayer.color;
        float startTime = Time.time;
        float tempElapsedTime = 0;
        while (time >= tempElapsedTime)
        {
            tempElapsedTime = Time.time - startTime;

            layer.color = Color.Lerp(src, clear, tempElapsedTime / time);

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        attackedLayer.gameObject.SetActive(false);
        attackedLayer.color = src;
        isFlashLayer = false;
    }
}
