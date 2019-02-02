using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviourSingleton<MusicController>
{
    //[Header("[PlayerPrefs Key]")]
    //[SerializeField] private string saveKey = "Option_Music";

    [Header("[Clips]")]
    [SerializeField] private AudioClip[] clips;

    AudioSource audioSource;
    private float volume = 1f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private int currentClipIndex = -1;

    public void SetVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;
    }

    // 음악 On/Off 여부 확인
    public bool IsEnableMusic()
    {
        return true;
        //return PlayerPrefs.GetInt(saveKey, 1) == 1 ? true : false;
    }

    /*
    // 음악 설정 저장
    public void EnableMusic(bool enable)
    {
        PlayerPrefs.SetInt(saveKey, enable ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 음악 On/Off 토글
    public void ToggleMusic()
    {
        EnableMusic(!IsEnableMusic());
    }*/



    // 일시 정지
    public void Pause()
    {
        audioSource.Pause();
    }

    // 일시 정지 해제
    public void Resume()
    {
        audioSource.UnPause();
    }

    // 정지
    public void Stop()
    {
        audioSource.Stop();
    }

    // 재생
    public void Play()
    {
        Play(currentClipIndex);
    }

    public void Play(int clipindex, bool ignoresame = false)
    {
        if (clipindex == -1)
            return;

        // 음악이 재생중이지 않다면 재생
        // 음악이 재생중이면 재생중인 클립과 요청한 클립이 다르거나 동일 클립 재생이 가능하면 재생
        if (audioSource.isPlaying)
        {
            if (currentClipIndex != clipindex || ignoresame)
                StartCoroutine(PlayNewMusic(clipindex));
        }
        else
            StartCoroutine(PlayNewMusic(clipindex));
    }

    IEnumerator PlayNewMusic(int clipindex)
    {
        // 재생중인 소스의 볼륨을 단계적으로 줄인다.
        while (audioSource.volume >= this.volume * 0.1f)
        {
            audioSource.volume -= this.volume * 0.1f;
            yield return new WaitForSeconds(0.05f);
        }

        // 볼륨이 기준치에 도달하면 오디오소스를 정지하고 세로운 클립을 세팅한다.
        audioSource.Stop();
        currentClipIndex = clipindex;
        audioSource.clip = clips[clipindex];

        if (IsEnableMusic())
            audioSource.Play();

        audioSource.volume = this.volume;
    }


    // 현재 상태에 따라 음악을 재생/정지 시킨다.
    // 옵션등에서 설정이 변경되었을 때 호출
    public void UpdateState()
    {
        if (IsEnableMusic()) Play();
        else Stop();
    }
}
