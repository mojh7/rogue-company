using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacteristicsAsset
{
    public class CharacteristicsSystem : MonoBehaviourSingleton<CharacteristicsSystem>
    {
        #region variables
        [SerializeField] private GameObject characteristicsUI;
        #endregion

        #region get / set
        #endregion

        #region unityFunc
        private void Awake()
        {
            characteristicsUI.SetActive(false);
        }
        #endregion

        #region func
        public void OpenCharacteristicUI()
        {
            characteristicsUI.SetActive(true);
            AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        }

        public void CloseCharacteristicUI()
        {
            characteristicsUI.SetActive(false);
            AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        }
        #endregion

        #region coroutine
        #endregion
    }
}
