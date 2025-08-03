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
        // musicSound�� �Ҵ���� �ʾ����� �ڵ����� AudioSource ������Ʈ ã��
        if (musicSound == null)
        {
            musicSound = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("musicSound�� �Ҵ���� �ʾ� �� AudioSource�� �߰��߽��ϴ�.");
        }

        // hitSound�� �Ҵ���� �ʾ����� �ڵ����� AudioSource ������Ʈ ã��
        if (hitSound == null)
        {
            hitSound = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("hitSound�� �Ҵ���� �ʾ� �� AudioSource�� �߰��߽��ϴ�.");
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
        //������ �뷡 ���ҽ� ����
        //string musicName = GameManagerProvider.Instance.curBMS.wavInfo.wav;
        //string path = $"{GameManagerProvider.Instance.resourcePath}/{musicName}";
        //musicSound.clip = Resources.Load<AudioClip>(path);
        //Debug.Log(path);
        musicSound.clip = GameManager_PSH.Instance.GetComponent<MusicData>().Audio;
    }

    void SetSound()
    {
        // ȿ���� ����
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
        //        Debug.LogError($"'{path}' ��ο��� VideoClip �Ǵ� Sprite ������ ã�� �� �����ϴ�.");
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
