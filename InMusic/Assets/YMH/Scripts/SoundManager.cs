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
            //FMOD 초기화
            Init();
        }

        private void Init()
        {
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

        public void Pause(bool isPause)
        {
            musicChannel.setPaused(isPause);
        }

        public void SongInit(string songName)
        {
            //FMOD 초기화
            Init();

            //노래 불러오기
            string path = "Assets/Resources/Song/" + songName + "/" + songName + ".ogg";
            UnityEngine.Debug.Log(path);
            FMOD.RESULT result = fmodSystem.createSound(path, FMOD.MODE.DEFAULT, out musicSound);
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError("노래 불러오기 실패" + result);
                return;
            }

            //현재 샘플 계산
            musicSound.getDefaults(out frequency, out _);
        }

        private void Update()
        {
            if (!this.isPlaying)
                return;

            //현재 샘플링 주파수 계산
            musicChannel.getPosition(out positionInSamples, FMOD.TIMEUNIT.PCM);

            //노래가 끝나는지 체크
            bool isPlaying;
            musicChannel.isPlaying(out isPlaying);

            if (!isPlaying)
                OnMusicEnd();

            fmodSystem.update();
        }

        private void OnMusicEnd()
        {
            UnityEngine.Debug.Log("노래 끝");
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