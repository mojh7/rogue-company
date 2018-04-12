using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour {

    public Image logoImage;

    void LoadLogoTitle()
    {
        StartCoroutine(FadeLogo(logoImage));
    }
    IEnumerator FadeLogo(Image image)
    {
        for(int i = 0; i <= 30; i++)
        {
            image.color = new Color(1, 1, 1, (float)i / 10);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }

        for (int i = 10; i >= 0; i--)
        {
            image.color = new Color(1, 1, 1, (float)i / 10);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }
	// Use this for initialization
	void Start () {
        LoadLogoTitle();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
