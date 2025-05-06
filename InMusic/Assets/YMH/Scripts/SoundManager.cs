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
        private FMOD.Studio.EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        private int startTimeSeconds = 60;
        public int currentPositionMs;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD 초기화
            Init();
        }

        #region Sound Init
        private void Init()
        {
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
        public void SongInit(Song songName, PlayStyle style)
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
        }

        public void Play()
        {
            bgmInstance.start();
            isPlaying = true;
        }
        #endregion

        #region Pause Music
        /// <summary>
        /// 노래 일시정지
        /// </summary>
        /// <param name="isPause">멈추고 싶으면 true</param>
        public void SetPause(bool isPause)
        {
            if(isPlaying == isPause)
            {
                isPlaying = !isPause;
                bgmInstance.setPaused(isPause);
            }
            else
            {
                UnityEngine.Debug.LogError("노래 플레이 상태 겹침");
            }
        }
        #endregion

        #region Check Music End
        public IEnumerator WaitForMusicEnd(Action onComplete)
        {
            if (!bgmInstance.isValid())
            {
                UnityEngine.Debug.LogError("BGM 인스턴스가 유효하지 않습니다.");
                yield break;
            }

            FMOD.Studio.PLAYBACK_STATE state;

            do
            {
                bgmInstance.getPlaybackState(out state);
                yield return null; // 매 프레임마다 확인
            } while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);

            UnityEngine.Debug.Log("노래가 끝났습니다!");
            onComplete?.Invoke(); // 콜백 실행
        }
        #endregion

        public int GetTimelinePosition()
        {
            bgmInstance.getTimelinePosition(out currentPositionMs);

            return currentPositionMs;
        }

        private void OnMusicEnd()
        {
            UnityEngine.Debug.Log("노래 끝");
            PlayManager.Instance.End();
        }

        public void End() 
        {
            //bgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            isPlaying = false;
        }
    }
}