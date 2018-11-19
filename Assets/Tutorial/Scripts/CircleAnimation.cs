using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleAnimation : MonoBehaviourSingleton<CircleAnimation> {
    // 포커싱되게 하기
    private float width;
    private float height;
    private RectTransform rect;
    [HideInInspector] public bool isTrue = false;

    private void Start()
    {
        rect = this.gameObject.GetComponent<Image>().GetComponent<RectTransform>();
        width = rect.rect.width;
        height = rect.rect.height;

        StartCoroutine("AnimationCircle");
    }

    private void Update()
    {
        
    }

    public IEnumerator AnimationCircle()
    {
        for (int i = 10; i > 0; i--)
        {
            float _width = width * 0.1f * i;
            float _height = height * 0.1f * i;
            rect.sizeDelta = new Vector2(_width, _height);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        for (int i = 10; i > 0; i--)
        {
            float _width = width * 0.1f * i;
            float _height = height * 0.1f * i;
            rect.sizeDelta = new Vector2(_width, _height);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        if (isTrue)
        {
            TutorialUIManager.Instance.FirstTest();
        }
        Destroy(this.gameObject, 0.5f);
    }
}
