using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * AudioManager Class
 * Music과 Sound 둘 다 관리하는 Class로 싱글톤을 사용합니다. 
 */
/* http://cafe.naver.com/unityhub
 * 제목 : 첫작품에 사용된 배경음악/사운드매니저 공유해봅니다
 */

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    private MusicController musicController;
    private SoundController soundController;


    [Header("디버그용, 배경음악 안 듣고 싶을 때 꺼주세요")]
    public bool canPlayMusic;


    void Awake()
    {
        musicController = MusicController.Instance;
        soundController = SoundController.Instance;
    }

    /*
    // 음악 설정
    */
    public bool IsEnableMusic()
    {
        return musicController.IsEnableMusic();
    }

    /*
    public void EnableMusic(bool enable)
    {
        musicController.EnableMusic(enable);
    }

    public void ToggleMusic()
    {
        musicController.ToggleMusic();
    }
    */

    /*
    // 일반 사운드 설정
    */
    public bool IsEnableSound()
    {
        return soundController.IsEnableSound();
    }

    /*
    public void EnableSound(bool enable)
    {
        soundController.EnableSound(enable);
    }

    public void ToggleSound()
    {
        soundController.ToggleSound();
    }
    */


    /*
    // 음악 재생 제어
    */
    public void PauseMusic()
    {
        musicController.Pause();
    }

    public void ResumeMusic()
    {
        musicController.Resume();
    }

    public void StopMusic()
    {
        musicController.Stop();
    }

    public void PlayMusic()
    {
        if (false == canPlayMusic)
            return;
        musicController.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicController.SetVolume(volume);
    }

    public void SetSoundVolume(float volume)
    {
        soundController.SetVolume(volume);
    }

    /// <summary>
    /// 배경음악 실행
    /// </summary>
    /// <param name="clipindex"></param>
    /// <param name="ignoresame"></param>
    public void PlayMusic(int musicIndex, bool ignoresame = false)
    {
        if (false == canPlayMusic)
            return;
        musicController.Play(musicIndex, ignoresame);
    }

    /// <summary>
    /// 사운드(효과음) 실행
    /// </summary>
    /// <param name="soundIndex">0이상의 사운드 인덱스</param>
    /// <param name="soundtype">출력할 사운드 타입</param>
    public void PlaySound(int soundIndex, SOUNDTYPE soundtype)
    {
        soundController.Play(soundIndex, soundtype);
    }

    public void PlaySound(string soundName, SOUNDTYPE soundtype)
    {
        soundController.Play(soundName, soundtype);
    }
}
