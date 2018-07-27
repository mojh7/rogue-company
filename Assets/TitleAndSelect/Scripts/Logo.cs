using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviourSingleton<Logo> {

    public Image bridgeLogoImage;
    public Image teamLogoImage;

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
                StartCoroutine(FadeLogo(teamLogoImage));
            else
                LoadTitle();
        }
    }
}
