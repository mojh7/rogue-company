using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject achievementsUI;

    // Use this for initialization
    void Awake ()
    {
        achievementsUI.SetActive(false);	
	}

    public void OpenAchievementUI()
    {
        achievementsUI.SetActive(true);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void CloseAchievementUI()
    {
        achievementsUI.SetActive(false);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }
}
