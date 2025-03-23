using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System;
using FMOD;
using System.Threading.Tasks;

namespace Play
{
    #region Enum Define
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
    #endregion

    public class SoundManager : SingleTon<SoundManager>
    {
        #region 사용X
        // FMOD Variable 
        [Header("music")]
        FMOD.System fmodSystem;
        FMOD.ChannelGroup musicChannelGroup;
        FMOD.Sound musicSound;
        FMOD.Channel musicChannel;
        #endregion

        private FMOD.Studio.EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        public int frequency;
        private int startTimeSeconds = 60;
        public uint positionInSamples;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD 초기화
            Init();
        }

        #region Sound Init
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
        #endregion

        #region Set Sound Volume
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

            #region 볼륨 변경 확인 테스트 코드
            //float masterVolume, sfxVolume, bgmVolume;
            //masterBus.getVolume(out masterVolume);
            //sfxBus.getVolume(out sfxVolume);
            //bgmBus.getVolume(out bgmVolume);
            //Debug.Log($"master 볼륨 : {masterVolume}, SFX 볼륨 : {sfxVolume}, BGM 볼륨 : {bgmVolume}");
            #endregion
        }
        #endregion

        #region Play Music
        /// <summary>
        /// SoundManager 노래 초기설정
        /// </summary>
        /// <param name="songName"></param>
        public async Task SongInit(Song songName, PlayStyle style)
        {
            //노래 제목 문자열 변환
            string songTitle = songName.ToString();

            //노래 불러오기
            //FMOD 상태 체크 및 초기화
            if (bgmInstance.isValid())
            {
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.release();
            }

            //노래 이벤트 불러오기
            string bgmEventPart = "event:/BGM/" + songTitle;
            bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);

            //노래 하이라이트 설정
            if(style == PlayStyle.Highlight)
            {
                int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000);
                bgmInstance.setTimelinePosition(startTimeMilliseconds);
            }

            //채널 그룹 가져오기
            //StartCoroutine(GetChannelGroup());
            await RunCoroutineAsTask(GetChannelGroup());
        }

        private Task RunCoroutineAsTask(IEnumerator coroutine)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(CoroutineWrapper(coroutine, tcs));
            return tcs.Task;
        }

        private IEnumerator CoroutineWrapper(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
        {
            yield return coroutine;
            tcs.SetResult(true);
        }

        private IEnumerator GetChannelGroup()
        {
            //노래 시작하고 바로 일시정지
            isPlaying = true;
            bgmInstance.start();
            Pause(!isPlaying);

            //프레임 대기
            yield return null;

            //채널그룹 가져오기
            FMOD.Studio.PLAYBACK_STATE state;
            do
            {
                yield return null;
                bgmInstance.getPlaybackState(out state);
            } while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING);
            bgmInstance.getChannelGroup(out masterChannelGroup);

            //샘플 구하기
            GetCurrentFrequency();
        }

        public void Play()
        {
            bgmInstance.start();
        }

        //private void LoadSong(string songName)
        //{
        //    //노래 불러오기
        //    if (bgmInstance.isValid())
        //    {
        //        bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //        bgmInstance.release();
        //    }
        //    string bgmEventPart = "event:/BGM/" + songName;
        //    bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);
        //}

        //public void PlayBGM(string songName, PlayStyle style)
        //{
        //    StartCoroutine(Play(songName, style));
        //}

        //private IEnumerator Play(string songName, PlayStyle style)
        //{
        //    // 노래 불러오기
        //    LoadSong(songName);

        //    // 노래 하이라이트 설정
        //    if (style == PlayStyle.Highlight)
        //    {
        //        int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000);
        //        bgmInstance.setTimelinePosition(startTimeMilliseconds);
        //    }

        //    //노래 시작
        //    bgmInstance.start();

        //    FMOD.Studio.PLAYBACK_STATE state;
        //    do
        //    {
        //        yield return null;
        //        bgmInstance.getPlaybackState(out state);
        //    } while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING);

        //    // 채널 그룹 가져오기
        //    bgmInstance.getChannelGroup(out masterChannelGroup);

        //    //yield return new WaitForSeconds(0.1f);

        //    //// 채널 그룹 가져오기
        //    //bgmInstance.getChannelGroup(out masterChannelGroup);
        //    //Debug.Log(masterChannelGroup);
        //}

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
        #endregion

        #region Pause Music
        public void Pause(bool isPause)
        {
            if(isPlaying != isPause)
            {
                isPlaying = isPause;
                musicChannel.setPaused(isPause);
            }
            else
            {
                UnityEngine.Debug.LogError("노래 플레이 상태 겹침");
            }
        }
        #endregion

        #region Get Frequency
        public void GetCurrentFrequency()
        {
            if (!masterChannelGroup.hasHandle())
            {
                UnityEngine.Debug.LogError("Master Channel Group has no handle");
                return;
            }

            FMOD.System system;
            FMOD.RESULT result = masterChannelGroup.getSystemObject(out system);
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError($"FMOD 시스템 가져오기 실패: {result}");
                return;
            }

            // 현재 샘플링 주파수 (예: 44100 Hz)
            FMOD.SPEAKERMODE speakerMode;
            int numRawSpeakers;
            result = system.getSoftwareFormat(out frequency, out speakerMode, out numRawSpeakers);
            UnityEngine.Debug.Log($"frequency : {frequency}");
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError($"FMOD 샘플링 주파수 가져오기 실패: {result}");
                return;
            }
        }

        //public void GetCurrentFrequency()
        //{
        //    if (masterChannelGroup.hasHandle())
        //    {
        //        FMOD.DSP dsp;
        //        masterChannelGroup.getDSP(0, out dsp);  // DSP 0번 가져오기

        //        // DSP가 연결되었는지 확인
        //        FMOD.DSP_PARAMETER_DESC paramDesc;
        //        dsp.getParameterInfo(0, out paramDesc);

        //        if(paramDesc.type == FMOD.DSP_PARAMETER_TYPE.FLOAT)
        //        {
        //            dsp.getParameterFloat(0, out frequency);  // DSP에서 주파수 가져오기
        //            Debug.Log($"SoundManager에서 샘플 구함 : {frequency}");
        //        }
        //        else
        //        {
        //            Debug.LogError("DSP Parameter 0 type is not FLOAT");
        //        }     
        //    }
        //    else
        //    {
        //        Debug.LogError("Master Channel Group has no handle");
        //    }
        //}
        #endregion

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