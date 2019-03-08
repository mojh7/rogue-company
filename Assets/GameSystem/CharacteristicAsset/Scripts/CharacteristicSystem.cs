using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicSystem : MonoBehaviourSingleton<CharacteristicSystem>
{
    #region variables
    [SerializeField] private GameObject characteristicUI;
    #endregion

    #region get / set
    #endregion

    #region unityFunc
    private void Awake()
    {
        characteristicUI.SetActive(false);
    }
    #endregion

    #region func
    public void OpenCharacteristicUI()
    {
        characteristicUI.SetActive(true);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }

    public void CloseCharacteristicUI()
    {
        characteristicUI.SetActive(false);
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
    }
    #endregion

    #region coroutine
    #endregion
}
