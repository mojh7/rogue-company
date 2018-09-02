using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviourSingleton<Logo> {

    public Image bridgeLogoImage;
    public Image backGround;
    public Image teamLogoImage;
    [SerializeField] private Sprite[] logoSprite;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            LoadTitle();
    }

    void LoadTitle()
    {
        GameStateManager.Instance.LoadTitle();
    }
    public void LoadLogo()
    {
        StartCoroutine(FadeLogo(bridgeLogoImage));
    }
    IEnumerator FadeLogo(Image image)
    {
        if (image != null)
        {
            for (int i = 0; i <= 30; i++)
            {
                image.color = new Color(1, 1, 1, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
            for (int i = 10; i >= 0; i--)
            {
                image.color = new Color(1, 1, 1, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
            if (image != teamLogoImage)
            {
                StartCoroutine(AnimationLogo(teamLogoImage));
            }
            else
                LoadTitle();
        }
    }

    // 수정 필요함.. 리소스 크기대로 잘라주세요 흑흑흑..
    IEnumerator AnimationLogo(Image image)
    {
        if (image != null)
        {
            for (int i = 0; i < 10; i++)
            {
                backGround.color = new Color(0, 0, 0, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
            image.gameObject.SetActive(true);
            backGround.color = new Color(0, 0, 0);
            for (int i = 0; i < logoSprite.Length; i++)
            {
                image.sprite = logoSprite[i];
                yield return YieldInstructionCache.WaitForSeconds(0.1f);
            }
            image.gameObject.SetActive(false);
            for (int i = 10; i >= 0; i--)
            {
                backGround.color = new Color(0, 0, 0, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
        }
        if (image == teamLogoImage)
            LoadTitle();
    }
}
