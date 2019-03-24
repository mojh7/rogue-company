using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneUI : MonoBehaviourSingleton<CutSceneUI> {

    [SerializeField] private GameObject bossPanel;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image chImage;
    [SerializeField] private Text text;
    [Space]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Animator storyAnim;

    /// <summary>
    /// Show cutscene
    /// </summary>
    /// <param name="_bgDir">배경화면이 날아오는 방향</param>
    /// <param name="_chDir">캐릭터이미지가 날아오는 방향</param>
    /// <param name="_textDir">문자가 날아오는 방향</param>
    public void BossCutScene(Vector2 _bgDir, Vector2 _chDir, Vector2 _textDir)
    {
        UIManager.Instance.SetActivedBool(true);
        Vector2 bgDest = bgImage.rectTransform.localPosition;
        Vector2 chDest = chImage.rectTransform.localPosition;
        Vector2 textDest = text.rectTransform.localPosition;
        bossPanel.SetActive(true);
        _bgDir.Normalize();
        _chDir.Normalize();
        _textDir.Normalize();
        int width = Screen.width;
        int height = Screen.height;
        bgImage.rectTransform.localPosition = new Vector2(_bgDir.x * width, _bgDir.y * height);
        chImage.rectTransform.localPosition = new Vector2(_chDir.x * width, _chDir.y * height);
        text.rectTransform.localPosition = new Vector2(_textDir.x * width, _textDir.y * height);
        MoveToTarget(bgImage.transform, bgDest, 0.5f);
        MoveToTarget(chImage.transform, chDest, 0.6f);
        MoveToTarget(text.transform, textDest, 0.7f);
    }
    public void SetCharacter(Sprite _sprite)
    {
        chImage.sprite = _sprite;
    }
    public void SetBackGround(Sprite _sprite)
    {
        bgImage.sprite = _sprite;
    }
    public void SetText(string _str)
    {
        text.text = _str;
    }
        
    void MoveToTarget(Transform _transform, Vector2 _target, float _duration)
    {
        StartCoroutine(CoroutineMoveToTarget(_transform, _target, _duration));
    }
    IEnumerator CoroutineMoveToTarget(Transform _transform, Vector2 _target, float _duration)
    {
        float elapsed = 0.0f;
        Vector2 start = _transform.localPosition;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            _transform.localPosition = Vector2.Lerp(start, _target, elapsed / _duration);

            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

    }

    public void ShowCutScene(int floor)
    {
        UIManager.Instance.SetActivedBool(true);
        storyPanel.SetActive(true);
        storyAnim.SetInteger("Floor", floor);
    }

    public void Hide() { bossPanel.SetActive(false); storyPanel.SetActive(false); UIManager.Instance.SetActivedBool(false); }
}
