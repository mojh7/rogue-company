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
    private bool canLoadData = true;
    private CharacterInfo.AimType aimType;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        settingObj.SetActive(false);
        creditUI.SetActive(false);
        if (GameDataManager.Instance.LoadData(GameDataManager.UserDataType.SETTING))
        {
            aimType = GameDataManager.Instance.GetAimType();
            if (CharacterInfo.AimType.AUTO != aimType)
            {
                aimToggles[0].isOn = false;
                aimToggles[(int)aimType].isOn = true;
            }
            musicVolumeSlider.value = GameDataManager.Instance.GetMusicVolume();
            soundVolumeSlider.value = GameDataManager.Instance.GetSoundVolume();
            SetMusicVolume();
            SetSoundVolume();
        }
        canLoadData = false;
        SetPlayerAim(aimType);
    }

    public void OpenCredit()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        creditUI.SetActive(true);
    }

    public void ExitCredit()
    {
        creditUI.SetActive(false);
        ExitSetting();
    }

    public void PrevCreadit()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        creditUI.SetActive(false);
    }

    public void OpenSetting()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        settingObj.SetActive(true);
    }

    public void ExitSetting()
    {
        settingObj.SetActive(false);
        ToggleMenu();
    }

    public void PrevSetting()
    {
        AudioManager.Instance.PlaySound(0, SOUNDTYPE.UI);
        settingObj.SetActive(false);
    }

    private void ToggleMenu()
    {
        switch (GameStateManager.Instance.GetGameScene())
        {
            case GameStateManager.GameScene.LOBBY:
                LobbyPauseMenu.Instance.ToggleMenu();
                break;
            case GameStateManager.GameScene.IN_GAME:
            case GameStateManager.GameScene.BOSS_RUSH:
                UIManager.Instance.ToggleMenu();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Aim 설정 및 설정 값 세이브
    /// </summary>
    /// <param name="i"></param>
    public void SettingPlayerAim(int type)
    {
        // data load 전에 toggle false 되면서 실행되며 꼬이는 것 방지
        if (canLoadData || false == aimToggles[type].isOn)
            return;
        SetPlayerAim((CharacterInfo.AimType)type);
        SavePlayerAim();
    }

    private void SetPlayerAim(CharacterInfo.AimType type)
    {
        aimType = type;
        if (GameStateManager.Instance.IsInGame())
        {
            Debug.Log("z : " + PlayerManager.Instance.GetPlayer());
            PlayerManager.Instance.GetPlayer().SetAimType(aimType);
        }
    }

    private void SavePlayerAim()
    {
        GameDataManager.Instance.SetAimType(aimType);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }

    public void SettingMusicVolume()
    {
        if (null == AudioManager.Instance)
            return;
        SetMusicVolume();
        SaveMusicVolume();
    }

    private void SetMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
    }

    private void SaveMusicVolume()
    {
        GameDataManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }

    public void SettingSoundVolume()
    {
        if (null == AudioManager.Instance)
            return;
        SetSoundVolume();
        SaveSoundVolume();
    }

    private void SetSoundVolume()
    {
        AudioManager.Instance.SetSoundVolume(soundVolumeSlider.value);
    }

    private void SaveSoundVolume()
    {
        GameDataManager.Instance.SetSoundVolume(soundVolumeSlider.value);
        GameDataManager.Instance.Savedata(GameDataManager.UserDataType.SETTING);
    }
}
