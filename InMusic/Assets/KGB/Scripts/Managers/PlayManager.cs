using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayManager : MonoBehaviour
{
    [SerializeField] VideoClip playBackgroundVideo;
    [SerializeField] Image playBackgroundImage;

    [SerializeField] VideoPlayer videoPlayer;
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
        SetMusic();
        SetSound();
        SetBackground();
    }

    void SetMusic()
    {
        //선택한 노래 리소스 세팅
        string musicName = GameManager.Instance.curBMS.wavInfo.wav;
        string path = $"Sound/Music/{musicName}";
        musicSound.clip = Resources.Load<AudioClip>(path);
        Debug.Log(path);
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

    }
}
