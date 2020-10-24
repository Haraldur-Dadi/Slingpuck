using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour {
    #region Instance
    public static AudioManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            return;
        }
    }
    #endregion
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip menuThemeSong;
    public AudioClip buttonClick;

    public float musicVol;
    public float sfxVol;

    public Slider musicVolSlider;
    public TextMeshProUGUI musicVolTxt;

    public Slider sfxVolSlider;
    public TextMeshProUGUI sfxVolTxt;

    private void Start() {
        ChangeMusicVol(PlayerPrefs.GetFloat("MusicVol", 1f));
        ChangeSfxVol(PlayerPrefs.GetFloat("SfxVol", 1f));
        musicAudioSource.clip = menuThemeSong;
        musicAudioSource.pitch = 0.85f;
        musicAudioSource.Play();

        musicVolSlider.value = musicVol;
        sfxVolSlider.value = sfxVol;

        musicVolSlider.onValueChanged.AddListener(delegate { ChangeMusicVol(musicVolSlider.value); });
        sfxVolSlider.onValueChanged.AddListener(delegate { ChangeSfxVol(sfxVolSlider.value); });
    }

    public void ChangeMusicVol(float vol) {
        musicVol = Mathf.Round(vol * 100) / 100;
        musicVolTxt.text = (int) (musicVol * 100) + "%";
        musicAudioSource.volume = musicVol;

        PlayerPrefs.SetFloat("MusicVol", musicVol);
    }

    public void ChangeSfxVol(float vol) {
        sfxVol = Mathf.Round(vol * 100) / 100;
        sfxVolTxt.text = (int)(sfxVol * 100) + "%";
        sfxAudioSource.volume = sfxVol;

        PlayerPrefs.SetFloat("SfxVol", sfxVol);
    }

    public void PlayButtonClick() {
        sfxAudioSource.PlayOneShot(buttonClick);
    }

    public void ResetMusicTempo() {
        StartCoroutine(SwitchTempo(musicAudioSource.pitch, .85f));
    }
    public void PucksLeft2() {
        StartCoroutine(SwitchTempo(musicAudioSource.pitch, .95f));
    }
    public void PucksLeft1() {
        StartCoroutine(SwitchTempo(musicAudioSource.pitch, 1f));
    }


    private IEnumerator SwitchTempo(float valueFrom, float valueTo) {
        float t = 0f;
        while (t <= 1f) {
            musicAudioSource.pitch = Mathf.Lerp(valueFrom, valueTo, t);
            t += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
