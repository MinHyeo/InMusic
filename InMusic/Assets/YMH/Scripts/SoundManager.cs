using UnityEngine;
using FMOD;
using UnityEngine.Rendering.Universal;

namespace Play 
{
    public class SoundManager : SingleTon<SoundManager>
    {
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
            Init();
        }

        private void Init()
        {
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

        public void Pause(bool isPause)
        {
            musicChannel.setPaused(isPause);
        }

        public void SongInit(string songName)
        {
            //FMOD �ʱ�ȭ
            Init();

            //�뷡 �ҷ�����
            string path = "Assets/Resources/Song/" + songName + "/" + songName + ".ogg";
            UnityEngine.Debug.Log(path);
            FMOD.RESULT result = fmodSystem.createSound(path, FMOD.MODE.DEFAULT, out musicSound);
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError("�뷡 �ҷ����� ����" + result);
                return;
            }

            //���� ���� ���
            musicSound.getDefaults(out frequency, out _);
        }

        private void Update()
        {
            if (!this.isPlaying)
                return;

            //���� ���ø� ���ļ� ���
            musicChannel.getPosition(out positionInSamples, FMOD.TIMEUNIT.PCM);

            //�뷡�� �������� üũ
            bool isPlaying;
            musicChannel.isPlaying(out isPlaying);

            if (!isPlaying)
                OnMusicEnd();

            fmodSystem.update();
        }

        private void OnMusicEnd()
        {
            UnityEngine.Debug.Log("�뷡 ��");
            PlayManager.Instance.End();
        }

        public void End()
        {
            musicChannel.stop();
            musicChannelGroup.release();

            musicSound.release();

            fmodSystem.close();
            fmodSystem.release();

            isPlaying = false;
        }
    }
}