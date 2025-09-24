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
        UI,
        BGM,
    }

    public enum PlayStyle
    {
        Normal,
        Highlight,
    }
    #endregion

    public class SoundManager : Singleton<SoundManager>
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
            //FMOD �ʱ�ȭ
            Init();
        }

        #region Sound Init
        private void Init()
        {
            RuntimeManager.LoadBank("Master");
            RuntimeManager.LoadBank("Musics");
            RuntimeManager.LoadBank("UI");

            // FMOD에서 Bus 가져오기
            masterBus = RuntimeManager.GetBus("bus:/");
            bgmBus = RuntimeManager.GetBus("bus:/Musics");
            sfxBus = RuntimeManager.GetBus("bus:/UI");

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
                case (int)SoundType.UI:
                    sfxBus.setVolume(volume);
                    break;
                case (int)SoundType.BGM:
                    bgmBus.setVolume(volume);
                    break;
            }

            #region ���� ���� Ȯ�� �׽�Ʈ �ڵ�
            //float masterVolume, sfxVolume, bgmVolume;
            //masterBus.getVolume(out masterVolume);
            //sfxBus.getVolume(out sfxVolume);
            //bgmBus.getVolume(out bgmVolume);
            //Debug.Log($"master ���� : {masterVolume}, SFX ���� : {sfxVolume}, BGM ���� : {bgmVolume}");
            #endregion
        }
        #endregion

        #region Play Music
        /// <summary>
        /// SoundManager 음악 초기설정
        /// </summary>
        /// <param name="songName"></param>
        public void SongInit(string songTitle, PlayStyle style)
        {
            //음악 로드하기
            //FMOD 정상 체크 및 초기화
            if (bgmInstance.isValid())
            {
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.release();
            }

            //음악 이벤트 불러오기
            string bgmEventPart = "event:/Musics/" + songTitle;
            bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);

            //�뷡 ���̶���Ʈ ����
            if (style == PlayStyle.Highlight)
            {
                bgmInstance.setTimelinePosition(startTimeSeconds * 1000);
                //bgmInstance.setParameterByName("BGM_Loop", 0f);
                RuntimeManager.StudioSystem.setParameterByName("BGM_Loop", 0f);
                bgmInstance.getParameterByName("BGM_Loop", out float value);
                UnityEngine.Debug.Log($"Highlight Play {value}");
            }
            else
            {
                //bgmInstance.setParameterByName("BGM_Loop", 1f);
                RuntimeManager.StudioSystem.setParameterByName("BGM_Loop", 1f);
                bgmInstance.getParameterByName("BGM_Loop", out float value);
                UnityEngine.Debug.Log($"Highlight Play {value}");
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
        /// �뷡 �Ͻ�����
        /// </summary>
        /// <param name="isPause">���߰� ������ true</param>
        public void SetPause(bool isPause)
        {
            if (isPlaying == isPause)
            {
                isPlaying = !isPause;
                bgmInstance.setPaused(isPause);
            }
            else
            {
                UnityEngine.Debug.LogError("�뷡 �÷��� ���� ��ħ");
            }
        }
        #endregion

        #region Check Music End
        public IEnumerator WaitForMusicEnd(Action onComplete)
        {
            if (!bgmInstance.isValid())
            {
                UnityEngine.Debug.LogError("Music instance is not valid.");
                yield break;
            }

            FMOD.Studio.PLAYBACK_STATE state;

            do
            {
                bgmInstance.getPlaybackState(out state);
                yield return null; // 다음 프레임까지 대기
            } while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);

            onComplete?.Invoke(); // 재생 완료
        }
        #endregion

        public int GetTimelinePosition()
        {
            bgmInstance.getTimelinePosition(out currentPositionMs);

            return currentPositionMs;
        }

        public void End()
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            isPlaying = false;
        }

        public void PlayUISound(string soundName)
        {
            // UI 사운드 재생
            string uiEventPath = "event:/UI/" + soundName;
            EventInstance uiEventInstance = RuntimeManager.CreateInstance(uiEventPath);
            uiEventInstance.start();
            uiEventInstance.release();
        }
    }
}