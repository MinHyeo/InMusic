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
            UnityEngine.Debug.LogError("Scene�� �������� SoundManager ����");
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
        //FMOD �ʱ�ȭ
        FMOD.Factory.System_Create(out fmodSystem);
        fmodSystem.init(512, FMOD.INITFLAGS.NORMAL, System.IntPtr.Zero);
    }

    public void Play()
    {
        //�뷡 ���
        UnityEngine.Debug.Log("�뷡 ���");
        fmodSystem.playSound(musicSound, musicChannelGroup, false, out musicChannel);
        musicChannel.setVolume(0.5f);

        isPlaying = true;
    }

    public void SongInit(string songName)
    {
        //�뷡 �ҷ�����
        string path = "Assets/Resources/Song/" + songName + "/" + songName + ".ogg";
        FMOD.RESULT result = fmodSystem.createSound(path, FMOD.MODE.DEFAULT, out musicSound);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("�뷡 �ҷ����� ����");
            return;
        }

        //���� ���� ���
        musicSound.getDefaults(out frequency, out _);
        UnityEngine.Debug.Log(frequency);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        //���� ���ø� ���ļ� ���
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
