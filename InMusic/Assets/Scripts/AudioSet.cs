using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // Scrollbars for volume control
    public Scrollbar masterVolumeScrollbar;
    public Scrollbar sfxVolumeScrollbar;
    public Scrollbar bgmVolumeScrollbar;

    // Texts to display the percentage
    public Text masterVolumeText;
    public Text sfxVolumeText;
    public Text bgmVolumeText;

    // Audio Sources for SFX and BGM
    public AudioSource sfxAudioSource;
    public AudioSource bgmAudioSource;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    void Start()
    {
        // Initialize scrollbars and text
        masterVolumeScrollbar.value = masterVolume;
        sfxVolumeScrollbar.value = sfxVolume;
        bgmVolumeScrollbar.value = bgmVolume;

        UpdateMasterVolume();
        UpdateSFXVolume();
        //UpdateBGMVolume();

        // Add listeners for scrollbars
        masterVolumeScrollbar.onValueChanged.AddListener(delegate { UpdateMasterVolume(); });
        sfxVolumeScrollbar.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
        //bgmVolumeScrollbar.onValueChanged.AddListener(delegate { UpdateBGMVolume(); });
    }

    // Update Master Volume
    public void UpdateMasterVolume()
    {
        masterVolume = masterVolumeScrollbar.value;
        AudioListener.volume = masterVolume; // Sets overall game volume
        masterVolumeText.text = Mathf.RoundToInt(masterVolume * 100) + "%";
    }

    // Update SFX Volume
    public void UpdateSFXVolume()
    {
        sfxVolume = sfxVolumeScrollbar.value;
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume; // Sets SFX volume
        }
        sfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100) + "%";
    }

    // Update BGM Volume
    //public void UpdateBGMVolume()
    //{
    //    bgmVolume = bgmVolumeScrollbar.value;
    //    if (bgmAudioSource != null)
    //    {
    //        bgmAudioSource.volume = bgmVolume; // Sets BGM volume
    //    }
    //    bgmVolumeText.text = Mathf.RoundToInt(bgmVolume * 100) + "%";
    //}
}
