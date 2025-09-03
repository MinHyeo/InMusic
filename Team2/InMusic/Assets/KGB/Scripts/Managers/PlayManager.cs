using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayManager : MonoBehaviour
{
    [SerializeField] VideoClip playBackgroundVideo;
    [SerializeField] Image playBackgroundImage;

    public VideoPlayer videoPlayer;
    public AudioSource hitSound;
    public AudioSource musicSound;
    void Awake()
    {
        // musicSound가 할당되지 않았으면 자동으로 AudioSource 컴포넌트 찾기
        if (musicSound == null)
        {
            musicSound = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("musicSound가 할당되지 않아 새 AudioSource를 추가했습니다.");
        }

        // hitSound가 할당되지 않았으면 자동으로 AudioSource 컴포넌트 찾기
        if (hitSound == null)
        {
            hitSound = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("hitSound가 할당되지 않아 새 AudioSource를 추가했습니다.");
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetResources()
    {
        ResetResources();
        SetMusic();
        SetSound();
        SetBackground();
    }

    void SetMusic()
    {
        //선택한 노래 리소스 세팅
        //string musicName = GameManagerProvider.Instance.curBMS.wavInfo.wav;
        //string path = $"{GameManagerProvider.Instance.resourcePath}/{musicName}";
        //musicSound.clip = Resources.Load<AudioClip>(path);
        //Debug.Log(path);
        musicSound.clip = GameManager_PSH.Instance.GetComponent<MusicData>().Audio;
    }

    void SetSound()
    {
        // 효과음 세팅
        string path = $"Sound/SFX/hitSound1";
        hitSound.clip = Resources.Load<AudioClip>(path);
        Debug.Log(path);
    }
    
    void SetBackground()
    {
        //string musicName = GameManagerProvider.Instance.curBMS.wavInfo.wav;
        //string path = GameManagerProvider.Instance.resourcePath + $"/Back_{musicName}";
        //Debug.Log(path);
        //playBackgroundVideo = Resources.Load<VideoClip>(path);
        //if (playBackgroundVideo != null) {
        //    videoPlayer.clip = playBackgroundVideo;
        //}
        //else if (playBackgroundVideo == null) {
        //    Sprite backgroundSprite = Resources.Load<Sprite>(path);
        //    if (backgroundSprite != null)
        //    {
        //        playBackgroundImage.sprite = backgroundSprite;
        //    }
        //    else
        //    {
        //        Debug.LogError($"'{path}' 경로에서 VideoClip 또는 Sprite 파일을 찾을 수 없습니다.");
        //    }
        //}

        if (GameManager_PSH.Instance.GetComponent<MusicData>().MuVi != null) {
            playBackgroundVideo = GameManager_PSH.Instance.GetComponent<MusicData>().MuVi;
            videoPlayer.clip = playBackgroundVideo;
        }
        else
        {
            playBackgroundImage.sprite = GameManager_PSH.Instance.GetComponent<MusicData>().Album;
        }
    }

    public void ResetResources()
    {
        playBackgroundVideo = null;
        playBackgroundImage.sprite = null;
        videoPlayer.clip= null;
        videoPlayer.targetTexture.Release();
    }
}
