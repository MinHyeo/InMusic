using UnityEngine;
using FMOD;
using UnityEngine.Rendering.Universal;

public class SoundManager : MonoBehaviour
{
    [Header("Instance")]
    public static SoundManager Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            UnityEngine.Debug.LogError("Scene에 여러개의 SoundManager 존재");
        }
        Instance = this;
    }

    [Header("music")]
    FMOD.System fmodSystem;
    FMOD.ChannelGroup musicChannelGroup;
    FMOD.Sound musicSound;
    FMOD.Channel musicChannel;

    [Header("MusicInfo")]
    public float frequency;
    public uint positionInSamples;

    private bool isPlaying = false;

    private void Start()
    {
        //FMOD 초기화
        FMOD.Factory.System_Create(out fmodSystem);
        fmodSystem.init(512, FMOD.INITFLAGS.NORMAL, System.IntPtr.Zero);
    }

    public void Play()
    {
        //노래 재생
        UnityEngine.Debug.Log("노래 재생");
        fmodSystem.playSound(musicSound, musicChannelGroup, false, out musicChannel);
        musicChannel.setVolume(0.5f);

        isPlaying = true;
    }

    public void SongInit(string songName)
    {
        //노래 불러오기
        string path = "Assets/Resources/Song/" + songName + "/" + songName + ".ogg";
        FMOD.RESULT result = fmodSystem.createSound(path, FMOD.MODE.DEFAULT, out musicSound);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("노래 불러오기 실패");
            return;
        }

        //현재 샘플 계산
        musicSound.getDefaults(out frequency, out _);
        UnityEngine.Debug.Log(frequency);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        //현재 샘플링 주파수 계산
        musicChannel.getPosition(out positionInSamples, FMOD.TIMEUNIT.PCM);
    }

    public void End()
    {
        musicChannelGroup.release();
        musicSound.release();
        fmodSystem.close();
        fmodSystem.release();

        isPlaying = false;
    }
}
