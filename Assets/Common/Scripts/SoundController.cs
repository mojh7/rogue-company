using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{

    //[Header("[PlayerPrefs Key]")]
    //[SerializeField] private string saveKey = "Option_Sound";

    [Header("[GUI Clips]")]
    [SerializeField] private AudioClip[] guiClips;

    [Header("[Game Clips]")]
    [SerializeField] private AudioClip[] gameClips;

    private AudioSource audioSource;

    public enum SoundType { GAME, GUI };

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 사운드 On/Off 여부 확인, 임시로 true
    public bool IsEnableSound()
    {
        return true;
        //return PlayerPrefs.GetInt(saveKey, 1) == 1 ? true : false;
    }

    /*
    // 사운드 설정 저장
    public void EnableSound(bool enable)
    {
        PlayerPrefs.SetInt(saveKey, enable ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 사운드 On/Off 토글
    public void ToggleSound()
    {
        EnableSound(!IsEnableSound());
    }*/



    // 사운드 재생
    public void Play(int clipindex, SoundType soundtype)
    {
        if (clipindex < 0) return;

        AudioClip _clip = null;

        if (soundtype == SoundType.GUI) _clip = guiClips[clipindex];
        if (soundtype == SoundType.GAME) _clip = gameClips[clipindex];

        if (_clip == null)
            return;

        audioSource.PlayOneShot(_clip);
    }
}
