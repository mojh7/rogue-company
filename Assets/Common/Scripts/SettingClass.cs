using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingClass : MonoBehaviour
{
    [SerializeField]
    private Toggle[] aimToggles;
    [SerializeField]
    private GameObject settingObj;
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider soundVolumeSlider;
    [SerializeField]
    private GameObject creditUI;

    private CharacterInfo.AimType aimType;

    private void Awake()
    {
        // 인게임씬에서 바로 테스트할 때 적용할 디버깅용
        if("InGameScene" == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.IN_GAME);
        }
        else if("SelectScene" == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            GameStateManager.Instance.SetGameScene(GameStateManager.GameScene.LOBBY);
        }

        creditUI.SetActive(false);
        if (GameDataManager.Instance.LoadData(GameDataManager.UserDataType.SETTING))
        {
            aimType = GameDataManager.Instance.GetAimType();
            musicVolumeSlider.value = GameDataManager.Instance.GetMusicVolume();
            soundVolumeSlider.value = GameDataManager.Instance.GetSoundVolume();
            SettingPlayerAim((int)aimType);
            SettingMusicVolume();
            SettingSoundVolume();
        }
        //aimType = GameDataManager.Instance.GetAimType();
        //aimToggles[(int)aimType].isOn = true;
    }
    /// <summary>
    /// Aim 설정 및 설정 값 세이브
    /// </summary>
    /// <param name="i"></param>
    public void SettingPlayerAim(int i)
    {
        aimType = (CharacterInfo.AimType)i;
        if (GameStateManager.Instance.IsInGame())
        {
            Debug.Log("player aim 설정 : " + aimType);
            PlayerManager.Instance.GetPlayer().SetAimType(aimType);
        }
        GameDataManager.Instance.SetAimType(aimType);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }

    public void SettingMusicVolume()
    {
        if (null == AudioManager.Instance)
            return;
        AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        GameDataManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }

    public void SettingSoundVolume()
    {
        if (null == AudioManager.Instance)
            return;
        AudioManager.Instance.SetSoundVolume(soundVolumeSlider.value);
        GameDataManager.Instance.SetSoundVolume(soundVolumeSlider.value);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }

    //GameObject obBG;
    //GameObject obEf;
    //[HideInInspector] public AudioSource adBG;
    //[HideInInspector] public AudioSource adEf;
    //[SerializeField] private Slider[] sl;

    //private void Awake()
    //{
    //    obBG = GameObject.Find("MusicController");
    //    adBG = obBG.gameObject.GetComponent<AudioSource>();

    //    obEf = GameObject.Find("SoundController");
    //    adEf = obEf.gameObject.GetComponent<AudioSource>();
    //}

    //public void SetMusicControll()
    //{
    //    adBG.volume = sl[0].value;
    //}

    //public void SetSoundControll()
    //{
    //    adEf.volume = sl[1].value;
    //}
}
