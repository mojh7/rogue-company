using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class GameSettingData
{
    private CharacterInfo.AimType aimType;
    private float musicVolume;
    private float soundVolume;
    public GameSettingData()
    {
        aimType = CharacterInfo.AimType.AUTO;
        musicVolume = 1f;
        soundVolume = 1f;
    }
    #region getter
    public CharacterInfo.AimType GetAimType() { return aimType; }
    public float GetMusicVolume() { return musicVolume; }
    public float GetSoundVolume() { return soundVolume; }
    #endregion

    #region setter
    public void SetAimType(CharacterInfo.AimType _aimType) { aimType = _aimType; }
    public void SetMusicVolume(float _musicVolume) { musicVolume = _musicVolume; }
    public void SetSoundVolume(float _soundVolume) { soundVolume = _soundVolume; }
    #endregion
}
