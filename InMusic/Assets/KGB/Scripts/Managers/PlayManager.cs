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
        SetMusic();
        SetSound();
        SetBackground();
    }

    void SetMusic()
    {
        //������ �뷡 ���ҽ� ����
        string musicName = GameManager.Instance.curBMS.wavInfo.wav;
        string path = $"Sound/Music/{musicName}";
        musicSound.clip = Resources.Load<AudioClip>(path);
        Debug.Log(path);
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

    }
}
