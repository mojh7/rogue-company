using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Logo : MonoBehaviourSingleton<Logo>
{
    [SerializeField] private Image bridgeLogoImg;
    [SerializeField] private Image teamLogoImg;
    [SerializeField] private RectTransform teamLogoRectTransform;
    [SerializeField] private Image fadeImg;

    private void Start()
    {
        GameDataManager.Instance.LoadInitialUserData();
        GameDataManager.Instance.LoadInitialSettingData();
        LoadLogo();
        AudioManager.Instance.PlaySound("bridgeOpeningSound", SOUNDTYPE.UI);
        GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.LOGO);
    }

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
        StartCoroutine(FadeLogo(bridgeLogoImg));
        StartCoroutine(AnimationLogo(teamLogoImg));
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
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            for (int i = 10; i >= 0; i--)
            {
                image.color = new Color(1, 1, 1, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.08f);
            }
        }
    }

    IEnumerator AnimationLogo(Image image)
    {
        if (image != null)
        {
            Vector2 teamLogoSize = new Vector2(600, 0);
            Vector3 teamLogoRotation = new Vector3(0, 0, 0);
            image.gameObject.SetActive(true);
            for (int i = 0; i < 16; i++)
            {
                image.color = new Color(1, 1, 1, (float)i / 15);
                teamLogoRectTransform.sizeDelta = teamLogoSize;
                teamLogoRectTransform.rotation = Quaternion.Euler(teamLogoRotation);
                teamLogoRotation.z = -2f + 4f * ((i/2) % 2);
                teamLogoSize.y += 250f / 15;
                yield return YieldInstructionCache.WaitForSeconds(0.02f);
            }
            yield return YieldInstructionCache.WaitForSeconds(2f);
            for (int i = 20; i >= 0; i--)
            {
                image.color = new Color(1, 1, 1, (float)i / 20);
                fadeImg.color = new Color(0, 0, 0, (float)(20-i) / 20);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
        }
        if (image == teamLogoImg)
            LoadTitle();
    }
}
