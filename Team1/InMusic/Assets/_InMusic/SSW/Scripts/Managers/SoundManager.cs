using UnityEngine;
using System.Collections.Generic;

public class SoundManager : Managers.Singleton<SoundManager>
{
    [Header("SFX용 AudioSource (2D 사운드)")]
    [SerializeField]
    private AudioSource sfxSource;

    [System.Serializable]
    public class SFXAudioData
    {
        public SFXType sfxType;     // 열거형 (SFXType)
        public AudioClip audioClip; // 재생할 사운드 파일
    }

    [Header("SFX 목록")]
    [SerializeField] 
    private SFXAudioData[] sfxAudioList;

    private Dictionary<SFXType, AudioClip> sfxDictionary;

    protected override void Awake() {
        base.Awake();
        // Dictionary 초기화
        sfxDictionary = new Dictionary<SFXType, AudioClip>();
        foreach (var data in sfxAudioList)
        {
            // 같은 SFXType이 중복 등록되지 않도록 체크
            if (!sfxDictionary.ContainsKey(data.sfxType))
            {
                sfxDictionary.Add(data.sfxType, data.audioClip);
            }
        }

        // AudioSource가 미리 세팅되어 있지 않으면 동적으로 추가
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;
        }
    }


    /// <summary>
    /// SFXType에 해당하는 효과음을 플레이한다. (OneShot 방식)
    /// </summary>
    public void PlaySFX(SFXType type) {
        if (sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            // OneShot 사용 시, 끝날 때까지 자동 재생 후 종료
            // 따로 Stop()을 호출하지 않아도 됨
            sfxSource.PlayOneShot(clip);
            Debug.Log($"[SoundManager] {type} 효과음 재생");
        }
        else
        {
            Debug.LogWarning($"[SoundManager] {type}에 해당하는 클립이 등록되지 않았습니다.");
        }
    }
}
