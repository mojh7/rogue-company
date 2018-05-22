using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 0517 모장현
 * Music - 배경음악
 * Sound - 효과음
 * 
 * AudioManager Class
 * Music과 Sound 둘 다 관리하는 Class로 싱글톤을 사용합니다. 
 * 
 *
 * [예정]
 *  - 음악 참조 뭐로 할지?(index, string, enum ??)
 *  
 */

/* http://cafe.naver.com/unityhub
 * 제목 : 첫작품에 사용된 배경음악/사운드매니저 공유해봅니다
 */


public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    private MusicController musicController;
    private SoundController soundController;


    [Header("디버그용, 배경음악 안 듣고 싶을 때 꺼주세요")]
    public bool playMusic;
    [Header("디버그용, 배경음악 재생 번호")]
    public int playMusicId;

    void Awake()
    {
        musicController = GetComponentInChildren<MusicController>();
        soundController = GetComponentInChildren<SoundController>();
    }


    void Start()
    {
        // for test
        if (playMusic)
        {
            if (playMusicId == -1)
                PlayMusic(Random.Range(0, 3));
            else
                PlayMusic(playMusicId);
        }
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
        musicController.Play();
    }

    public void PlayMusic(int clipindex, bool ignoresame = false)
    {
        musicController.Play(clipindex, ignoresame);
    }


    /*
    // 일반 사운드 재생
    */
    public void PlaySound(int clipindex, SoundController.SoundType soundtype = SoundController.SoundType.GAME)
    {
        soundController.Play(clipindex, soundtype);
    }
}
