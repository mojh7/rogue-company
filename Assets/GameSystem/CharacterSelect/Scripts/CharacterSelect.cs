using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{

    [SerializeField]
    private GameObject selectWindowUI;



	// Use this for initialization
	void Start () {
		
	}

    #region func

    public void OpenCharacterSelectWindow()
    {
        selectWindowUI.SetActive(true);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void CloseCharacterSelectWindow()
    {
        selectWindowUI.SetActive(false);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    #endregion
}
