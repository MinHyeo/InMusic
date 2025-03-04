using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Play
{
    public enum SoundType 
    {
        Master = 0,
        SFX,
        BGM,
    }

    public enum PlayStyle
    {
        Normal,
        Highlight,
    }

    public class SoundManager : SingleTon<SoundManager>
    {
        // 현재 사용 X
        // FMOD Variable 
        [Header("music")]
        FMOD.System fmodSystem;
        FMOD.ChannelGroup musicChannelGroup;
        FMOD.Sound musicSound;
        FMOD.Channel musicChannel;

        private EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        public float frequency;
        private int startTimeSeconds = 60;
        public uint positionInSamples;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD 초기화
            Init();
        }

        private void Init()
        {
            //fmodSystem = RuntimeManager.CoreSystem;
            //fmodSystem.createChannelGroup("Music", out musicChannelGroup);

            RuntimeManager.LoadBank("Master");
            RuntimeManager.LoadBank("BGM");
            RuntimeManager.LoadBank("SFX");

            // FMOD에서 Bus 가져오기
            masterBus = RuntimeManager.GetBus("bus:/");
            bgmBus = RuntimeManager.GetBus("bus:/BGM");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");

            DontDestroyOnLoad(this.gameObject);
        }

        public void SetVolume(int soundType, float volume) 
        {
            switch (soundType)
            {
                case (int)SoundType.Master:
                    masterBus.setVolume(volume);
                    break;
                case (int)SoundType.SFX:
                    sfxBus.setVolume(volume);
                    break;
                case (int)SoundType.BGM:
                    bgmBus.setVolume(volume);
                    break;
            }

            //볼륨 변경 확인 테스트 코드
            //float masterVolume, sfxVolume, bgmVolume;
            //masterBus.getVolume(out masterVolume);
            //sfxBus.getVolume(out sfxVolume);
            //bgmBus.getVolume(out bgmVolume);
            //Debug.Log($"master 볼륨 : {masterVolume}, SFX 볼륨 : {sfxVolume}, BGM 볼륨 : {bgmVolume}");
        }

        private void LoadSong(string songName)
        {
            //노래 불러오기
            if (bgmInstance.isValid())
            {
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.release();
            }
            string bgmEventPart = "event:/BGM/" + songName;
            bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);
        }

        public void PlayBGM(string songName, PlayStyle style)
        {
            // 노래 불러오기
            LoadSong(songName);

            // 노래 하이라이트 설정
            if(style == PlayStyle.Highlight)
            {
                int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000);
                bgmInstance.setTimelinePosition(startTimeMilliseconds);
            }

            //노래 시작
            bgmInstance.start();
            // 채널 그룹 가져오기
            bgmInstance.getChannelGroup(out masterChannelGroup);
        }

        //public void PlayBGM(string bgmEventPart, PlayStyle style)
        //{
        //    bgmInstance.start();

        //    isPlaying = true;
        //}

        //public void PlayBGMHighLight(string songName)
        //{
        //    LoadSong(songName);

        //    //bgmInstance.setParameterByName("BGM_Loop", 1.0f);

        //    int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000); // 44.1kHz 기준
        //    bgmInstance.setTimelinePosition(startTimeMilliseconds);

        //    bgmInstance.start();
        //}

        public void Pause(bool isPause)
        {
            musicChannel.setPaused(isPause);
        }

        //public void SongInit(string songName)
        //{
        //    //FMOD 초기화
        //    //Init();

        //    //노래 불러오기
        //    LoadSong(songName);

        //    //현재 샘플 계산
        //    frequency = GetCurrentFrequency();
        //}

        public float GetCurrentFrequency()
        {
            Debug.Log("dddddddd");
            if (masterChannelGroup.hasHandle())
            {
                Debug.Log("dddd");
                FMOD.DSP dsp;
                masterChannelGroup.getDSP(0, out dsp);  // DSP 0번 가져오기

                // DSP가 연결되었는지 확인
                FMOD.DSP_PARAMETER_DESC paramDesc;
                dsp.getParameterInfo(0, out paramDesc);

                if(paramDesc.type == FMOD.DSP_PARAMETER_TYPE.FLOAT)
                {
                    dsp.getParameterFloat(0, out frequency);  // DSP에서 주파수 가져오기
                    Debug.Log($"SoundManager에서 샘플 구함 : {frequency}");
                }
                else
                {
                    Debug.LogError("DSP Parameter 0 type is not FLOAT");
                }     
            }

            return frequency;
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