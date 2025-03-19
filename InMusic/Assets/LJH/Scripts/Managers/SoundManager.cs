using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance ??= new SoundManager();

    public float MasterVolume { get; private set; } = 1f;
    public float BGMVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private SoundManager()
    {
        // 저장된 볼륨 설정 불러오기
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        // MasterVolume을 기반으로 BGM, SFX 볼륨 조절
        float adjustedBGMVolume = MasterVolume * BGMVolume;
        float adjustedSFXVolume = MasterVolume * SFXVolume;

        Debug.Log($"[Volume Updated] Master: {MasterVolume}, BGM: {adjustedBGMVolume}, SFX: {adjustedSFXVolume}");
    }

    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("BGMVolume", BGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }
}
