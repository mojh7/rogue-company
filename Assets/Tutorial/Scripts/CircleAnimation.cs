using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleAnimation : MonoBehaviour {
    // 포커싱되게 하기
    private float width;
    private float height;
    private RectTransform rect;
    private bool isTrue;

    private void Awake()
    {
        rect = this.gameObject.GetComponent<RectTransform>();
        width = rect.rect.width;
        height = rect.rect.height;
        isTrue = false;

        StartCoroutine("AnimationCircle");
    }

    private void Update()
    {
        if (isTrue)
            this.gameObject.SetActive(false);
    }

    public IEnumerator AnimationCircle()
    {
        for (int i = 10; i > 0; i--)
        {
            float _width = width * 0.1f * i;
            float _height = height * 0.1f * i;
            Debug.Log(_width + " " + _height);
            rect.sizeDelta = new Vector2(_width, _height);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        for (int i = 10; i > 0; i--)
        {
            float _width = width * 0.1f * i;
            float _height = height * 0.1f * i;
            Debug.Log(_width + " " + _height);
            rect.sizeDelta = new Vector2(_width, _height);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        isTrue = true;
        yield return null;
    }
}
