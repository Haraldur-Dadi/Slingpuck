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
    public AudioClip buttonClick;
    public AudioClip puckCollission;
    public AudioClip slingshotRelease;

    public Slider musicVolSlider;
    public Slider sfxVolSlider;

    private void Start() {
        musicVolSlider.onValueChanged.AddListener(delegate { ChangeMusicVol(musicVolSlider.value); });
        sfxVolSlider.onValueChanged.AddListener(delegate { ChangeSfxVol(sfxVolSlider.value); });

        musicVolSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
        sfxVolSlider.value = PlayerPrefs.GetFloat("SfxVol", 1f);
    }

    public void ChangeMusicVol(float vol) {
        musicAudioSource.volume = Mathf.Round(vol * 100) / 100;
        PlayerPrefs.SetFloat("MusicVol", musicAudioSource.volume);
    }

    public void ChangeSfxVol(float vol) {
        sfxAudioSource.volume = Mathf.Round(vol * 100) / 100;
        PlayerPrefs.SetFloat("SfxVol", sfxAudioSource.volume);
    }

    public void PlayButtonClick() {
        sfxAudioSource.PlayOneShot(buttonClick);
    }
    public void PlayPuckCollission() {
        sfxAudioSource.PlayOneShot(puckCollission);
    }
    public void PlaySlingshotRelease() {
        sfxAudioSource.PlayOneShot(slingshotRelease);
    }

    public void ChangeMusicPitch(int team1, int team2) {
        if (team1 == 2 || team2 == 2) {
            StartCoroutine(SwitchTempo(musicAudioSource.pitch, .95f));
        } else if (team1 == 1 || team2 == 1) {
            StartCoroutine(SwitchTempo(musicAudioSource.pitch, 1f));
        } else {
            StartCoroutine(SwitchTempo(musicAudioSource.pitch, .85f));
        }
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
