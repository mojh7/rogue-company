using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviourSingleton<Title> {

    public Image bridgeLogoImage;
    public Image teamLogoImage;
    public GameObject titleObj;
    public GameObject restartButton;
    bool isSaved = true;

    public void LoadLogoTitle()
    {
        StartCoroutine(FadeLogo(bridgeLogoImage));
    }
    public void StartSelect()
    {
        SceneDataManager.SetNextScene("SelectScene");
        SceneManager.LoadScene("LoadingScene");
    }
    public void StartSaved()
    {
        SceneDataManager.SetNextScene("InGameScene");
        SceneManager.LoadScene("LoadingScene");
    }
    void ShowTitle()
    {
        titleObj.SetActive(true);
        if (isSaved)
            restartButton.SetActive(true);
        else
            restartButton.SetActive(false);
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
            if (image == teamLogoImage)
                ShowTitle();
            for (int i = 10; i >= 0; i--)
            {
                image.color = new Color(1, 1, 1, (float)i / 10);
                yield return YieldInstructionCache.WaitForSeconds(0.05f);
            }
            if (image != teamLogoImage)
                StartCoroutine(FadeLogo(teamLogoImage));
        }
    }
}
