using UnityEngine;
using UnityEngine.UI;

public class MusicSetting : MonoBehaviour {

    GameObject obBG;
    GameObject obEf;
    [HideInInspector] public AudioSource adBG;
    [HideInInspector] public AudioSource adEf;
    [SerializeField] private Slider[] sl;

    private void Awake()
    {
        obBG = GameObject.Find("MusicController");
        adBG = obBG.gameObject.GetComponent<AudioSource>();

        obEf = GameObject.Find("SoundController");
        adEf = obEf.gameObject.GetComponent<AudioSource>();
    }

    public void SetMusicControll() {
        adBG.volume = sl[0].value;
    }

    public void SetSoundControll() {
        adEf.volume = sl[1].value;
    }
}
